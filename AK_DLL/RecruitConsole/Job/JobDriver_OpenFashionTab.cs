using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;
using Verse;

namespace AK_DLL
{
    //fixme:干员换装 可能没写完。 逻辑是让可能换装的干员自己操作通讯台换装。
    public class JobDriver_OpenFashionTab : JobDriver
    {
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
                Find.TickManager.Pause();
                RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                RIWindowHandler.OpenRIWindow_OpDetail(pawn, TargetThingA);
            };
            yield return t;
            yield break;
        }
    }
}
