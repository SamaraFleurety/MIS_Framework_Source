using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.HarmonyPatchs
{
    /*[HarmonyPatch(typeof(DirectXmlCrossRefLoader))]
    public class Patch_ForbidCrossRef
    {
        [HarmonyPatch("RegisterObjectWantsCrossRef")]
        [HarmonyPatch(new Type[] { typeof(object), typeof(FieldInfo), typeof(string), typeof(string), typeof(string), typeof(Type) })]
        [HarmonyPrefix]
        public static bool prefix_def_single1(string targetDefName)
        {
            if (Patch_DefsInMod.ShouldSkipDefname(targetDefName))
            {
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }

        [HarmonyPatch("RegisterObjectWantsCrossRef")]
        [HarmonyPatch(new Type[] { typeof(object), typeof(string), typeof(string), typeof(string), typeof(string), typeof(Type) })]
        [HarmonyPrefix]
        public static bool prefix_def_single2(string targetDefName)
        {
            var a = 5;
            if (Patch_DefsInMod.ShouldSkipDefname(targetDefName))
            {
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }*/
}
