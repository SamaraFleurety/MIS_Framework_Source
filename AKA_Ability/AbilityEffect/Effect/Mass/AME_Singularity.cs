using Verse;

namespace AKA_Ability.AbilityEffect
{
    //把周围的人吸入灵璧
    public class AME_Singularity : AbilityMassEffectBase
    {
        public int stunTick = 1; //会导致被吸引之后的人被眩晕

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            AME_Worker<Pawn> worker = new AME_Worker<Pawn>(this, doEffect_SingleTarget: delegate (AKAbility_Base ab, Pawn victim)
            {
                if (victim.DestroyedOrNull()) return;

                //推拉的终点位置是，从人往技能点看，最远的可站立点
                victim.Position = GenSight.LastPointOnLineOfSight(victim.Position, target.Cell, delegate (IntVec3 cell)
                {
                    return cell.Standable(caster.CasterPawn.Map);
                }); 

                victim.pather.StopDead();
                victim.jobs.StopAll();
                if (stunTick > 0) victim.stances.stunner.StunFor(stunTick, caster.CasterPawn);
            }, AllPawnAliveInCell);
            worker.DoEffect_AllTargets(caster, target);
            return true;
        }

    }
}
