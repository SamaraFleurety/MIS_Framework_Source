using RimWorld;
using RimWorld.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace AKA_Ability.AI
{
    //没写完,准备换皮
    /*public class JobGiver_ReloadCDTrackerShared : ThinkNode_JobGiver
    {
        private const bool ForceReloadWhenLookingForWork = false;

        public override float GetPriority(Pawn pawn)
        {
            return 5.9f;
        }
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return null;
            }
            IReloadableComp reloadableComp = ReloadableUtility.FindSomeReloadableComponent(pawn, allowForcedReload: false);
            if (reloadableComp == null)
            {
                return null;
            }
            if (pawn.carryTracker.AvailableStackSpace(reloadableComp.AmmoDef) < reloadableComp.MinAmmoNeeded(allowForcedReload: true))
            {
                return null;
            }
            List<Thing> list = ReloadableUtility.FindEnoughAmmo(pawn, pawn.Position, reloadableComp, forceReload: false);
            if (list.NullOrEmpty())
            {
                return null;
            }
            return MakeReloadJob(reloadableComp, list);
        }

        public static Job MakeReloadJob(IReloadableComp reloadable, List<Thing> chosenAmmo)
        {
            //应该在AKA里写个新的JobDef
             <JobDef>
                 <defName>AKA_ReloadCDTrackerShared</defName>
                 <driverClass>JobDriver_ReloadCDTrackerShared</driverClass>
                 <reportString>reloading TargetA.</reportString>
                 <suspendable>false</suspendable>
                 <allowOpportunisticPrefix>true</allowOpportunisticPrefix>
             </JobDef>
             
            Job job = JobMaker.MakeJob(AKADefOf.AKA_ReloadCDTrackerShared, reloadable.ReloadableThing);
            job.targetQueueB = chosenAmmo.Select((t) => new LocalTargetInfo(t)).ToList();
            job.count = chosenAmmo.Sum((t) => t.stackCount);
            job.count = Math.Min(job.count, reloadable.MaxAmmoNeeded(allowForcedReload: true));
            return job;
        }
    }*/
}
