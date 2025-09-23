using RimWorld.Planet;
using System.Linq;
using Verse;

namespace AKA_Ability.DelayedEffects
{
    //考虑以后改成直接使用ability effect base而不是每个都要重开一套。
    public class DelayedEffectorBase : IExposable
    {
        //public DelayedEffectDef effectDef;
        protected bool cachedCastableCondition = false;

        public Pawn CasterPawn => sourceAbility.CasterPawn;

        public LocalTargetInfo localTarget;
        public GlobalTargetInfo globalTarget;

        public AbilityEffectsDef effectDef;

        public AKAbility_Base sourceAbility;

        public DelayedEffectorBase(AbilityEffectsDef effectDef, AKAbility_Base sourceAbility, LocalTargetInfo targetInfo = default, GlobalTargetInfo globalTarget = default)
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

        //判定是否能发动技能
        public virtual bool CastableNow
        {
            get
            {
                cachedCastableCondition = effectDef.castConditions == null || effectDef.castConditions.All(condition => condition.Castable(sourceAbility));
                return cachedCastableCondition;
            }
        }

        public virtual void TryDoEffect()
        {
            if (CastableNow == false) return;

            foreach (AbilityEffectBase effect in effectDef.compEffectList)
            {
                effect.DoEffect(sourceAbility, globalTarget, localTarget);
            }
        }
    }
}
