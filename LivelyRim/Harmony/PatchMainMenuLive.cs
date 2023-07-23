using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using RimWorld.Planet;
using System.Reflection;

namespace FS_LivelyRim
{
    [HarmonyPatch(typeof(UIRoot_Entry), "DoMainMenu")]
    public class PatchMainMenuLive
    {
        [HarmonyPostfix]
        public static void postfix(UIRoot_Entry __instance)
        {
            PropertyInfo doMenuProp = typeof(UIRoot_Entry).GetProperty("ShouldDoMainMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            bool doMenuFlag = (bool)doMenuProp.GetValue(__instance);
            if (!WorldRendererUtility.WorldRenderedNow && doMenuFlag)
            {
            }
            else
            {
            }
        }
    }
}
