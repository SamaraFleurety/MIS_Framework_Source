﻿using System;

namespace AKA_Ability.CastConditioner
{
    public class CC_NeedFullCharge : CastConditioner_Base
    {
        [Obsolete]
        public override bool Castable(AKAbility_Base instance)
        {
            return instance.cooldown.Charge >= instance.cooldown.MaxCharge;
        }
    }
}