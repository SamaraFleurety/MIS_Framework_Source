using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace AK_DLL
{
    //要么用这种低级的办法禁止IMGUI，要么自己去写个shader改掉unityengine.GUI里面的4个material

    [HarmonyPatch(typeof(UIRoot), "UIRootOnGUI")]
    public static class PatchDisableUIRootOnGUI
    {
        [HarmonyPrefix]
        public static bool prefix()
        {
            if (AK_Tool.disableIMGUI)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UIRoot), "UIRootUpdate")]
    public static class PatchDisableUIRootUpdate
    {
        [HarmonyPrefix]
        public static bool prefix()
        {
            if (AK_Tool.disableIMGUI)
            {
                return false;

            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UIRoot_Entry), "UIRootOnGUI")]
    public static class PatchDisableUIRootOnGUIE
    {
        [HarmonyPrefix]
        public static bool prefix()
        {
            if (AK_Tool.disableIMGUI)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UIRoot_Entry), "UIRootUpdate")]
    public static class PatchDisableUIRootUpdateE
    {
        [HarmonyPrefix]
        public static bool prefix()
        {
            if (AK_Tool.disableIMGUI)
            {
                return false;

            }
            return true;
        }
    }
    [HarmonyPatch(typeof(UIRoot_Play), "UIRootOnGUI")]
    public static class PatchDisableUIRootOnGUIP
    {
        [HarmonyPrefix]
        public static bool prefix()
        {
            if (AK_Tool.disableIMGUI)
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(UIRoot_Play), "UIRootUpdate")]
    public static class PatchDisableUIRootUpdateP
    {
        [HarmonyPrefix]
        public static bool prefix()
        {
            if (AK_Tool.disableIMGUI)
            {
                return false;

            }
            return true;
        }
    }
}
