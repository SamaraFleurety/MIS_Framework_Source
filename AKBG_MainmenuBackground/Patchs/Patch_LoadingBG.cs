using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKBG_MainmenuBackground.HarmonyPatchs
{
    [HarmonyPatch(typeof(UI_BackgroundMain), "DoOverlay"), StaticConstructorOnStartup]
    public class Patch_LoadingBG
    {
        public static Texture2D BGTexture => AKBG_ModSettings.LoadingBG_Tex;
        public static Texture2D VanillaOverrideBGImage
        {
            get => ((UI_BackgroundMain)UIMenuBackgroundManager.background).overrideBGImage;
            set => ((UI_BackgroundMain)UIMenuBackgroundManager.background).overrideBGImage = value;
        }

        public static bool loaded = false;

        public static bool initialized = false;

        public static bool gameLoading = true;

        [HarmonyPostfix]
        public static void postfix()
        {
            if (!GenScene.InEntryScene || !gameLoading)
            {
                return;
            }
            /*if (!loaded)
            {
                loaded = true;
            }*/
            if (BGTexture != null && BGTexture != VanillaOverrideBGImage)
            {
                VanillaOverrideBGImage = BGTexture;
                initialized = true;
            }
        }
    }
}
