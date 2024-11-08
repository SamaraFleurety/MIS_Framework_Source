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
        public HediffDef hediffOnSummon;
        public float duration = 96; //持续时间 单位是游戏内小时

        int enemyCnt = 0;

        const float Mov_Spd_Per_Enemy = 0.1f;
        const float Dmg_Bonus_Per_Enemy = 0.1f;
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            enemyCnt = 0;
            //改成直接作用全图了
            foreach (Pawn p in caster.CasterPawn.Map.mapPawns.AllHumanlikeSpawned)
            {
                DoEffect_SinglePawn(caster, p);
            }

            /*AME_Worker<Pawn> worker = new AME_Worker<Pawn>(this, doEffect_SingleTarget: delegate (AKAbility ab, Pawn victim)
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
            worker.DoEffect_AllTargets(caster, target);*/

            //吸移速最多20层
            Log.Message($"illu inory with cnt {enemyCnt}");
            int enemyCnt_MoveSpd = enemyCnt;
            if (enemyCnt_MoveSpd > 20) enemyCnt_MoveSpd = 20;
            AbilityEffect_AddHediff.AddHediff(caster.CasterPawn, hediffOnSelf, severity: -100);
            Hediff_DynamicStage onSelf = AbilityEffect_AddHediff.AddHediff(caster.CasterPawn, hediffOnSelf, severity: duration) as Hediff_DynamicStage;
            onSelf.stageProperty.TryAddMergeStatModifier(StatDefOf.MoveSpeed, Mov_Spd_Per_Enemy * enemyCnt_MoveSpd, false);

            //增伤最多10层
            int enemyCnt_Dmg = enemyCnt;
            if (enemyCnt_Dmg > 10) enemyCnt_Dmg = 10;
            onSelf.stageProperty.TryAddMergeStatModifier(StatDefOf.MeleeDamageFactor, Dmg_Bonus_Per_Enemy * enemyCnt_Dmg, false);
            onSelf.stageProperty.TryAddMergeStatModifier(AKADefOf.AKA_Stat_RangedDamageFactor, Dmg_Bonus_Per_Enemy * enemyCnt_Dmg, false);

            onSelf.ForceRefreshStage();

            //召唤物享受一半效果
            foreach (Thing summon in caster.container.AllSummoneds())
            {
                if (summon is Pawn summonedPawn)
                {
                    AbilityEffect_AddHediff.AddHediff(summonedPawn, hediffOnSummon, severity: -100);
                    Hediff_DynamicStage onSummon = AbilityEffect_AddHediff.AddHediff(summonedPawn, hediffOnSummon, severity: duration) as Hediff_DynamicStage;

                    onSummon.stageProperty.TryAddMergeStatModifier(StatDefOf.MoveSpeed, Mov_Spd_Per_Enemy * enemyCnt_MoveSpd / 2, false);

                    onSummon.stageProperty.TryAddMergeStatModifier(StatDefOf.MeleeDamageFactor, Dmg_Bonus_Per_Enemy * enemyCnt_Dmg / 2, false);
                    onSummon.stageProperty.TryAddMergeStatModifier(AKADefOf.AKA_Stat_RangedDamageFactor, Dmg_Bonus_Per_Enemy * enemyCnt_Dmg / 2, false);
                }
            }

            return true;
        }

        private void DoEffect_SinglePawn(AKAbility ab, Pawn victim)
        {
            if (victim.Destroyed) return;
            if (victim.GetUniqueLoadID() == ab.CasterPawn.GetUniqueLoadID()) return;
            if (victim.Faction != null && victim.Faction.HostileTo(Faction.OfPlayer))
            {
                ++enemyCnt;
                AbilityEffect_AddHediff.AddHediff(victim, hediffOnEnemy, severity: duration);
            }
            else
            {
                AbilityEffect_AddHediff.AddHediff(victim, hediffOnFriend, severity: duration);
            }
        }
    }
}
