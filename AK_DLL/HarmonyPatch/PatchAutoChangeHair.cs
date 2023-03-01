using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace AK_DLL
{

    [HarmonyPatch(typeof(Dialog_InfoCard), "FillCard")]
    public class PatchAutoChangeHair
    {
        [HarmonyPrefix]
        public bool prefix(Pawn pawn, ref string __result)
        {
            if (pawn.GetDoc() != null)
            {
                __result = null;
            }
            return false;
        }
    }
}
