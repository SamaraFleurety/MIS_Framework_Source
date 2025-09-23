using AK_DLL;
using AK_DLL.UI;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace LMA_Lib.AI
{
    public class JobDriver_UseRecruitConsoleAzur : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);
            Toil t = new()
            {
                initAction = delegate
            {
                Find.TickManager.Pause();
                RIWindowHandler.continuousRecruit = TargetThingB != null;    //用target B做标记，要是不为null就是连续招募模式
                RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_MainMenu, TargetThingA, pawn, OpDetailType.Recruit);
            }
            };
            yield return t;
            yield break;
        }
    }
}
