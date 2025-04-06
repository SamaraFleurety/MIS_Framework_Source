using HarmonyLib;
using RimWorld;
using RimWorld.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    [HarmonyPatch(typeof(FloatMenuMakerMap), "ChoicesAtFor")]
    [HarmonyPatch(new Type[] { typeof(Vector3), typeof(Pawn), typeof(bool) })]
    //打算暴力加个菜单了
    public class Patch_FloatMenu
    {
        public static void Postfix(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> __result)
        {
            IntVec3 clickCell = IntVec3.FromVector3(clickPos);
            //下面还没写
        }
    }
}
