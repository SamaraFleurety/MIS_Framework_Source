using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace AK_DLL
{
    /*public class JobDriver_TeleportToRecruitTower : JobDriver
    {
        TargetIndex indexOrigin = TargetIndex.A;
        TargetIndex indexDestination = TargetIndex.B;

        Thing thingDestination => job.targetB.Thing;
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);
            Toil t = new Toil();
            t.initAction = delegate
            {
                pawn.DeSpawn(DestroyMode.QuestLogic);
                GenSpawn.Spawn(pawn, thingDestination.InteractionCell, thingDestination.Map);
                CameraJumper.TryJump(new GlobalTargetInfo(pawn.Position, pawn.Map));
            };
            t.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return t;
        }
    }*/
}
