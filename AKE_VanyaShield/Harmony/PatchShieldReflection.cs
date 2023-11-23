using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using VanyaMod;
using Verse;

namespace AKE_VanyaShield
{
    [HarmonyPatch(typeof(Vanya_ShieldBelt), "CheckPreAbsorbDamage")]
    public class PatchShieldReflection
    {
        [HarmonyPostfix]
        public static void postfix(DamageInfo dinfo, Vanya_ShieldBelt __instance, ref bool __result)
        {
            if (dinfo.Instigator == null) return;
            if (__result == true)
            {
                TC_VanyaShieldExtension comp = __instance.GetComp<TC_VanyaShieldExtension>();
                if (comp == null) return;
                if (ShouldReflect(dinfo, __instance, comp))
                {
                    dinfo.Instigator.TakeDamage(new DamageInfo(dinfo.Def, dinfo.Amount * comp.ReflectionRatio, dinfo.ArmorPenetrationInt, dinfo.Angle, dinfo.Instigator, dinfo.HitPart));
                }
            }
        }

        private static bool ShouldReflect(DamageInfo info, Vanya_ShieldBelt shield, TC_VanyaShieldExtension comp)
        {
            if (shield.Energy > 0 && comp.ReflectionRatio > 0 && info.Def != DamageDefOf.SurgicalCut && info.Def != DamageDefOf.Extinguish && info.Instigator != null && info.Instigator != shield.Wearer)
            {
                return true;
            }

            return false;
        }
    }
}
