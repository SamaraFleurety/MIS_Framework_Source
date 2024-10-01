﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //传送用通用的
    public class AE_Illustrious_Dilapidate : AbilityEffectBase
    {
        public DamageDef damageDef;
        static List<float> damageOnCharge  = new List<float>() { 0, 1, 2, 50, 100, 225, 550 };

        public HediffDef hediffDef; //下面两个加攻击力用的
        public float buffDuration = 96;
        static List<float> statCDFactorOnCharge = new List<float>() { 0, 0, 0.91f, 0.91f, 0.625f, 0.625f, 0.556f };
        static List<float> statAtkDmgFactorOnCharge = new List<float>() { 0, 0, 1.1f, 1.1f, 1.1f, 1.6f, 1.8f };

        public HediffDef stunDef;
        static List<float> stunTimeOnCharge = new List<float>() { 0, 0, 0, 0, 0, 3, 6 };

        public HediffDef cloakDef;
        static List<float> cloakTimeOnCharge = new List<float>() { 0, 0, 0, 0, 2, 5, 9 };
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            Pawn casterPawn = caster.CasterPawn;
            Pawn targetPawn = target.Pawn;
            int charge = caster.cooldown.charge;
            if (targetPawn != null)
            {
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
                //5充开始眩晕
                if (charge>= 5)
                {
                    AbilityEffect_AddHediff.AddHediff(targetPawn, stunDef, severity: stunTimeOnCharge[charge]);
                }
            }

            //2充能开始加攻击和降低攻击冷却
            if (charge >= 2)
            {
                AbilityEffect_AddHediff.AddHediff(casterPawn, hediffDef, severity: -1000);
                Hediff_DynamicStage atkBonus = AbilityEffect_AddHediff.AddHediff(casterPawn, hediffDef, severity: buffDuration) as Hediff_DynamicStage;
                float cdFactor = statCDFactorOnCharge[charge];
                atkBonus.stageProperty.TryAddMergeStatModifier(StatDefOf.MeleeCooldownFactor, cdFactor);
                atkBonus.stageProperty.TryAddMergeStatModifier(StatDefOf.RangedCooldownFactor, cdFactor);

                float dmgFactor = statAtkDmgFactorOnCharge[charge];
                atkBonus.stageProperty.TryAddMergeStatModifier(StatDefOf.MeleeDamageFactor, dmgFactor);
                atkBonus.stageProperty.TryAddMergeStatModifier(AKADefOf.AKA_Stat_RangedDamageFactor, dmgFactor);
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