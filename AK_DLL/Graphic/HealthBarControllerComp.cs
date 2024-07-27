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
        private Pawn pawn;
        private float HealthPercent;
        Vector3 BottomMargin = Vector3.back * 1f;
        private static readonly Vector2 BarSize = new Vector2(1.5f, 0.075f);
        private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(105, 180, 210, 255));
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.75f));
        public override void PostDraw()
        {
            base.PostDraw();
            if (AK_ModSettings.displayBarModel)
            {
                pawn = parent as Pawn;
                if (!pawn.Drafted)
                {
                    return;
                }
                GenDraw.FillableBarRequest fbr = default;
                fbr.center = pawn.DrawPos + (Vector3.up * 5f) + BottomMargin;
                fbr.size = BarSize;
                //填充比例
                HealthPercent = pawn.health?.summaryHealth?.SummaryHealthPercent ?? (-1f);
                fbr.fillPercent = (HealthPercent < 0f) ? 0f : HealthPercent;
                //填充部分的颜色
                fbr.filledMat = BarFilledMat;
                fbr.unfilledMat = BarUnfilledMat;
                //间距
                fbr.margin = 0f;
                fbr.rotation = Rot4.North;
                GenDraw.DrawFillableBar(fbr);
            }
        }
    }
}
