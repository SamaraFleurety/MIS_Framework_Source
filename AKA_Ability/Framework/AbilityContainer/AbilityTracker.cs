using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
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
            foreach(AKAbility i in innateAbilities)
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
    }
}
