using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public class HealthBarControllerComp : ThingComp
    {
        private Pawn pawn => parent as Pawn;
        private float HealthPercent;
        private static readonly Vector3 BottomMargin = Vector3.back * 1f;
        private static readonly Vector2 BarSize = new Vector2(1.5f, 0.075f);
        private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(105, 180, 210, 180));
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.75f));
        public override void PostDraw()
        {
            base.PostDraw();
            if (AK_ModSettings.displayBarModel)
            {
                if (!pawn.Drafted || pawn.GetDoc() == null)
                {
                    return;
                }
                GenDraw.FillableBarRequest fbr = default;
                fbr.center = pawn.DrawPos + (Vector3.up * 5f) + BottomMargin;
                fbr.size = BarSize;
                HealthPercent = pawn.health?.summaryHealth?.SummaryHealthPercent ?? (-1f);
                fbr.fillPercent = (HealthPercent < 0f) ? 0f : HealthPercent;
                fbr.filledMat = BarFilledMat;
                fbr.unfilledMat = BarUnfilledMat;
                fbr.margin = 0.001f;
                fbr.rotation = Rot4.North;
                GenDraw.DrawFillableBar(fbr);
            }
        }
    }
}
