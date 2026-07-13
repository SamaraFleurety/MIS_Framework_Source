using AK_DLL.UI;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace LMA_Lib.AI
{
    public class JobDriver_UseGachaConsoleAzur : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);
            yield return Toils_General.Do(delegate
            {
                Find.TickManager.Pause();
                RIWindowHandler.continuousRecruit = TargetThingB != null;
                RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_Gacha, TargetThingA, pawn);
            });
        }
    }
}
