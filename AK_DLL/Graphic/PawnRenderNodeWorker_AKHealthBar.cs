using Verse;
using UnityEngine;
using RimWorld;

namespace AK_DLL
{
    //泰南我草你妈 Draw方法是交替执行的 Worker还是唯一实例
    public class PawnRenderNodeWorker_AKHealthBar : PawnRenderNodeWorker
    {
        //locOffset
        private static Vector2 BarSize = new Vector2(1.5f, 0.075f);
        private static Vector3 BottomMargin = Vector3.back;
        private static Vector3 IconMargin = Vector3.back + Vector3.left * 0.8f;
        //Mat
        private static Material BarFilledMat => AK_BarUITool.HealthBarFilledMat;
        private static Material BarEnemyFilledMat => AK_BarUITool.EnemyHealthBarFilledMat;
        private static Material BarUnfilledMat => AK_BarUITool.BarUnfilledMat;
        private static Material HP_Icon => AK_BarUITool.HP_Icon;

        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            return AK_ModSettings.displayBarModel;
        }
        public override void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
        {
            Pawn pawn = parms.pawn;
            if (AK_ModSettings.displayEnemyBarModel || pawn.HostileTo(Faction.OfPlayer) || !pawn.Dead)
            {
                float eHealthPercent = pawn.health?.summaryHealth?.SummaryHealthPercent ?? (-1f);
                GenDraw.FillableBarRequest efbr = default;
                efbr.center = pawn.DrawPos + (Vector3.up * 3f) + BottomMargin;
                efbr.size = BarSize;
                efbr.fillPercent = (eHealthPercent < 0f) ? 0f : eHealthPercent;
                efbr.filledMat = BarEnemyFilledMat;
                efbr.unfilledMat = BarUnfilledMat;
                efbr.rotation = Rot4.North;
                GenDraw.DrawFillableBar(efbr);
                return;
            }
            if (pawn.GetDoc() == null || !pawn.Drafted)
            {
                return;
            }
            float HealthPercent = pawn.health?.summaryHealth?.SummaryHealthPercent ?? (-1f);
            GenDraw.FillableBarRequest fbr = default;
            fbr.center = pawn.DrawPos + (Vector3.up * 3f) + BottomMargin;
            fbr.size = BarSize;
            fbr.fillPercent = (HealthPercent < 0f) ? 0f : HealthPercent;
            fbr.filledMat = BarFilledMat;
            fbr.unfilledMat = BarUnfilledMat;
            //fbr.margin = 0;
            fbr.rotation = Rot4.North;
            GenDraw.DrawFillableBar(fbr);
            //图标
            Log.Message("当前地图缩放数值" + Find.CameraDriver.ZoomRootSize.ToString());
            Matrix4x4 matrix1 = default;
            matrix1.SetTRS(pawn.DrawPos + IconMargin, Rot4.North.AsQuat, new Vector3(0.25f, 1f, 0.25f));
            Graphics.DrawMesh(MeshPool.plane025, matrix1, material: HP_Icon, 2);
        }
    }
}
