using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this.FailOnDespawnedOrNull(indexRecord);
            this.FailOnForbidden(indexRecord);

            this.FailOnDespawnedOrNull(indexPlayer);
            this.FailOnForbidden(indexPlayer);

            yield return Toils_Goto.GotoThing(indexRecord, PathEndMode.Touch);

            //拿起来
            yield return Toils_Haul.StartCarryThing(indexRecord);

            yield return Toils_Goto.GotoThing(indexPlayer, PathEndMode.InteractionCell);

            Toil toilWait = Toils_General.Wait(100);
            toilWait.FailOnCannotTouch(indexPlayer, PathEndMode.InteractionCell);
            toilWait.WithProgressBarToilDelay(indexPlayer);
            yield return toilWait;

            Toil insertRecordIntoPlayer = ToilMaker.MakeToil();
            insertRecordIntoPlayer.initAction = delegate ()
            {
                GC_MusicMuseum.EnableSong(thingRecord.recordedSong);
                thingRecord.Destroy();
            };
            insertRecordIntoPlayer.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return insertRecordIntoPlayer;
        }
    }
}
