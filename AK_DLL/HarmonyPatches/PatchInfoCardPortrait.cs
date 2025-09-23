using HarmonyLib;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    //个人详细属性界面显示立绘
    [HarmonyPatch(typeof(Dialog_InfoCard), "FillCard")]
    public class PatchInfoCardPortrait
    {
        [HarmonyPostfix]
        public static void postfix(Rect cardRect, Dialog_InfoCard __instance, Thing ___thing)
        {
            OperatorDocument doc;

            if (___thing is Pawn p && ((doc = p.GetDoc()) != null) && !doc.operatorDef.alwaysHideStand)
            {
                Rect position = cardRect.AtZero();
                position.width = 384f;
                position.height = 384f;
                position.x = (cardRect.width * 0.75f) - (position.width / 2f) + 18f;
                position.y = cardRect.center.y + 136f - (position.height / 2f);
                Texture2D stand = doc.operatorDef.PreferredStand(0); /*ContentFinder<Texture2D>.Get(doc.operatorDef.stand);*/
                GUI.DrawTexture(position, stand, ScaleMode.ScaleAndCrop, true);
            }
        }
    }
}
