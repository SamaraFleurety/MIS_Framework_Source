using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.ActiveConditioner
{
    public abstract class ActiveConditioner_Base
    {
        public abstract bool ShouldActiveNow(AKAbility_Base instance);
    }
}
