using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace AKA_Ability.Summon
{
    //召唤物会带此comp
    public class TCP_SummonedProperties : CompProperties
    {
        public int timeExpire = -1;
        //public int summonCap = 1;        //最多同时存在多少个

        public bool allowDraft = false;
        public TCP_SummonedProperties()
        {
            compClass = typeof(TC_SummonedProperties);
        }
    }

    public class TC_SummonedProperties : ThingComp
    {
        public TCP_SummonedProperties Props => props as TCP_SummonedProperties;
        public int timeSpawned;

        //上面时间大于这个就会消失。为-1不消失
        public int TimeExpire => Props.timeExpire;

        public Pawn Parent_Summoner;

        public AKAbility_Summon Parent_Ability;

        //仅在make thing时会调用一次
        public override void PostPostMake()
        {
            base.PostPostMake();
            if (Props.allowDraft)
            {
                Pawn summonedPawn = parent as Pawn;
                if(summonedPawn != null && summonedPawn.drafter == null) summonedPawn.drafter = new Pawn_DraftController(summonedPawn);
            }
        }

        //每次spawn都会被调用。thing会被先make然后可能会也可能不会spawn。
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (Props.allowDraft)
            {
                Pawn summonedPawn = parent as Pawn;
                if (summonedPawn != null && summonedPawn.drafter == null) summonedPawn.drafter = new Pawn_DraftController(summonedPawn);
            }
        }

        public override void CompTick()
        {
            base.CompTick();
        }

        public virtual void Tick(int amt)
        {
            if (TimeExpire != -1)
            {
                timeSpawned += amt;
                if (timeSpawned > TimeExpire)
                {
                    parent.Destroy(DestroyMode.QuestLogic);
                }
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            Parent_Ability.Notify_SummonedVanished(parent);
            base.PostDestroy(mode, previousMap);
        }

        //让召唤物可以征召。
        //原版机制中，需要智能是人类（但同时会导致一堆想法）或者是殖民地机械（需要有机械师控制）才能draft，直接改xml我没找到可行又方便的做法
        //以下这玩意严重不可靠！利用了泰南drafter内部getgizmo缺乏检测iscolonist实现，很有可能随着更新失效
        //如果哪天真失效 建议手写gizmo切换draft亦或者插入反射调用的函数的内部判定
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Pawn summonedPawn = parent as Pawn;
            if (Props.allowDraft)
            {
                //Log.Message("allow draft");
                MethodInfo method = typeof(Pawn_DraftController).GetMethod("GetGizmos", BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (Gizmo i in (IEnumerable<Gizmo>)method.Invoke(summonedPawn.drafter, new object[] { }))
                {
                    yield return i;
                }
            }

            foreach (Gizmo j in base.CompGetGizmosExtra())
            {
                yield return j;
            }

            //return base.CompGetGizmosExtra();
            //fixme:手动取消召唤gizmo
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref timeSpawned, "spawn");
            //Scribe_Values.Look(ref timeExpire, "expire", -1);
            Scribe_References.Look(ref Parent_Summoner, "p_master");
            Scribe_References.Look(ref Parent_Ability, "p_ability");
        }

    }
}
