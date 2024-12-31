using System.Collections.Generic;
using RimWorld;
using Verse.AI;
using Verse;
using AK_DLL.UI;

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
                //RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                RIWindowHandler.continuousRecruit = TargetThingB == null ? false : true;    //用target B做标记，要是不为null就是连续招募模式
                UI.RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccMainMenu, TargetThingA, pawn, OpDetailType.Recruit /*RIWindowType.MainMenu,TargetThingA*/);
            };
            yield return t;
            yield break;
        }
    }
}
