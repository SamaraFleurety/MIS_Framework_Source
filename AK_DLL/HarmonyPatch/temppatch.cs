//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using HarmonyLib;
//using UnityEngine;
//using Verse;
//using Verse.AI;
//using System.Reflection;

//namespace AK_DLL
//{

//    [HarmonyPatch(typeof(Debug), "Log")]
//    public static class PatchReloadAll
//    {
//        [HarmonyPostfix]
//        public static void postfix(object message)
//        {
//            try
//            {
//                Log.Message(message as string);
//            }
//            catch
//            {

//            }
//        }
//    }
//    [HarmonyPatch(typeof(Debug), "LogError")]
//    public static class Patchlogerror
//    {
//        [HarmonyPostfix]
//        public static void postfix(object message)
//        {
//            try
//            {
//                Log.Message(message as string);
//            }
//            catch
//            {

//            }
//        }
//    }
//}

using RimWorld;
using System.Reflection;
using Verse;
using HarmonyLib;
using System;


namespace VanillaPlantsExpanded
{



    [HarmonyPatch(typeof(Plant))]
    [HarmonyPatch("PlantCollected")]
    public static class PatchPlant
    {
        [HarmonyPostfix]
        public static void postfix(Plant __instance, Pawn by)
        {
            Log.Message("used PlantCollected function");
        }
    }
}