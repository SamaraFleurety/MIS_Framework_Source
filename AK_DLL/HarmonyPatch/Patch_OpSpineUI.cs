using HarmonyLib;
using SpriteEvo;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    //切换动态立绘显示的补丁
    [HarmonyPatch(typeof(Window), "WindowOnGUI")]
    public class Patch_OpSpineUI
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (!AK_ModSettings.displayDynStand) return;
            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count == 0) return;
            if (GC_OperatorDocumentation.cachedOperators.NullOrEmpty()) return;
            Pawn OP = Find.Selector.SelectedPawns.First();
            //选中干员激活Object
            OperatorDocument doc = OP.GetDoc();
            if (doc != null)
            {
                Log.Message(OP.Name + "的换装编号是" + doc.preferedSkin);
            }
            if (GC_OperatorDocumentation.cachedOperators.ContainsKey(OP))
            {
                if (GC_OperatorDocumentation.opUIStandData.Values.Count == 0) return;
                foreach (Dictionary<int, GameObject> map in GC_OperatorDocumentation.opUIStandData.Values)
                {
                    if (map.Values.Count == 0) return;
                    foreach (GameObject ooa in map.Values)
                    {
                        if (doc.preferedSkin == 1 && doc.operatorDef.standAnimation != null)
                        {
                            if (ooa == GC_OperatorDocumentation.opUIStandData[doc][1])
                            {
                                ooa?.SetActive(true);
                                continue;
                            }
                        }
                        else if (doc.preferedSkin > 1 && doc.operatorDef.fashionAnimation.Count > doc.preferedSkin - 2)
                        {
                            if (ooa == GC_OperatorDocumentation.opUIStandData[doc][doc.preferedSkin])
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
                foreach (Dictionary<int, GameObject> map in GC_OperatorDocumentation.opUIStandData.Values)
                {
                    foreach (GameObject ooa in map.Values)
                    {
                        ooa?.SetActive(false);
                    }
                }
            }
        }
    }
}
