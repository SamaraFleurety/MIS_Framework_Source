using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Window), "WindowOnGUI")]
    public class PatchWindowOnGUI
    {
        [HarmonyPrefix]
        public static void Prefix ()
        {
            DrawBottomLeftPortrait();
        }


        public static void DrawBottomLeftPortrait()
        {
            if (AK_ModSettings.displayBottomLeftPortrait == false) return;
            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count == 0) return;
            Pawn p = Find.Selector.SelectedPawns.First();
            if (p == null) return;
            OperatorDocument doc = AK_Tool.GetDoc(p);
            if (doc == null) return;

            Color color = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, AK_ModSettings.opacity / 100f);
            Texture2D texture = doc.operatorDef.PreferredStand(doc.preferedSkin);
            Widgets.DrawTextureFitted(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 408, 408), texture, (float)AK_ModSettings.ratio * 0.05f);

            GUI.color = color;
        }
    }
}
