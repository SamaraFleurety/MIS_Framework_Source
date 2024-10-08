﻿using System.Collections.Generic;

namespace AKA_Ability.Range
{
    //这玩意的range甚至要写个class
    public class RangeWorker_Illustrious_Dilapidate : RangeWorker_Base
    {
        static List<float> RANGE_BY_CHARGE = new List<float>() { 20, 25, 25, 25, 40, 60, 60 };
        public RangeWorker_Illustrious_Dilapidate(AKAbility parent) : base(parent)
        {
        }

        public override float Range()
        {
            return RANGE_BY_CHARGE[parent.cooldown.charge];
        }
    }
}
