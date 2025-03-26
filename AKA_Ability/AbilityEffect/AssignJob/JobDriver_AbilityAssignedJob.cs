using Verse.AI;

namespace AKA_Ability.AI
{
    public abstract class JobDriver_AbilityAssignedJob : JobDriver
    {
        protected TargetIndex indexAbilityCaster => TargetIndex.A;
        protected TargetIndex indexAbilityTarget => TargetIndex.B;
    }
}
