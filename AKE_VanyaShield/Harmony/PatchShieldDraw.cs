using HarmonyLib;
using System.Reflection;
using Verse;
using VanyaMod;

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
            if(comp != null)
            {
                comp.CompDrawWornExtras();
                return !comp.HideVanillaBubble;
            }
            return true;
        }
    }
}
