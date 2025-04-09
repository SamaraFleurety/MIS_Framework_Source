using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //带有此接口的AE，会判断目标是否合法，并影响是否可以释放
    public interface ITargetingValidator
    {
        public bool TargetingValidator(TargetInfo info);
    }
}
