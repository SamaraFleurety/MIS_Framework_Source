using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.CastConditioner
{
    public class CC_NeedFullCharge : CastConditioner_Base
    {
        public override bool Castable(AKAbility_Base instance)
        {
            return instance.cooldown.Charge >= instance.cooldown.MaxCharge;
        }
    }
}