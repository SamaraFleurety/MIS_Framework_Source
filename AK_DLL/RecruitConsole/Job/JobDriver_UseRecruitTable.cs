using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;
using Verse;

namespace AK_DLL
{
    public class JobDriver_UseRecruitConsole : JobDriver
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
                RIWindowHandler.OpenRIWindow(RIWindow.MainMenu,TargetThingA);
            };
            yield return t;
            yield break;
        }
    }
}
