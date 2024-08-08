using UnityEngine;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public class TC_HealthBarController : ThingComp
    {
        private Pawn Pawn => parent as Pawn;
        private float HealthPercent;
        private Vector3 IconMargin => Vector3.back + Vector3.left * 0.8f;
        private static Vector3 BottomMargin => Vector3.back;
        private static Vector2 BarSize = new Vector2(1.5f, 0.075f);
        private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(105, 180, 210, 180));
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.75f));
        private Material HP_Icon;
        private void GenHPIcon()
        {
            if (HP_Icon == null)
            {
                HP_Icon = AK_BarUITool.HP_Icon;
            }
        }
        public override void PostDraw()
        {
            base.PostDraw();
            if (!AK_ModSettings.displayBarModel || Pawn.GetDoc() == null || !Pawn.Drafted)
            {
                return;
            }
            GenDraw.FillableBarRequest fbr = default;
            fbr.center = Pawn.DrawPos + (Vector3.up * 3f) + BottomMargin;
            fbr.size = BarSize;
            HealthPercent = Pawn.health?.summaryHealth?.SummaryHealthPercent ?? (-1f);
            fbr.fillPercent = (HealthPercent < 0f) ? 0f : HealthPercent;
            fbr.filledMat = BarFilledMat;
            fbr.unfilledMat = BarUnfilledMat;
            fbr.margin = 0.001f;
            fbr.rotation = Rot4.North;
            GenDraw.DrawFillableBar(fbr);
            GenHPIcon();
            Matrix4x4 matrix = default;
            Vector3 scale = new Vector3(0.25f, 1f, 0.25f);
            matrix.SetTRS(Pawn.DrawPos + IconMargin, Rot4.North.AsQuat, scale);
            Graphics.DrawMesh(MeshPool.plane025, matrix, material: HP_Icon, 2);
        }
    }
}
