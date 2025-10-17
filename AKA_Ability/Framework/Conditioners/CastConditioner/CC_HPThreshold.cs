using System;

namespace AKA_Ability.CastConditioner
{
    public class CC_HPThreshold : CastConditioner_Base
    {
        public float HPRatio = 1;
        public bool lower = true;

        public CC_HPThreshold()
        {
            failReason = "AKA_HPThreshold";
        }

        public override bool Castable(AKAbility_Base instance)
        {
            float pawnHPRatio = instance.CasterPawn.health?.summaryHealth?.SummaryHealthPercent ?? 0;
            bool flag = pawnHPRatio <= HPRatio;
            if (!lower) flag = !flag;
            return flag;
        }
    }
}
