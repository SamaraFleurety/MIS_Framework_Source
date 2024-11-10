using HarmonyLib;
using Verse;

namespace AKA_Ability.HarmonyPatchs
{
    //挨打回sp
    [HarmonyPatch(typeof(Pawn), "PreApplyDamage")]
    public class Patch_PawnStricken
    {
        [HarmonyPrefix]
        public static void prefix(Pawn __instance, ref DamageInfo dinfo)
        {
            if (AKA_Utilities.pawn_NotifyStricken.ContainsKey(__instance))
            {
                foreach (AKAbility_Base a in AKA_Utilities.pawn_NotifyStricken[__instance])
                {
                    a.Notify_OwnerStricken(ref  dinfo);
                }
            }
        }
    }
}