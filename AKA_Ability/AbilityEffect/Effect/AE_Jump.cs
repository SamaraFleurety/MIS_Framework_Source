using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AE_Jump : AbilityEffectBase
    {
        public VerbProperties verbJump;
        public override void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster = null)
        {
            JumpUtility.DoJump(caster, target, null, verbJump);
        }
    }
}
