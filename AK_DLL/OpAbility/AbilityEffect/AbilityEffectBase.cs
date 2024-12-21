using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    /*public abstract class AbilityEffectBase
    {
        public virtual void DoEffect_All(Pawn caster, LocalTargetInfo targetInfo)
        {
            Pawn target = targetInfo.Pawn;
            if (target != null) DoEffect_Pawn(caster, target);
            if (targetInfo.Cell.InBounds(caster.Map)) DoEffect_IntVec(targetInfo.Cell, caster.Map, caster);
        }

        public virtual void DoEffect_All(Pawn caster, Thing target, IntVec3 cell, Map map)
        {
            if (target != null) DoEffect_Pawn(caster, target);
            if (cell != null && cell.InBounds(map)) DoEffect_IntVec(cell, map, caster);
        }

        public virtual void DoEffect_Pawn(Pawn user, Thing target)
        { 
        
        }
        public virtual void DoEffect_IntVec(IntVec3 target, Map map , Pawn caster = null)
        {
        
        }
    }*/
}
