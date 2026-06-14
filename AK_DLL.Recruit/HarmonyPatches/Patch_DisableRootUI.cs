using HarmonyLib;
using RimWorld;
using Verse;

namespace AK_DLL.Recruit
{
    //要么用这种低级的办法禁止IMGUI，要么自己去写个shader改掉unityengine.GUI里面的4个material

    [HarmonyPatch(typeof(UIRoot))]
    public static class Patch_DisableUIRootOnGUI
    {
        //UIRoot
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootOnGUI))]
        public static bool Prefix_UIRootOnGUI()
        {
            return !AK_AssetTool.disableIMGUI;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIRoot), nameof(UIRoot.UIRootUpdate))]
        public static bool Prefix_UIRootUpdate()
        {
            return !AK_AssetTool.disableIMGUI;
        }

        //UIRoot_Entry
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIRoot_Entry), nameof(UIRoot_Entry.UIRootOnGUI))]
        public static bool Prefix_UIRootEntryOnGUI()
        {
            return !AK_AssetTool.disableIMGUI;
        }
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIRoot_Entry.UIRootUpdate))]
        public static bool Prefix_UIRootEntryUpdate()
        {
            return !AK_AssetTool.disableIMGUI;
        }

        //UIRoot_Play
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIRoot_Play.UIRootOnGUI))]
        public static bool Prefix_UIRootPlayOnGUI()
        {
            return !AK_AssetTool.disableIMGUI;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIRoot_Play.UIRootUpdate))]
        public static bool Prefix_UIRootPlayUpdate()
        {
            return !AK_AssetTool.disableIMGUI;
        }
    }
}
