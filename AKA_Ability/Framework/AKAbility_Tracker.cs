using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_Tracker : IExposable
    {
        public Pawn owner;

        public int indexActiveGroupedAbility = 0;

        public List<AKAbility> innateAbilities = new List<AKAbility>();

        public List<AKAbility> groupedAbilities = new List<AKAbility>();

        public void Tick()
        {
            foreach(AKAbility i in innateAbilities)
            {
                i.Tick();
            }
            if (groupedAbilities.Count > 0) groupedAbilities[indexActiveGroupedAbility].Tick();
        }

        public IEnumerable<Gizmo> GetGizmos()
        {
            foreach (AKAbility i in innateAbilities) yield return i.GetGizmo();
            if (groupedAbilities.Count > 0)  yield return groupedAbilities[indexActiveGroupedAbility].GetGizmo();
        }

        public virtual void ExposeData()
        {
            Scribe_References.Look(ref owner, "p");
            Scribe_Values.Look(ref indexActiveGroupedAbility, "indexAGA");

            Scribe_Collections.Look(ref innateAbilities, "iA", LookMode.Deep);
            Scribe_Collections.Look(ref groupedAbilities, "gA", LookMode.Deep);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (AKAbility i in innateAbilities) i.container = this;
                foreach (AKAbility j in groupedAbilities) j.container = this;
            }
        }

        public virtual void PostPlayAbilitySound(AKAbility ability)
        {

        }
    }
}
