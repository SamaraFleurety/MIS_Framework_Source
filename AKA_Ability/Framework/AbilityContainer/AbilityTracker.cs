using AKA_Ability.InertiaConditioner;
using AKA_Ability.SharedData;
using AKA_Ability.TickCondition;
using System;
using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    //aka tracker生成时参数。此参数在生成后不可被调用，也不见得一定使用此参数生成tracker。
    public class AbilityTrackerGenerationProperty
    {
        public Type AKATrackerClass = typeof(AbilityTracker);

        //必须这玩意返回true，同时满足trakcer内其他条件才会tick。
        public Type tickCondition = typeof(TiC_ColonistOnly);

        //对于多个技能共享数据(例如cd)的，这里面的才是原始数据。不可以把CD_TrackerShared等非真实数据放进来！
        public AbilityTrackerSharedDataProperty sharedDataProperty = null;
        public List<AKAbilityDef> abilities = new();

        public AbilityTrackerGenerationProperty()
        {
        }

        public AbilityTracker GenerateAbilityTracker(Pawn casterPawn)
        {
            //新版好像自动兼容ce了
            /*if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null)
            {
                return;
            }*/
            AbilityTracker tracker = (AbilityTracker)Activator.CreateInstance(AKATrackerClass, casterPawn);
            tracker.tickCondition = (TickCondion_Base)Activator.CreateInstance(tickCondition, tracker);
            if (sharedDataProperty != null)
            {
                tracker.sharedData = (AbilityTrackerSharedData_Base)Activator.CreateInstance(sharedDataProperty.sharedDataType, tracker, sharedDataProperty);
            }
            if (!abilities.NullOrEmpty())
            {
                foreach (AKAbilityDef i in this.abilities)
                {
                    tracker.AddAbility(i);
                    //AKAbilityMaker.MakeAKAbility(i, AKATracker);
                }
            }
            return tracker;
        }
    }

    //不能保证一个pawn只有一个tracker。
    public class AbilityTracker : IExposable
    {
        //不可以改，许多ae都默认使用者是pawn
        public Pawn owner;

        public ThingWithComps holder;//为TC_AKATracker新增的载体引用字段

        public int indexActiveGroupedAbility = -1;

        public List<AKAbility_Base> innateAbilities = new();

        public List<AKAbility_Base> groupedAbilities = new();

        //召唤技能列表，是技能的子集。存在这个是因为有时候需要调用召唤物
        public List<AKAbility_Summon> summonAbilities = new();

        //取消innate和group的区分，统一成此类。没有做完
        public List<AKAbility_Base> abilitiesUnified = new();

        //fixme:没做完
        public AKAbility_Base barDisplayedAbility = null;   //舟味ui显示技能指示时 有多个技能则仅显示此技能。不允许是未被选中的分组技能。

        public bool shouldRefreshActiveStatus = true;

        //对于使用共享数据(比如cd)这里面的才是真实数据
        public AbilityTrackerSharedData_Base sharedData = null;

        public TickCondion_Base tickCondition = null;
        public AKAbility_Base SelectedGroupedAbility
        {
            get
            {
                if (indexActiveGroupedAbility < 0 || groupedAbilities.Count <= indexActiveGroupedAbility) return null;
                return groupedAbilities[indexActiveGroupedAbility];
            }
        }

        //用这个构造器需要手动绑定owner
        public AbilityTracker()
        {
        }

        public AbilityTracker(Pawn p) : this()
        {
            owner = p;
        }

        public void Tick()
        {
            if (tickCondition != null && !tickCondition.TickableNow() /*owner == null ||  !owner.IsColonist*/) return;
            foreach (AKAbility_Base i in innateAbilities)
            {
                if (i.Inertia) continue;
                i.Tick();
            }

            if (indexActiveGroupedAbility > -1 && groupedAbilities.Count > indexActiveGroupedAbility) groupedAbilities[indexActiveGroupedAbility].Tick();
        }

        public IEnumerable<Command> GetGizmos()
        {
            if (owner == null) yield break;

            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count > 1) yield break;

            //Command c;
            //固有的 不取决于分组的技能会一直显示
            foreach (AKAbility_Base i in innateAbilities)
            {
                foreach (Command c1 in i.GetGizmos())
                {
                    yield return c1;
                }
            }
            //分组技能 仅显示最多1个
            if (indexActiveGroupedAbility != -1 && groupedAbilities.Count > 0)
            {
                foreach (Command c2 in groupedAbilities[indexActiveGroupedAbility].GetGizmos())
                {
                    yield return c2;
                }
            }
            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "[AKA]立刻回复SP",
                    defaultDesc = "desc",
                    icon = BaseContent.BadTex,
                    action = delegate ()
                    {
                        if (SelectedGroupedAbility != null) SelectedGroupedAbility.cooldown.charge = SelectedGroupedAbility.cooldown.MaxCharge;
                        foreach (AKAbility_Base ab in innateAbilities)
                        {
                            ab.cooldown.charge = ab.cooldown.MaxCharge;
                        }
                    }
                };
            }
        }

        public virtual AKAbility_Base AddAbility(AKAbilityDef def)
        {
           /* AKAbility_Base ability = (AKAbility_Base)Activator.CreateInstance(def.abilityClass, def, this);

            foreach (Type icType in def.inertiaConditions)
            {
                InertiaConditioner_Base ic = (InertiaConditioner_Base)Activator.CreateInstance(icType, ability);
                ability.inertiaConditions.Add(ic);
            }*/

            AKAbility_Base ability = def.MakeAbility(this);

            List<AKAbility_Base> allAbilities = this.innateAbilities;
            if (def.grouped)
            {
                allAbilities = this.groupedAbilities;
                this.indexActiveGroupedAbility = 0;
            }

            if (ability is AKAbility_Summon ab_summon)
            {
                summonAbilities.Add(ab_summon);
            }

            //fixme:临时的 以后优化
            for (int i = 0; i < allAbilities.Count; ++i)
            {
                AKAbility_Base ab = allAbilities[i];
                if (ab.def.sortOrder >= def.sortOrder)
                {
                    allAbilities.Insert(i, ability);
                    return ability;
                }
            }

            allAbilities.Add(ability);

            return ability;
        }

        //技能成功释放后调用 现在用来播放干员音效
        public virtual void Notify_AbilityCasted(AKAbility_Base ability)
        {
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref indexActiveGroupedAbility, "indexAGA");
            Scribe_Collections.Look(ref innateAbilities, "iA", LookMode.Deep, this);
            Scribe_Collections.Look(ref groupedAbilities, "gA", LookMode.Deep, this);
            Scribe_Collections.Look(ref summonAbilities, "summonA", LookMode.Reference);
            Scribe_References.Look(ref barDisplayedAbility, "barA");
            Scribe_Deep.Look(ref sharedData, "sharedData", this);
            Scribe_Deep.Look(ref tickCondition, "ticker", this);
            Scribe_References.Look(ref holder, "holder");

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (AKAbility_Base i in innateAbilities) i.container = this;
                foreach (AKAbility_Base j in groupedAbilities) j.container = this;

                if (owner != null) SpawnSetup();
            }
        }

        //技能被装载上了pawn调用这个
        public virtual void SpawnSetup()
        {
            foreach (AKAbility_Base ia in innateAbilities)
            {
                ia.SpawnSetup();
            }
            SelectedGroupedAbility?.SpawnSetup();
        }

        //技能不再被绑定调用这个 va永远不被卸载，但是tcp上面的有可能会
        public virtual void PostDespawn()
        {
            foreach (AKAbility_Base ia in innateAbilities)
            {
                ia.PostDespawn();
            }
            foreach (AKAbility_Base ga in groupedAbilities)
            {
                ga.PostDespawn();
            }
        }

        public IEnumerable<Thing> AllSummoneds()
        {
            foreach (AKAbility_Summon i in summonAbilities)
            {
                //还没想好召唤之后又切换技能怎么算
                //if (i.def.grouped && SelectedGroupedAbility != i) continue;
                foreach (Thing summoned in i.summoneds)
                {
                    yield return summoned;
                }
            }
        }

        public T TryGetSharedData<T>() where T : AbilityTrackerSharedData_Base
        {
            if (sharedData is not null and T data) return data;
            return null;
        }

        //遍历，因为技能没几个 要是以后多了找个缓存
        public AKAbility_Base TryGetAbility(AKAbilityDef def)
        {
            foreach (AKAbility_Base ab in innateAbilities)
            {
                if (ab.def == def) return ab;
            }
            foreach (AKAbility_Base ab in groupedAbilities)
            {
                if (ab.def == def) return ab;
            }
            return null;
        }
    }
}
