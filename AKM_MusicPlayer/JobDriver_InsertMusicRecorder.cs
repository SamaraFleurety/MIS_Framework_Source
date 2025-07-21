using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AKM_MusicPlayer
{
    //把唱片插入留声机
    public class JobDriver_InsertMusicRecorder : JobDriver
    {
        private TargetIndex indexRecord = TargetIndex.A;
        private TargetIndex indexPlayer = TargetIndex.B;

        private ThingClass_MusicRecord thingRecord => job.targetA.Thing as ThingClass_MusicRecord;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (ReservationUtility.Reserve(pawn, job.targetA, job, 1, -1, (ReservationLayerDef)null, errorOnFailed))
            {
                return ReservationUtility.Reserve(pawn, job.targetB, job, 1, -1, (ReservationLayerDef)null, errorOnFailed);
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(indexPlayer);
            this.FailOnForbidden(indexPlayer);


            yield return Toils_General.DoAtomic(delegate ()
            {
                job.count = 1;
            });

            Toil reserveThing = Toils_Reserve.Reserve(indexRecord);
            yield return reserveThing;

            yield return Toils_Goto.GotoThing(indexRecord, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(indexRecord).FailOnSomeonePhysicallyInteracting(indexRecord);

            //拿起来
            yield return Toils_Haul.StartCarryThing(indexRecord, false, true).FailOnDestroyedNullOrForbidden(indexRecord);

            yield return Toils_Goto.GotoThing(indexPlayer, PathEndMode.InteractionCell);

            Toil toilWait = Toils_General.Wait(100);
            toilWait.WithProgressBarToilDelay(indexPlayer);
            toilWait.FailOnCannotTouch(indexPlayer, PathEndMode.Touch);
            yield return toilWait;

            Toil insertRecordIntoPlayer = ToilMaker.MakeToil();
            insertRecordIntoPlayer.initAction = delegate ()
            {
                GC_MusicMuseum.EnableSong(thingRecord.recordedSong);
                thingRecord.Destroy();
                if (pawn.skills != null)
                {
                    pawn.skills.GetSkill(SkillDefOf.Artistic).Learn(ThingClass_MusicRecord.XPGain);
                }
            };
            insertRecordIntoPlayer.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return insertRecordIntoPlayer;
        }
    }
}
