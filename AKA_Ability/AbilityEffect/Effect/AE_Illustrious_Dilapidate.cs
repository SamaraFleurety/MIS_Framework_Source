using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //传送用通用的
    public class AE_Illustrious_Dilapidate : AbilityEffectBase
    {
        public DamageDef damageDef;
        static List<float> damageOnCharge  = new List<float>() { 0, 1, 2, 50, 100, 225, 550 };

        public HediffDef hediffDef; //下面两个加攻击力用的 必须是Hediff_DynamicStage
        public float buffDuration = 96; //hour
        static List<float> statCDFactorOnCharge = new List<float>() { 0, 0, 0.91f, 0.91f, 0.625f, 0.625f, 0.556f };
        static List<float> statAtkDmgFactorOnCharge = new List<float>() { 0, 0, 0.1f, 0.1f, 0.1f, 0.6f, 0.8f };

        //public HediffDef stunDef;
        static List<float> stunTimeOnCharge = new List<float>() { 0, 0, 0, 0, 0, 3, 6 };  //second

        public HediffDef cloakDef;
        static List<float> cloakTimeOnCharge = new List<float>() { 0, 0, 0, 0, 2, 5, 9 };
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.container == null) return false;

            Pawn casterPawn = caster.CasterPawn;
            int charge = caster.cooldown.charge;

            if (target.Thing != null)
            {
                Pawn targetPawn = target.Pawn;
                if (targetPawn != null)
                {
                    //5充开始眩晕
                    if (charge >= 5)
                    {
                        StunHandler stunHandler = targetPawn.stances?.stunner;
                        stunHandler?.StunFor((int)(stunTimeOnCharge[charge] * 60), casterPawn);
                        //AbilityEffect_AddHediff.AddHediff(targetPawn, stunDef, severity: stunTimeOnCharge[charge]);
                    }
                    //3充开始对目标有伤害
                    if (charge >= 3)
                    {
                        float damage = damageOnCharge[charge] / 5;
                        for (int i = 0; i < 5; ++i)
                        {
                            DamageInfo absDmgInfo = new DamageInfo(damageDef, damage, 999, instigator: casterPawn, instigatorGuilty: true);
                            absDmgInfo.Def.Worker.Apply(absDmgInfo, targetPawn);
                        }
                    }
                }
                else if (target.Thing.def.useHitPoints && charge >= 3)
                {
                    target.Thing.HitPoints -= (int)damageOnCharge[charge];
                    if (target.Thing.HitPoints <= 0) target.Thing.Kill();
                }
            }


            //2充能开始加攻击和降低攻击冷却
            if (charge >= 2)
            {
                AbilityEffect_AddHediff.AddHediff(casterPawn, hediffDef, severity: -1000);
                Hediff_DynamicStage atkBonus = AbilityEffect_AddHediff.AddHediff(casterPawn, hediffDef, severity: buffDuration) as Hediff_DynamicStage;
                float cdFactor = statCDFactorOnCharge[charge];
                atkBonus.stageProperty.TryAddMergeStatModifier(StatDefOf.MeleeCooldownFactor, cdFactor, false);
                atkBonus.stageProperty.TryAddMergeStatModifier(StatDefOf.RangedCooldownFactor, cdFactor, false);

                float dmgFactor = statAtkDmgFactorOnCharge[charge];
                atkBonus.stageProperty.TryAddMergeStatModifier(StatDefOf.MeleeDamageFactor, dmgFactor);
                atkBonus.stageProperty.TryAddMergeStatModifier(AKADefOf.AKA_Stat_RangedDamageFactor, dmgFactor);

                atkBonus.ForceRefreshStage();
            }

            //4充开始隐身
            if (charge >= 4)
            {
                AbilityEffect_AddHediff.AddHediff(casterPawn, cloakDef, severity: cloakTimeOnCharge[charge]);
            }

            return base.DoEffect(caster, target);
        }

    }
}
