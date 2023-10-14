using Verse;
using AKA_Ability;
using RimWorld;

namespace AK_DLL
{
    public class VAbility_Operator : VAbility_AKATrackerContainer
    {
        public AK_AbilityTracker AK_Tracker => AKATracker as AK_AbilityTracker;

        public OperatorDocument Document => AK_Tracker.doc;

        //需要保留这个来使用原版的读档机制
        public VAbility_Operator(Pawn pawn)
            : base(pawn)
        {
        }

        public VAbility_Operator(Pawn pawn, AbilityDef def)
            : base(pawn, def)
        {
        }
    }
}
