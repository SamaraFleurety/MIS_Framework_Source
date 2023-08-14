using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using Live2D.Cubism.Rendering;

namespace FS_LivelyRim
{
    [HarmonyPatch(typeof(Dialog_Trade))]
    public class PatchTraderDialog
    {
        private static bool moved = false; // 标志位，记录窗口是否已经移动
        static GameObject cachedJanus = null;
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
            GameObject ins = FS_Utilities.DrawModel(DisplayModelAt.MerchantRight, FS_Tool.defaultModelDef);

            ins.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;

            /*if (cachedJanus == null)
            {
                LiveModelDef l2dDef = DefDatabase<LiveModelDef>.GetNamed("AZ_Live_Janus");
                //GameObject janusPrefab = TypeDef.janustest.LoadAsset<GameObject>("FileReferences_Moc_0");
                cachedJanus = FS_Tool.InstantiateLive2DModel(TypeDef.janustest, l2dDef.modID, l2dDef.modelName, rigJsonPath: l2dDef.rigJsonPath, eyeFollowMouse: false);
                cachedJanus.GetComponent<CubismRenderController>().SortingMode = CubismSortingMode.BackToFrontOrder;
                cachedJanus.transform.position += new Vector3(10, 0, 0);
                Vector3 v3 = cachedJanus.transform.rotation.eulerAngles;
                Log.Message($"{v3.x};;{v3.y};;{v3.z}");
            }
            cachedJanus.SetActive(true);*/
        }
        [HarmonyPatch("Close")]
        [HarmonyPostfix]
        public static void Postfix_Close()
        {
            FS_Utilities.SetDefaultModelInActive();
            //cachedJanus.SetActive(false);
            Log.Message("mect close");
        }
    }
}
