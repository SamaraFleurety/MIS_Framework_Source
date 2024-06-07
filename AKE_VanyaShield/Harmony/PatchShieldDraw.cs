using HarmonyLib;
using System.Reflection;
using Verse;
using VanyaMod;
using System.Collections.Generic;
using System.Linq;

namespace AKE_VanyaShield
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            var instance = new Harmony("AKE_VanyaShield");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(Vanya_ShieldBelt), "DrawWornExtras")]
    public class PatchShieldDraw
    {
        [HarmonyPrefix]
        public static bool prefix(Vanya_ShieldBelt __instance)
        {
            TC_VanyaShieldExtension comp = __instance.GetComp<TC_VanyaShieldExtension>();
            if (comp != null)
            {
                comp.CompDrawWornExtras();
                List<TC_ShieldExtraRenderer> extras = __instance.GetComps<TC_ShieldExtraRenderer>().ToList();
                if (!extras.NullOrEmpty())
                {
                    foreach (TC_ShieldExtraRenderer extra in __instance.GetComps<TC_ShieldExtraRenderer>())
                    {
                        extra.CompDrawWornExtras();
                    }
                }
                return !comp.HideVanillaBubble;
            }
            return true;
        }
    }
}
