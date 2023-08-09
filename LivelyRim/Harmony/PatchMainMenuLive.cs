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
            PropertyInfo doMenuProp = typeof(UIRoot_Entry).GetProperty("ShouldDoMainMenu", BindingFlags.Instance | BindingFlags.NonPublic);
            bool doMenuFlag = (bool)doMenuProp.GetValue(__instance);
            if (!WorldRendererUtility.WorldRenderedNow && doMenuFlag)
            {
                if (cachedJanus == null)
                {
                    LiveModelDef l2dDef = DefDatabase<LiveModelDef>.GetNamed("AZ_Live_Janus");
                    //GameObject janusPrefab = TypeDef.janustest.LoadAsset<GameObject>("FileReferences_Moc_0");
                    cachedJanus = FS_Tool.InstantiateLive2DModel(TypeDef.janustest, l2dDef.modID, l2dDef.modelName, rigJsonPath: l2dDef.rigJsonPath, eyeFollowMouse : false);
                    cachedJanus.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;
                    //cachedJanus.transform.position += new Vector3(10, 0, 0);
                }
                cachedJanus.transform.rotation = Quaternion.Euler(90, 0, 0);
                Vector3 v3 = cachedJanus.transform.rotation.eulerAngles;
                cachedJanus.SetActive(true);
            }
            else
            {
                if (cachedJanus != null)
                {
                    cachedJanus.SetActive(false);
                }
            }
        }
    }

    [HarmonyPatch(typeof(MainMenuDrawer), "MainMenuOnGUI")]
    public class PatchMainMenuDrawer
    {
        [HarmonyPostfix]
        public static void postfix()
        {
            Texture t = OffscreenRendering.OffCamera.targetTexture;
            GUI.DrawTexture(new Rect(0,0, 1920, 1080), t);
        }
    }
}
