using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace AK_DLL
{
    //个人详细属性界面显示立绘
    [HarmonyPatch(typeof(Dialog_InfoCard), "FillCard")]
    public class PatchInfoCardPortrait
    {
        [HarmonyPostfix]
        public static void postfix(Rect cardRect, Dialog_InfoCard __instance, Thing ___thing)
        {
            Pawn p = ___thing as Pawn; 
            OperatorDocument doc;

            if (p != null && ((doc = p.GetDoc()) != null)) 
            {
                Rect position = cardRect.AtZero(); 
                position.width = 384f;
                position.height = 384f;
                position.x = cardRect.width * 0.75f - position.width / 2f + 18f;
                position.y = cardRect.center.y + 136f - position.height / 2f;
                Texture2D stand = ContentFinder<Texture2D>.Get(doc.operatorDef.stand);
                GUI.DrawTexture(position, stand, ScaleMode.ScaleAndCrop, true);
            }
        }
    }
}
