using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace AKA_Ability
{
    //不能保证一个pawn只有一个tracker。
    public class AbilityTracker : IExposable
    {
        public Pawn owner;

        public int indexActiveGroupedAbility = -1;

        public List<AKAbility> innateAbilities = new List<AKAbility>();

        public List<AKAbility> groupedAbilities = new List<AKAbility>();

        //fixme:没做完
        public AKAbility barDisplayedAbility = null;   //舟味ui显示技能指示时 有多个技能则仅显示此技能。不允许是未被选中的分组技能。

        //召唤技能列表，是技能的子集。存在这个是因为有时候需要调用召唤物
        public List<AKAbility_Summon> summonAbilities = new();

        public AKAbility SelectedGroupedAbility
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
            foreach (AKAbility i in innateAbilities)
            {
                i.Tick();
            }
            if (groupedAbilities.Count > 0) groupedAbilities[indexActiveGroupedAbility].Tick();
        }

        public IEnumerable<Command> GetGizmos()
        {
            if (owner == null) yield break;

            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count > 1) yield break;

            Command c;
            //固有的 不取决于分组的技能会一直显示
            foreach (AKAbility i in innateAbilities)
            {
                c = i.GetGizmo();
                if (c != null) yield return c;
            }
            //分组技能 仅显示最多1个
            if (indexActiveGroupedAbility != -1 && groupedAbilities.Count > 0)
            {
                c = groupedAbilities[indexActiveGroupedAbility].GetGizmo();
                if (c != null) yield return c;
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
                        foreach (AKAbility ab in innateAbilities)
                        {
                            ab.cooldown.charge = ab.cooldown.MaxCharge;
                        }
                    }
                };
            }
        }

        public virtual AKAbility AddAbility(AKAbilityDef def)
        {
            AKAbility ability = (AKAbility)Activator.CreateInstance(def.abilityClass, def, this);

            if (def.grouped)
            {
                this.groupedAbilities.Add(ability);
                this.indexActiveGroupedAbility = 0;
            }
            else this.innateAbilities.Add(ability);

            return ability;
        }

        //技能成功释放后调用 现在用来播放干员音效
        public virtual void Notify_AbilityCasted(AKAbility ability)
        {

        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref indexActiveGroupedAbility, "indexAGA");

            Scribe_Collections.Look(ref innateAbilities, "iA", LookMode.Deep, this);
            Scribe_Collections.Look(ref groupedAbilities, "gA", LookMode.Deep, this);

            Scribe_References.Look(ref barDisplayedAbility, "barA");
            Scribe_Collections.Look(ref summonAbilities, "summonA", LookMode.Reference);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (AKAbility i in innateAbilities) i.container = this;
                foreach (AKAbility j in groupedAbilities) j.container = this;

                if (owner != null) SpawnSetup();
            }
        }

        //技能被装载上了pawn调用这个
        public virtual void SpawnSetup()
        {
            foreach (AKAbility ia in innateAbilities)
            {
                ia.SpawnSetup();
            }
            SelectedGroupedAbility?.SpawnSetup();
        }

        //技能不再被绑定调用这个 va永远不被卸载，但是tcp上面的有可能会
        public virtual void PostDespawn()
        {
            foreach (AKAbility ia in innateAbilities)
            {
                ia.PostDespawn();
            }
            foreach (AKAbility ga in groupedAbilities)
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
    }
}
