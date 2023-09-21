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
using UnityEngine;
using Live2D.Cubism.FSAddon;
using Live2D.Cubism.Rendering;

namespace FS_LivelyRim
{
    [HarmonyPatch(typeof(UIRoot_Entry), "DoMainMenu")]
    public class PatchMainMenuLive
    {
        [HarmonyPostfix]
        public static void postfix(UIRoot_Entry __instance)
        {
            //初始化。设置里面不可以直接存def，因为读档的时候还没加载def
            if (FS_Tool.defaultModelInstance == null)
            {
                LiveModelDef def = DefDatabase<LiveModelDef>.GetNamedSilentFail(FS_ModSettings.l2dDefname);
                if (def == null)
                {
                    Log.Error($"[FS.L2D] Try loading null def named {FS_ModSettings.l2dDefname}. resetting to janus");
                    def = DefDatabase<LiveModelDef>.GetNamed("AZ_Live_Janus");
                }
                FS_Utilities.ChangeDefaultModel(def);
            }

            if (!FS_ModSettings.mainMenuLive) FS_Utilities.SetDefaultModelInActive();

            PropertyInfo doMenuProp = typeof(UIRoot_Entry).GetProperty("ShouldDoMainMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            bool doMenuFlag = (bool)doMenuProp.GetValue(__instance);
            if (!WorldRendererUtility.WorldRenderedNow && doMenuFlag)
            {
                GameObject ins = FS_Utilities.DrawModel(DisplayModelAt.MainMenu, FS_Tool.defaultModelDef);

                ins.transform.rotation = Quaternion.Euler(90, 0, 0);

                //他猫猫滴，这个界面是perspective view的camera
                OffscreenRendering.MainCamera.orthographic = true;

                OffscreenRendering.MainCamera.orthographicSize = 11;

                OffscreenRendering.MainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
            else
            {
                FS_Utilities.SetDefaultModelInActive();
            }
            //PatchMainMenuDrawer.postfix();
            return;
        }

    }

    [HarmonyPatch(typeof(MainMenuDrawer), "MainMenuOnGUI")]
    public class PatchMainMenuDrawer
    {
        static Texture t => OffscreenRendering.OffCamera.targetTexture;
        [HarmonyPostfix]
        public static void postfix()
        {
            if (t == null || !FS_ModSettings.mainMenuLive) return;
            int width = Screen.width;
            int height = Screen.height;
            int widthRatio = width / 16;
            int heightRatio = height / 9;
            if (widthRatio != heightRatio)
            {
                //超长屏幕，比如32:9
                if (widthRatio > heightRatio)
                {
                    width = heightRatio * 16;
                }
                //超高屏
                else
                {
                    height = widthRatio * 9;
                }
            }

            width = (int)(width / Prefs.UIScale);
            height = (int)(height / Prefs.UIScale);

            GUI.DrawTexture(new Rect(0, 0, width, height), t);
        }
    }
}
