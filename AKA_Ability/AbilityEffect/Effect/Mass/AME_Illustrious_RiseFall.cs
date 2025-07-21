using Verse;

namespace AKA_Ability.AbilityEffect
{
    //黑光辉 星起星落
    //对附近敌人造成伤害并治疗自身
    public class AME_Illustrious_RiseFall : AbilityMassEffectBase
    {
        DamageDef damageDef = null;
        const float DAMAGE_HEAL_PER_PAWN = 20;
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn == null) return false;

            AME_Worker<Pawn> worker = new AME_Worker<Pawn>(this, doEffect_SingleTarget: delegate (AKAbility_Base ab, Pawn victim)
            {
                if (victim.Destroyed) return;

                if (victim.GetUniqueLoadID() == ab.CasterPawn.GetUniqueLoadID()) return;

                DamageInfo absDmgInfo = new DamageInfo(damageDef, DAMAGE_HEAL_PER_PAWN, 999, instigator: ab.CasterPawn, instigatorGuilty: false);
                if (victim.def.useHitPoints)
                {
                    victim.HitPoints -= (int)DAMAGE_HEAL_PER_PAWN;
                    if (victim.HitPoints <= 0)
                    {
                        victim.HitPoints = 0;
                        victim.Kill(absDmgInfo);
                    }
                }
                else absDmgInfo.Def.Worker.Apply(absDmgInfo, victim);

                AbilityEffect_Heal.Heal(caster.CasterPawn, DAMAGE_HEAL_PER_PAWN);

            }, AllPawnAliveInCell);
            worker.DoEffect_AllTargets(caster, target);
            return true;
        }
    }
}