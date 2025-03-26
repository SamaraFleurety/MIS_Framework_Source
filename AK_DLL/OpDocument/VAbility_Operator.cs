using AKA_Ability;
using RimWorld;
using Verse;

namespace AK_DLL
{
    //放在原版的技能上的 干员额外数据的容器
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
