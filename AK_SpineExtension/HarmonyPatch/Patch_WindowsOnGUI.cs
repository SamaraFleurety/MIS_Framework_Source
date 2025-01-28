using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AK_DLL;
using UnityEngine;
using Verse;
using SpriteEvo;

namespace AK_SpineExtention
{
    #region 左上角动态立绘
    [HarmonyPatch(typeof(AK_Tool), "DrawBottomLeftPortrait")]
    public class Patch_DrawBottomLeftPortrait
    {
        public static bool CanDrawNow => AK_ModSettings.displayAnimationLeftPortrait;
        [HarmonyPrefix]
        public static bool DrawAnimationtLeftPortrait()
        {
            if (!CanDrawNow) return true;
            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count == 0) return false;
            Pawn p = Find.Selector.SelectedPawns.First();
            if (p == null) return false;
            OperatorDocument doc = AK_Tool.GetDoc(p);
            if (doc == null) return false;
            //动态立绘显示
            int fashion = doc.preferedSkin;
            if ((fashion == 1) && (doc.operatorDef.fashionAnimation.Count > fashion - 1))
            {
                if (!GC_OpAnimationDocument.cachedOpSkinAnimation.ContainsKey(doc))
                {
                    GC_OpAnimationDocument.cachedOpSkinAnimation.Add(doc, new Dictionary<int, GameObject>());
                }
                GameObject ooa = GC_OpAnimationDocument.cachedOpSkinAnimation[doc].TryGetValue(fashion);
                if (ooa == null)
                {
                    SpriteEvo.AnimationDef def = doc.operatorDef.fashionAnimation?[fashion - 1]?.FindDef();
                    if (def == null) return true;
                    ooa = SkeletonAnimationUtility.InstantiateInGameOnly(def, key: def.defName);
                    if (ooa == null) return true;
                    GC_OpAnimationDocument.cachedOpSkinAnimation[doc].Add(fashion, ooa);
                }
                else
                {
                    if (ooa.activeInHierarchy)
                        Widgets.DrawTextureFitted(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 408, 408), ooa.AnimationTexture(), AK_ModSettings.ratio * 0.05f);
                }
                return false;
            }
            //皮肤切换
            else if ((fashion > 1) && (doc.operatorDef.fashionAnimation.Count > fashion - 2))
            {
                if (!GC_OpAnimationDocument.cachedOpSkinAnimation.ContainsKey(doc))
                {
                    GC_OpAnimationDocument.cachedOpSkinAnimation.Add(doc, new Dictionary<int, GameObject>());
                }
                GameObject oob = GC_OpAnimationDocument.cachedOpSkinAnimation[doc].TryGetValue(fashion);
                if (oob == null)
                {
                    SpriteEvo.AnimationDef def = doc.operatorDef.fashionAnimation?[fashion - 1]?.FindDef();
                    if (def == null) return true;
                    oob = SkeletonAnimationUtility.InstantiateInGameOnly(def, key: def.defName);
                    if (oob == null) return true;
                    GC_OpAnimationDocument.cachedOpSkinAnimation[doc].Add(fashion, oob);
                }
                else
                {
                    if (oob.activeInHierarchy)
                        Widgets.DrawTextureFitted(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 408, 408), oob.AnimationTexture(), AK_ModSettings.ratio * 0.05f);
                }
                return false;
            }
            //else
            return true;
        }
    }
    #endregion

    #region 切换动态立绘显示
    [HarmonyPatch(typeof(Window), "WindowOnGUI")]
    public class Patch_OpSpineUI
    {
        public static bool CanDrawNow => AK_ModSettings.displayAnimationLeftPortrait;
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!CanDrawNow) return;
            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count == 0) return;
            if (GC_OperatorDocumentation.cachedOperators.NullOrEmpty()) return;
            Pawn OP = Find.Selector.SelectedPawns.First();
            //选中干员激活Object
            OperatorDocument doc = OP.GetDoc();
            if (GC_OperatorDocumentation.cachedOperators.ContainsKey(OP))
            {
                if (GC_OpAnimationDocument.cachedOpSkinAnimation.Values.Count == 0) return;
                foreach (Dictionary<int, GameObject> map in GC_OpAnimationDocument.cachedOpSkinAnimation.Values)
                {
                    if (map.Values.Count == 0) return;
                    int fashion = doc.preferedSkin;
                    foreach (GameObject ooa in map.Values)
                    {
                        //在这里实现切换皮肤功能
                        if (fashion == 1 && doc.operatorDef.fashionAnimation?.Count > fashion - 1)
                        {
                            if (!GC_OpAnimationDocument.cachedOpSkinAnimation[doc].ContainsKey(fashion)) return;
                            if (ooa == GC_OpAnimationDocument.cachedOpSkinAnimation[doc][fashion])
                            {
                                ooa?.SetActive(true);
                                continue;
                            }
                        }
                        else if (fashion > 1 && doc.operatorDef.fashionAnimation?.Count > fashion - 1)
                        {
                            if (ooa == GC_OpAnimationDocument.cachedOpSkinAnimation[doc][fashion])
                            {
                                ooa?.SetActive(true);
                                continue;
                            }
                        }
                        ooa?.SetActive(false);
                    }
                }
            }
            else
            {
                foreach (Dictionary<int, GameObject> map in GC_OpAnimationDocument.cachedOpSkinAnimation.Values)
                {
                    foreach (GameObject ooa in map.Values)
                    {
                        ooa?.SetActive(false);
                    }
                }
            }
        }
    }
    #endregion
}
