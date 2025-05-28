using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKBG_MainmenuBackground.HarmonyPatchs
{
    //在主菜单更改显示的背景
    [HarmonyPatch(typeof(MainMenuDrawer), "Init")]
    public class Patch_MainMenuBG
    {
        private static long CurrentTime => AK_DLL.RealTime.CurrentTimeTick;
        private static long RefreshInterval => AK_DLL.RealTime.second * AKBG_ModSettings.randomMainmenuBGInterval;
        private static long lastTime = 0;
        [HarmonyPostfix]
        public static void fix()
        {
            Patch_LoadingBG.gameLoading = false;
            if (!AKBG_ModSettings.enableMainmenuBG) return;
            if (AKBG_ModSettings.randomMainmenuBG)
            {
                if (lastTime + RefreshInterval <= CurrentTime)
                {
                    lastTime = CurrentTime;
                    AKBG_ModSettings.mainmenuBG_Path = null;
                    //SSR_ModSettings.__instance.Write();
                }
            }
            if (Patch_LoadingBG.VanillaOverrideBGImage != AKBG_ModSettings.MainmenuBG_Tex) Patch_LoadingBG.VanillaOverrideBGImage = AKBG_ModSettings.MainmenuBG_Tex;
        }
    }
}
