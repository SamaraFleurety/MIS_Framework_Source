using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.CastConditioner
{
    public class CC_SummonLimit : CastConditioner_Base
    {
        public override bool Castable(AKAbility instance)
        {
            AKAbility_Summon ability = instance as AKAbility_Summon;

            if (ability == null) { Log.Error($"[AK] {instance.def.label}非召唤技能但使用了{this.GetType()}"); }

            return ability.summoneds.Count < ability.Effector.existLimits;
        }
    }
}
