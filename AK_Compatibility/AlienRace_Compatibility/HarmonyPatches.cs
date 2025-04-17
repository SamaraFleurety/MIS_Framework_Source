using HarmonyLib;
using System.Reflection;
using Verse;

namespace Paluto22.AK.Patch.AlienRace
{
    [StaticConstructorOnStartup]
    public class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("paluto22.alienrace.compatibility");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("[Arknights-AlienRaces Compability] Initialized");
        }
    }
}
