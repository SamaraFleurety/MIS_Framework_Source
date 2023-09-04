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
        static GameObject cachedJanus = null;
        [HarmonyPostfix]
        public static void postfix(UIRoot_Entry __instance)
        {
            //初始化。设置里面不可以直接存def，因为读档的时候还没加载def
            if (FS_Tool.defaultModelInstance == null) FS_Utilities.ChangeDefaultModel(DefDatabase<LiveModelDef>.GetNamed(FS_ModSettings.l2dDefname));
            
            PropertyInfo doMenuProp = typeof(UIRoot_Entry).GetProperty("ShouldDoMainMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            bool doMenuFlag = (bool)doMenuProp.GetValue(__instance);
            if (!WorldRendererUtility.WorldRenderedNow && doMenuFlag)
            {
                GameObject ins =  FS_Utilities.DrawModel(DisplayModelAt.MainMenu, FS_Tool.defaultModelDef);

                ins.transform.rotation = Quaternion.Euler(90, 0, 0);
                ins.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;

                //他猫猫滴，这个界面是perspective view的camera
                OffscreenRendering.MainCamera.orthographic = true;

                OffscreenRendering.MainCamera.orthographicSize = 11;

                /*if (cachedJanus == null)
                {
                    LiveModelDef l2dDef = DefDatabase<LiveModelDef>.GetNamed("AZ_Live_Janus");
                    //GameObject janusPrefab = TypeDef.janustest.LoadAsset<GameObject>("FileReferences_Moc_0");
                    cachedJanus = FS_Tool.InstantiateLive2DModel(TypeDef.janustest, l2dDef.modID, l2dDef.modelName, rigJsonPath: l2dDef.rigJsonPath, eyeFollowMouse : false);
                    cachedJanus.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;
                    //cachedJanus.transform.position += new Vector3(10, 0, 0);
                }
                cachedJanus.transform.rotation = Quaternion.Euler(90, 0, 0);
                Vector3 v3 = cachedJanus.transform.rotation.eulerAngles;
                cachedJanus.SetActive(true);*/
            }
            else
            {
                FS_Utilities.SetDefaultModelInActive();
            }
        }

    }

    [HarmonyPatch(typeof(MainMenuDrawer), "MainMenuOnGUI")]
    public class PatchMainMenuDrawer
    {
        static Texture t => OffscreenRendering.OffCamera.targetTexture;
        [HarmonyPostfix]
        public static void postfix()
        {
            if (t == null) return;
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

            GUI.DrawTexture(new Rect(0,0, width, height), t);
        }
    }
}
