using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.CastConditioner
{
    public class CC_Drafted : CastConditioner_Base
    {
        public override bool Castable(AKAbility instance)
        {
            return instance.CasterPawn.Drafted;
        }
    }
}
