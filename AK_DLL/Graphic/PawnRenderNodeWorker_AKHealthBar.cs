using Verse;
using UnityEngine;

namespace AK_DLL
{
    //节点工作器
    public class PawnRenderNodeWorker_AKHealthBar : PawnRenderNodeWorker
    {
        private float HealthPercent;
        //locOffset
        private static Vector2 BarSize = new Vector2(1.5f, 0.075f);
        private static Vector3 BottomMargin = Vector3.back;
        private static Vector3 IconMargin = Vector3.back + Vector3.left * 0.8f;
        //材质
        private static Material BarFilledMat => AK_BarUITool.HealthBarFilledMat;
        private static Material BarUnfilledMat => AK_BarUITool.BarUnfilledMat;
        private static Material HP_Icon => AK_BarUITool.HP_Icon;

        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            return AK_ModSettings.displayBarModel;
        }
        public override void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
        {
            if (parms.pawn.GetDoc() == null || !parms.pawn.Drafted)
            {
                return;
            }
            GenDraw.FillableBarRequest fbr = default;
            fbr.center = parms.pawn.DrawPos + (Vector3.up * 3f) + BottomMargin;
            fbr.size = BarSize;
            HealthPercent = parms.pawn.health?.summaryHealth?.SummaryHealthPercent ?? (-1f);
            fbr.fillPercent = (HealthPercent < 0f) ? 0f : HealthPercent;
            fbr.filledMat = BarFilledMat;
            fbr.unfilledMat = BarUnfilledMat;
            //fbr.margin = 0;
            fbr.rotation = Rot4.North;
            GenDraw.DrawFillableBar(fbr);
            //检查
            Matrix4x4 matrix1 = default;
            matrix1.SetTRS(parms.pawn.DrawPos + IconMargin, Rot4.North.AsQuat, new Vector3(0.25f, 1f, 0.25f));
            Graphics.DrawMesh(MeshPool.plane025, matrix1, material: HP_Icon, 2);
        }
    }
}
