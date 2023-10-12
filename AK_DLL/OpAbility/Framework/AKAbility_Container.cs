using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class AKAbility_Container : IExposable
    {
        public OperatorDocument doc;

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

        public void ExposeData()
        {
            throw new NotImplementedException();
        }
    }
}
