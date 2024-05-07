using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;
using static Verse.DamageWorker;

namespace AKE_VanyaShield
{
    [HarmonyPatch(typeof(HealthUtility), "SimulateKilled")]
    public class PatchTest
    {
        [HarmonyPrefix]
        public static void prefix(Pawn p, DamageDef damage, ThingDef sourceDef, Tool sourceTool)
        {
            List<Apparel> wornApparel = p.apparel.WornApparel.ToList();
            for (int i = 0; i < wornApparel.Count; ++i)
            {
                if (wornApparel[i] is VanyaMod.Vanya_ShieldBelt)
                {
                    p.apparel.Remove(wornApparel[i]);
                }
            }
        }

        private static IEnumerable<BodyPartRecord> HittablePartsViolence(HediffSet bodyModel)
        {
            return from x in bodyModel.GetNotMissingParts()
                   where x.depth == BodyPartDepth.Outside || (x.depth == BodyPartDepth.Inside && x.def.IsSolid(x, bodyModel.hediffs))
                   select x;
        }
    }

    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public class Patchtkdmg
    {
        [HarmonyPostfix]
        public static void post(DamageInfo dinfo, DamageResult __result)
        {
            if (__result.hediffs == null) __result.hediffs = new List<Hediff>();
        }

    }
}
