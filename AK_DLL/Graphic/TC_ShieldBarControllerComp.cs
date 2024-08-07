using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using VanyaMod;
using Verse;

namespace AK_DLL
{
    public class TC_ShieldBarControllerComp : ThingComp
    {
        private Pawn Pawn => parent as Pawn;
        private float SheildPercent;
        private Vector3 IconMargin => Vector3.back * 1.125f + Vector3.left * 0.8f;
        private static Vector3 BottomMargin => Vector3.back * 1.125f;
        private static readonly Vector2 BarSize = new Vector2(1.5f, 0.025f);
        private Material BarFilledMat;
        private Material BarUnfilledMat;
        private Material Def_Icon;
        private List<Apparel> apparels = new List<Apparel>();
        private Vanya_ShieldBelt AKEShield;
        public bool HasVanyaShield()
        {
            apparels= Pawn.apparel.WornApparel;
            foreach(Apparel a in apparels)
            {
                if (a is Vanya_ShieldBelt)
                {
                    AKEShield = a as Vanya_ShieldBelt;
                    return true;
                }
            }
            return false;
        }
        private void GenDefendIcon()
        {
            if (Def_Icon == null)
            {
                Def_Icon = AK_BarUITool.DEF_Icon;
            }
        }
        public override void PostDraw()
        {
            base.PostDraw();
            if (!AK_ModSettings.displayBarModel || ModLister.GetActiveModWithIdentifier("Mlie.VanyaShield") == null)
            {
                return;
            }
            if (!Pawn.Drafted || Pawn.GetDoc() == null)
            {
                return;
            }
            if (BarFilledMat == null || BarUnfilledMat == null) 
            {
                BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(245, 245, 245, 180));
                BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.75f));
            }
            GenDraw.FillableBarRequest fbr = default;
            fbr.center = Pawn.DrawPos + (Vector3.up * 3f) + BottomMargin;
            fbr.size = BarSize;
            fbr.filledMat = BarFilledMat;
            fbr.unfilledMat = BarUnfilledMat;
            fbr.margin = 0.001f;
            fbr.rotation = Rot4.North;
            fbr.fillPercent = SheildPercent;
            if (HasVanyaShield())
            {
                SheildPercent = AKEShield.Energy / Mathf.Max(1f, AKEShield.GetStatValue(StatDefOf.EnergyShieldEnergyMax));
            }
            else
            {
                SheildPercent = 0f;
            }
            GenDraw.DrawFillableBar(fbr);
            GenDefendIcon();
            Matrix4x4 matrix = default;
            Vector3 scale = new Vector3(0.25f, 1f, 0.25f);
            matrix.SetTRS(Pawn.DrawPos + IconMargin, Rot4.North.AsQuat, scale);
            Graphics.DrawMesh(MeshPool.plane025, matrix, material: Def_Icon, 2);
        }
    }
}
