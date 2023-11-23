using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AKA_Ability
{
    //既是property，也是效果器
    public abstract class AbilityEffectBase
    {

        public virtual void DoEffect_All(Pawn caster, LocalTargetInfo targetInfo, bool delayed = false)
        {
            Pawn target = targetInfo.Pawn;
            if (target != null) DoEffect_Pawn(caster, target, delayed);
            if (targetInfo.Cell.InBounds(caster.Map)) DoEffect_IntVec(targetInfo.Cell, caster.Map, delayed, caster);
        }

        public virtual void DoEffect_All(Pawn caster, Thing target, IntVec3? cell, Map map, bool delayed = false)
        {
            if (target != null) DoEffect_Pawn(caster, target, delayed);
            if (cell is IntVec3 vec && vec.InBounds(map)) DoEffect_IntVec(vec, map, delayed, caster);
        }

        public virtual void DoEffect_Pawn(Pawn user, Thing target, bool delayed)
        {

        }
        public virtual void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster = null)
        {

        }
    }
}
