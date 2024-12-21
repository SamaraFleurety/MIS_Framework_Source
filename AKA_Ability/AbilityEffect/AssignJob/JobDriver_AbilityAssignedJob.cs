using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AKA_Ability.AI
{
    public abstract class JobDriver_AbilityAssignedJob : JobDriver
    {
        protected TargetIndex indexAbilityCaster => TargetIndex.A;
        protected TargetIndex indexAbilityTarget => TargetIndex.B;
    }
}
