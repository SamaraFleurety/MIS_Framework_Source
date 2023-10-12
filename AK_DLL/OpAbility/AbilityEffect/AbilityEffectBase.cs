using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public abstract class AbilityEffectBase
    {
        public virtual void DoEffect_All(Pawn caster, LocalTargetInfo targetInfo)
        {
            Pawn target = targetInfo.Pawn;
            if (target != null) DoEffect_Pawn(caster, target);
            DoEffect_IntVec(targetInfo.Cell, caster.Map, caster);
        }

        public virtual void DoEffect_Pawn(Pawn user, Thing target)
        { 
        
        }
        public virtual void DoEffect_IntVec(IntVec3 target, Map map , Pawn caster = null)
        {
        
        }
    }
}
