using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //黑光辉交织的祈愿
    //光辉以自身为圆心吸取范围内敌人的移动速度和伤害，自身获得一定量加成，并获得生命吸取。范围内队友会承受一定效果并且不提供加成。
    public class AME_Illustrious_Inory : AbilityMassEffectBase
    {
        public HediffDef hediffOnFriend;
        public HediffDef hediffOnEnemy;
        public HediffDef hediffOnSelf;
        public float duration = 96; //持续时间 单位是游戏内小时
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            int enemyCnt = 0;
            AME_Worker<Pawn> worker = new AME_Worker<Pawn>(this, doEffect_SingleTarget: delegate (AKAbility ab, Pawn victim)
            {
                if (victim.Destroyed) return;
                if (victim.Faction != null && victim.Faction.HostileTo(Faction.OfPlayer))
                {
                    ++enemyCnt;
                    AbilityEffect_AddHediff.AddHediff(victim, hediffOnEnemy, severity: duration);
                }
                else
                {
                    AbilityEffect_AddHediff.AddHediff(victim, hediffOnFriend, severity: duration);
                }

            }, AllPawnAliveInCell);
            worker.DoEffect_AllTargets(caster, target);

            //吸移速最多20层
            if (enemyCnt > 20) enemyCnt = 20;
            AbilityEffect_AddHediff.AddHediff(caster.CasterPawn, hediffOnSelf, severity: -100);
            Hediff_DynamicStage onSelf = AbilityEffect_AddHediff.AddHediff(caster.CasterPawn, hediffOnSelf, severity: duration) as Hediff_DynamicStage;
            onSelf.stageProperty.TryAddMergeStatModifier(StatDefOf.MoveSpeed, 0.1f * enemyCnt, false);

            //增伤最多10层
            if (enemyCnt > 10) enemyCnt = 10;
            onSelf.stageProperty.TryAddMergeStatModifier(StatDefOf.MeleeDamageFactor, 0.1f * enemyCnt, false);
            onSelf.stageProperty.TryAddMergeStatModifier(AKADefOf.AKA_Stat_RangedDamageFactor, 0.1f * enemyCnt, false);

            return true;
        }
    }
}
