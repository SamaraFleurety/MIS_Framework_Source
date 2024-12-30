using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LMA_Lib.HarmonyPatchs
{
    //草 这玩意就是实际射程
    /*[HarmonyPatch(typeof(Verb), "EffectiveRange", MethodType.Getter)]
    public class Patch_Range
    {
        [HarmonyPostfix]
        public static void fix(ref float __result)
        {
            __result = 50;
        }
    }*/
}
