using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AK_DLL {
    [HarmonyPatch(typeof(HumanlikeMeshPoolUtility), "GetHumanlikeHairSetForPawn")]
    public class PatchHairOffset
    {
        [HarmonyAfter]
        public static void postfix(Pawn pawn)
        {
            Log.Message("HAIR OFFSET INFO");
        }
    }
}
