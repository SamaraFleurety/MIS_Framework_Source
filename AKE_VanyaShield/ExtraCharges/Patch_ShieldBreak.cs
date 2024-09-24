using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanyaMod;
using Verse;

namespace AKE_VanyaShield.HarmonyPatchs
{
    [HarmonyPatch(typeof(Vanya_ShieldBelt), "Break")]
    public class Patch_ShieldBreak
    {
        [HarmonyPostfix]
        public static void postfix(Vanya_ShieldBelt __instance, ref int ___ticksToReset)
        {
            TC_ExtraCharges compCharge = __instance.GetComp<TC_ExtraCharges>();

            if (compCharge != null && compCharge.charges > 0)
            {
                --compCharge.charges;
                ___ticksToReset = -1;
            }
        }
    }
}
