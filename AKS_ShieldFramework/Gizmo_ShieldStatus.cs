﻿using UnityEngine;
using Verse;

namespace AKS_Shield
{
    public class Gizmo_ShieldStatus : Gizmo
    {
        public TC_GenericShield shield;
        private static readonly Texture2D FullShieldBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.2f, 0.2f, 0.24f));

        private static readonly Texture2D EmptyShieldBarTex = SolidColorMaterials.NewSolidColorTexture(Color.clear);
        public override float GetWidth(float maxWidth)
        {
            return 140f;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            Rect rect = new Rect(topLeft.x, topLeft.y, GetWidth(maxWidth), 75f);
            Rect rect2 = rect.ContractedBy(6f);
            Widgets.DrawWindowBackground(rect);
            Rect rect3 = rect2;
            rect3.height = rect.height / 2f;
            Text.Font = GameFont.Tiny;
            Widgets.Label(rect3, shield.VanillaGizmoLabel);  //顶上那行字
            Rect rect4 = rect2;
            rect4.yMin = rect2.y + rect2.height / 2f;
            float fillPercent = shield.Energy / shield.EnergyMax;
            Widgets.FillableBar(rect4, fillPercent, FullShieldBarTex, EmptyShieldBarTex, doBorder: false);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(rect4, (shield.Energy).ToString("F0") + " / " + (shield.EnergyMax).ToString("F0"));
            Text.Anchor = TextAnchor.UpperLeft;
            TooltipHandler.TipRegion(rect2, shield.VanillaGizmoDesc /*"ShieldPersonalTip".Translate()*/);
            return new GizmoResult(GizmoState.Clear);
        }
    }
}
