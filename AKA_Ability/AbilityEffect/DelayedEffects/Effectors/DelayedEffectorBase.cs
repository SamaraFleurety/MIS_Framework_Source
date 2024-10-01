﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.DelayedEffects
{
    public abstract class DelayedEffectorBase : IExposable
    {
        public DelayedEffectDef effectDef;

        public Pawn casterPawn;

        public LocalTargetInfo targetInfo;

        public DelayedEffectorBase(DelayedEffectDef effectDef, Pawn casterPawn, LocalTargetInfo targetInfo)
        {
            this.casterPawn = casterPawn;
            this.effectDef = effectDef;
            this.targetInfo = targetInfo;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref effectDef, "edef");
            Scribe_References.Look(ref casterPawn, "caster");
            Scribe_TargetInfo.Look(ref targetInfo, "lTarget");
        }

        public abstract void DoEffect();
    }
}