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
            AK_Tool.DrawBottomLeftPortrait();
        }
    }
}
