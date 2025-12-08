using System.Linq;
using Verse;

namespace AKA_Ability.Gizmos
{
    public class Gizmo_SelectAllSummon : Gizmo_AbilityCast_Action
    {
        public Gizmo_SelectAllSummon(AKAbility_Base parent) : base(parent)
        {
            action = delegate ()
            {
                Find.Selector.ClearSelection();
                Log.Message($"{parent.CasterPawn.Name}, {parent.container.AllSummoneds().Count()}");
                foreach (Thing t in parent.container.AllSummoneds())
                {
                    Log.Message("select summon1");
                    if (t.Spawned && t.Map == parent.CasterPawn.Map)
                    {
                        Log.Message("select summon2");
                        Find.Selector.Select(t);
                    }
                    Log.Message("select summon3");
                }
            };
        }

    }
}
