using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace LMA_Lib.AI
{
    public class JobDriver_StoreSilverAzur : JobDriver
    {
        private const TargetIndex ConsoleIndex = TargetIndex.A;
        private const TargetIndex SilverIndex = TargetIndex.B;
        private int initialSilver = -1;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref initialSilver, "initialSilver", -1);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.GetTarget(ConsoleIndex), job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedNullOrForbidden(ConsoleIndex);

            yield return Toils_General.Do(delegate
            {
                if (initialSilver < 0)
                {
                    initialSilver = pawn.inventory.Count(ThingDefOf.Silver);
                }
            });
            yield return Toils_Reserve.ReserveQueue(SilverIndex);

            Toil extractSilver = Toils_JobTransforms.ExtractNextTargetFromQueue(SilverIndex);
            yield return extractSilver;
            yield return Toils_Goto.GotoThing(SilverIndex, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(SilverIndex);
            yield return Toils_General.Do(delegate
            {
                Thing silver = job.GetTarget(SilverIndex).Thing;
                int storedForJob = pawn.inventory.Count(ThingDefOf.Silver) - initialSilver;
                int transferCount = Mathf.Min(silver.stackCount, GC_AzurManager.SilverExchangeRate - storedForJob);
                Thing silverToStore = silver.SplitOff(transferCount);
                if (silverToStore.Spawned)
                {
                    silverToStore.DeSpawn();
                }
                if (!pawn.inventory.innerContainer.TryAdd(silverToStore))
                {
                    GenSpawn.Spawn(silverToStore, pawn.Position, pawn.Map);
                    EndJobWith(JobCondition.Incompletable);
                }
            });
            yield return Toils_Jump.JumpIfHaveTargetInQueue(SilverIndex, extractSilver);
            yield return Toils_Goto.GotoThing(ConsoleIndex, PathEndMode.InteractionCell);
            yield return Toils_General.Wait(240, ConsoleIndex).WithProgressBarToilDelay(ConsoleIndex);
            yield return Toils_General.Do(delegate
            {
                if (pawn.inventory.Count(ThingDefOf.Silver) - initialSilver < GC_AzurManager.SilverExchangeRate)
                {
                    EndJobWith(JobCondition.Incompletable);
                    return;
                }

                pawn.inventory.RemoveCount(ThingDefOf.Silver, GC_AzurManager.SilverExchangeRate, false);
                GC_AzurManager.Instance.storedSilver += GC_AzurManager.SilverExchangeRate;
            });
        }
    }
}
