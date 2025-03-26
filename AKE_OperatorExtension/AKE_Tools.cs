using RimWorld;
using Verse;

namespace AKE_OperatorExtension
{
    public static class AKE_Tools
    {
        internal static bool HasSkillTracker(this Pawn p)
        {
            return p != null && p.skills != null;
        }
        internal static bool HasTraitTracker(this Pawn p)
        {
            return p != null && p.story != null && p.story.traits != null;
        }
        internal static bool HasOperatorTraitDef(this Pawn p, string XMLdefName)
        {
            for (int i = 0; i < p.story.traits.allTraits.Count; i++)
            {
                if (p.story.traits.allTraits[i].def.defName == DefDatabase<TraitDef>.GetNamed(XMLdefName).defName)
                {
                    return true;
                }
            }
            return false;
        }
        internal static bool HasStoryTracker(this Pawn p)
        {
            return p != null && p.story != null;
        }
        internal static bool HasAbilityTracker(this Pawn p)
        {
            return p != null && p.abilities != null;
        }
    }
}
