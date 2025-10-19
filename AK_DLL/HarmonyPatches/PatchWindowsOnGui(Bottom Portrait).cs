using AK_DLL.UI;
using HarmonyLib;
using System.Linq;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Window), "WindowOnGUI")]
    public class Patch_WindowOnGUI
    {
        private static string _lastSpineDefName = "";
        public static GameObject LastSpineInstance;

        [HarmonyPrefix]
        public static void Prefix()
        {
            DrawBottomLeftPortrait();
        }


        public static void DrawBottomLeftPortrait()
        {
            if (!AK_ModSettings.displayBottomLeftPortrait) return;
            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || !Find.Selector.AnyPawnSelected || Find.Selector.SelectedPawns.Count == 0) return;
            Pawn p = Find.Selector.SelectedPawns.First();

            OperatorDocument doc = p?.GetDoc();
            if (doc == null || doc.operatorDef.alwaysHideStand) return;

            int skinIndex = doc.preferedSkin;
            //Log.Message($"{doc.operatorDef.label} has skin {skinIndex}");
            if (skinIndex is >= 1000 and < 2000) return; //没做支持l2d

            Color color = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, AK_ModSettings.opacity / 100f);

            if (skinIndex < 1000)
            {
                Texture2D texture = doc.operatorDef.PreferredStand(doc.preferedSkin);
                Widgets.DrawTextureFitted(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 408, 408), texture, AK_ModSettings.ratio * 0.05f);
                LastSpineInstance?.SetActive(false);
                LastSpineInstance = null;
            }
            else if (skinIndex >= 2000 && ModLister.GetActiveModWithIdentifier("Paluto22.SpriteEvo") != null) //spine立绘渲染
            {
                string spineDefname = doc.operatorDef.fashionAnimation[doc.preferedSkin - 2000];
                if (spineDefname != _lastSpineDefName)
                {
                    LastSpineInstance?.SetActive(false);
                    LastSpineInstance = null;
                    _lastSpineDefName = spineDefname;
                }
                LastSpineInstance ??= RIWindow_MainMenu.DrawSpine2DModel(spineDefname);
                if (!LastSpineInstance.activeSelf) LastSpineInstance.SetActive(true);

                Widgets.DrawTextureFitted(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 408, 408), RIWindow_MainMenu.GetOrSetSpineRenderTexture(LastSpineInstance, 1080, 1080), AK_ModSettings.ratio * 0.05f);
            }

            GUI.color = color;
        }
    }
}
