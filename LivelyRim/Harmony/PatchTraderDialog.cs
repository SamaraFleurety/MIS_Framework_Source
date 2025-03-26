#define L2DFull
#if L2DFull

using HarmonyLib;
using Live2D.Cubism.Rendering;
using RimWorld;
using UnityEngine;
using Verse;

namespace FS_LivelyRim
{
    [HarmonyPatch(typeof(Dialog_Trade))]
    public class PatchTraderDialog
    {
        private static bool moved = false; // 标志位，记录窗口是否已经移动
        [HarmonyPatch("DoWindowContents")]
        [HarmonyPostfix]
        public static void Postfix_DoWindowContents(Dialog_Trade __instance, ref Rect inRect)
        {
            if (!FS_ModSettings.merchantSideLive) return;
            if (!moved)
            {

                Rect originalRect = Traverse.Create(__instance).Field<Rect>("windowRect").Value;
                Rect newWindowRect = new Rect(originalRect.x * 0.1f, originalRect.y, originalRect.width, originalRect.height);

                Traverse.Create(__instance).Field<Rect>("windowRect").Value = newWindowRect;

                moved = true; // 设置标志位为 true，表示已经移动过窗口
            }
        }

        public static LiveModelDef def;
        [HarmonyPatch("PostOpen")]
        [HarmonyPostfix]
        public static void Postfix_PostOpen()
        {
            if (!FS_ModSettings.merchantSideLive) return;
            if (FS_Tool.defaultModelInstance == null)
            {
                if (def == null)
                    def = DefDatabase<LiveModelDef>.GetNamedSilentFail(FS_ModSettings.l2dDefname);
                if (def == null)
                {
                    Log.Error($"[FS.L2D] Try loading null def named {FS_ModSettings.l2dDefname}. resetting to janus");
                    def = DefDatabase<LiveModelDef>.GetNamed("AZ_Live_Janus");
                }
                FS_Utilities.ChangeDefaultModel(def);
            }

            moved = false; 
            //仅在开始时绘制一次模型
            GameObject ins = FS_Utilities.DrawModel(DisplayModelAt.MerchantRight, FS_Tool.defaultModelDef);

            ins.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;

        }
        [HarmonyPatch("Close")]
        [HarmonyPostfix]
        public static void Postfix_Close()
        {
            if (!FS_ModSettings.merchantSideLive) return;
            FS_Utilities.SetDefaultModelInactive();
        }
    }
}

#endif