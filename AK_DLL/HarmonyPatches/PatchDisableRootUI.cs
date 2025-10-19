using HarmonyLib;
using RimWorld;
using Verse;

namespace AK_DLL
{
    //要么用这种低级的办法禁止IMGUI，要么自己去写个shader改掉unityengine.GUI里面的4个material

    [HarmonyPatch(typeof(UIRoot))]
    public static class Patch_DisableUIRootOnGUI
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIRoot.UIRootOnGUI))]
        public static bool Prefix_UIRootOnGUI()
        {
            return !AK_Tool.disableIMGUI;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIRoot.UIRootUpdate))]
        public static bool Prefix_UIRootUpdate()
        {
            return !AK_Tool.disableIMGUI;
        }
    }


    [HarmonyPatch(typeof(UIRoot_Entry))]
    public static class PatchDisableUIRootOnGUIEntry
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIRoot_Entry.UIRootOnGUI))]
        public static bool Prefix_UIRootOnGUI()
        {
            return !AK_Tool.disableIMGUI;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIRoot_Entry.UIRootUpdate))]
        public static bool Prefix_UIRootUpdate()
        {
            return !AK_Tool.disableIMGUI;
        }
    }

    [HarmonyPatch(typeof(UIRoot_Play), "UIRootOnGUI")]
    public static class PatchDisableUIRootOnGUIPlay
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIRoot_Play.UIRootOnGUI))]
        public static bool Prefix_UIRootOnGUI()
        {
            return !AK_Tool.disableIMGUI;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIRoot_Play.UIRootUpdate))]
        public static bool Prefix_UIRootUpdate()
        {
            return !AK_Tool.disableIMGUI;
        }
    }

}
