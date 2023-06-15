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