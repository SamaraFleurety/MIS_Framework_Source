using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace AK_DLL
{
    [HarmonyPatch(typeof(Dialog_Trade))]
    public class PatchTraderDialog
    {
        private static bool moved = false; // 标志位，记录窗口是否已经移动
        [HarmonyPatch("DoWindowContents")]
        [HarmonyPostfix]
        public static void Postfix_DoWindowContents(Dialog_Trade __instance, ref Rect inRect)
        {
            if (!moved)
            {

                Rect originalRect = Traverse.Create(__instance).Field<Rect>("windowRect").Value;
                Rect newWindowRect = new Rect(originalRect.x * 0.1f, originalRect.y, originalRect.width, originalRect.height);

                Traverse.Create(__instance).Field<Rect>("windowRect").Value = newWindowRect;

                moved = true; // 设置标志位为 true，表示已经移动过窗口
            }
        }
        [HarmonyPatch("PostOpen")]
        [HarmonyPostfix]
        public static void Postfix_PostOpen()
        {
            moved = false;
        }
    }
}
