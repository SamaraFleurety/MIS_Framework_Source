using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class AbilityEffect_ReclaimSummoned : AbilityEffectBase
    {
        public void Reclaim(Pawn target,CompAbility comp) 
        {
            if (!target.Dead&&!target.Destroyed) 
            {
                target.Destroy();
                comp.SummonedDead();
            }
        }
    }
}
