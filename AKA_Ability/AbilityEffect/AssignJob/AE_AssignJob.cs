using Verse;

namespace AKA_Ability.AbilityEffect
{
    //给技能生效的目标直接指定一个job。可能技能实际效果是走过去近战之类。
    public enum JobAssignOption
    {
        caster,
        target
    }

    public class AE_AssignJob : AbilityEffectBase
    {
        public JobDef assignedJob;
        public JobAssignOption assignTarget = JobAssignOption.caster;

        public bool invertTargetIndex = false;  //逆转job的taget index，可能会导致配套的JobDriver_AbilityAssignedJob逻辑失效 慎用
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn == null) return false;

            Pawn assignedPawn;
            if (assignTarget == JobAssignOption.caster) assignedPawn = caster.CasterPawn;
            else assignedPawn = target.Pawn;
            if (assignedPawn == null || assignedPawn.Destroyed || assignedPawn.DeadOrDowned || assignedPawn.jobs == null)
            {
                return false;
            }
            if (!invertTargetIndex) assignedPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(assignedJob, caster.CasterPawn, target));
            else assignedPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(assignedJob, target, caster.CasterPawn));
            return base.DoEffect(caster, target);
        }
    }
}