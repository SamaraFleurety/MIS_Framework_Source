using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.DelayedEffects
{
    //考虑以后改成直接使用ability effect base而不是每个都要重开一套。
    public class DelayedEffectorBase : IExposable
    {
        //public DelayedEffectDef effectDef;

        public Pawn CasterPawn => sourceAbility.CasterPawn;

        public LocalTargetInfo localTarget;
        public GlobalTargetInfo globalTarget;

        public AbilityEffectsDef effectDef;

        public AKAbility_Base sourceAbility;

        public DelayedEffectorBase(AbilityEffectsDef effectDef, AKAbility_Base sourceAbility, LocalTargetInfo targetInfo = default(LocalTargetInfo), GlobalTargetInfo globalTarget = default(GlobalTargetInfo))
        {
            //this.casterPawn = casterPawn;
            this.sourceAbility = sourceAbility;
            this.effectDef = effectDef;
            this.localTarget = targetInfo;
            this.globalTarget = globalTarget;
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref effectDef, "edef");
            //Scribe_References.Look(ref casterPawn, "caster");
            Scribe_References.Look(ref sourceAbility, "sourceAB");
            Scribe_TargetInfo.Look(ref localTarget, "lTarget");
            Scribe_TargetInfo.Look(ref globalTarget, "gTarget");
        }

        public virtual void DoEffect()
        {
            if (sourceAbility == null) return;

            foreach (AbilityEffectBase effect in effectDef.compEffectList)
            {
                effect.DoEffect(sourceAbility, globalTarget, localTarget);
            }
        }
    }
}
