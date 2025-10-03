using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKE_OperatorExtension.HarmonyPatchs
{
    [HarmonyPatch(typeof(TrailRenderer), "time")]
    public class Patch_TrailRenderAlternativeTimeScale
    {
        [HarmonyPatch(MethodType.Setter)]
        [HarmonyPrefix]
        public static bool Prefix(TrailRenderer __instance)
        {
            if (Bullet_SakiChan.cacedTrailRender.TryGetValue(__instance, out var _) && !Bullet_SakiChan.allowChangeTime)
            {
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }
}
