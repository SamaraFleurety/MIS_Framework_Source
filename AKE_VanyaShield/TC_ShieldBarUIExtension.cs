using AK_DLL;
using RimWorld;
using UnityEngine;
using VanyaMod;
using Verse;

namespace AKE_VanyaShield
{
    public class TCP_ShieldBarUIExtension : CompProperties
    {
        public TCP_ShieldBarUIExtension()
        {
            compClass = typeof(TC_ShieldBarUIExtension);
        }
    }
    public class TC_ShieldBarUIExtension : ThingComp
    {
        TCP_ShieldBarUIExtension Props => (TCP_ShieldBarUIExtension)props;
        protected Vanya_ShieldBelt Parent => (Vanya_ShieldBelt)parent;
        protected Pawn Wearer => Parent.Wearer;
        private static bool CameraPlusModEnabled => AK_BarUITool.CameraPlusModEnabled;
        private static bool SimpleCameraModEnabled => AK_BarUITool.SimpleCameraModEnabled;
        private float ZoomRootSize => Find.CameraDriver.ZoomRootSize;
        private float SheildPercent;
        private static float Width => AK_ModSettings.barWidth * 0.01f;
        private static float Height => AK_ModSettings.barHeight * 0.001f;
        private static float Margin => AK_ModSettings.barMargin * 0.01f;
        private static Vector2 BarSize => new Vector2(Width, Height);
        private static Vector3 BottomMargin => new Vector3(0f, 0f, Margin - Height * 2);

        private Material BarFilledMat;
        private Material BarUnfilledMat;
        private Material Def_Icon;
        private void GenDefendIcon()
        {
            if (Def_Icon == null)
            {
                Def_Icon = AK_BarUITool.DEF_Icon;
            }
        }
        private float GetZoomRatio()
        {
            if (AK_ModSettings.zoomWithCamera)
            {
                return Mathf.Max(ZoomRootSize, 11) / 11;
            }
            return 1f;
        }
        public override void CompDrawWornExtras()
        {
            /*if (!AK_ModSettings.displayBarModel || ModLister.GetActiveModWithIdentifier("Mlie.VanyaShield") == null) return;*/
            if (Wearer == null || !Wearer.Drafted) return;
            if (BarFilledMat == null || BarUnfilledMat == null)
            {
                BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(245, 245, 245, 190));
                BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.50f));
            }
            GenDefendIcon();
            DrawShieldBar();
        }
        private void DrawShieldBar()
        {
            Vector3 drawPos = Wearer.DrawPos;
            float zoomRatio = GetZoomRatio();
            float zoomWidthRatio;
            float zoomYRatio;
            if (CameraPlusModEnabled || SimpleCameraModEnabled)
            {
                zoomWidthRatio = zoomRatio > 4.35f ? 4.35f : zoomRatio;
                zoomYRatio = zoomRatio > 5f ? 5f : zoomRatio;
            }
            else
            {
                zoomWidthRatio = zoomRatio > 3.75f ? 3.75f : zoomRatio;
                zoomYRatio = zoomRatio > 3f ? 3f : zoomRatio;
            }
            GenDraw.FillableBarRequest fbr = default;
            if (CameraPlusModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.9f : zoomYRatio);
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 1.75f ? zoomRatio * 1.5f : zoomRatio;
            }
            else if (SimpleCameraModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio);
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 3f ? zoomRatio * 1.05f : zoomRatio;
            }
            else
            {
                fbr.center = drawPos + (Vector3.up * 3f) + BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio);
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 6.5f ? zoomRatio * 1.25f : zoomRatio;
            }
            //fbr.center = Wearer.DrawPos + (Vector3.up * 3f) + BottomMargin;
            //fbr.size = BarSize;
            fbr.filledMat = BarFilledMat;
            fbr.unfilledMat = BarUnfilledMat;
            //fbr.margin = 0;
            fbr.rotation = Rot4.North;
            fbr.fillPercent = SheildPercent;
            Vanya_ShieldBelt shield = Parent;
            if (shield != null)
            {
                SheildPercent = shield.Energy / Mathf.Max(1f, shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax));
            }
            else
            {
                SheildPercent = 0f;
            }
            GenDraw.DrawFillableBar(fbr);
            Matrix4x4 matrix = default;
            Vector3 scale = new Vector3(0.25f, 1f, 0.25f);
            Vector3 iconPos = new Vector3(fbr.center.x - (fbr.size.x / 2) - 0.075f, fbr.center.y, fbr.center.z);
            matrix.SetTRS(iconPos, Rot4.North.AsQuat, scale);
            Graphics.DrawMesh(MeshPool.plane025, matrix, material: Def_Icon, 2);
        }
    }
}
