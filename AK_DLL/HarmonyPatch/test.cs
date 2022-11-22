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
    /*[HarmonyPatch(typeof(InspectPaneFiller), "DoPaneContentsFor")]
    public class test
    {
        [HarmonyPrefix]
        public static void Prefix (ISelectable sel, Rect rect)
        {
            Log.Message($"{rect.x} {rect.y} {rect.height} {rect.width}");
        }
    }*/
    [HarmonyPatch(typeof(Window), "WindowOnGUI")]
    public class test1
    {
        [HarmonyPrefix]
        public static void Prefix ()
        {
            AK_Tool.DrawBottomLeftPortrait();
        }
    }
}
