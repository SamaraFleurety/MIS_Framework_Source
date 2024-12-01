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

        private float SheildPercent;
        private Vector3 IconMargin => Vector3.back * 1.125f + Vector3.left * 0.8f;
        private static Vector3 BottomMargin => Vector3.back * 1.125f;
        private static readonly Vector2 BarSize = new Vector2(1.5f, 0.025f);
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
        public override void CompDrawWornExtras()
        {
            /*if (!AK_ModSettings.displayBarModel || ModLister.GetActiveModWithIdentifier("Mlie.VanyaShield") == null)
            {
                return;
            }*/
            if (Wearer == null || !Wearer.Drafted)
            {
                return;
            }
            if (BarFilledMat == null || BarUnfilledMat == null)
            {
                BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(245, 245, 245, 190));
                BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.50f));
            }
            //Log.Message("有盾");
            GenDraw.FillableBarRequest fbr = default;
            fbr.center = Wearer.DrawPos + (Vector3.up * 3f) + BottomMargin;
            fbr.size = BarSize;
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
            GenDefendIcon();
            Matrix4x4 matrix = default;
            Vector3 scale = new Vector3(0.25f, 1f, 0.25f);
            matrix.SetTRS(Wearer.DrawPos + IconMargin, Rot4.North.AsQuat, scale);
            Graphics.DrawMesh(MeshPool.plane025, matrix, material: Def_Icon, 2);
        }
    }
}
