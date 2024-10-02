using System.Reflection;
using HarmonyLib;
using Verse;
using AlienRace;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            Harmony instance = new Harmony("AK_DLL");
            instance.PatchAll(Assembly.GetExecutingAssembly());
            if (ModLister.GetActiveModWithIdentifier("Nals.FacialAnimation") == null)
            {
                instance.Patch(original: AccessTools.Method(typeof(PawnRenderer), "ParallelGetPreRenderResults"), prefix: new HarmonyMethod(typeof(Patch_PreRenderResults), nameof(Patch_PreRenderResults.Prefix_ParallelGetPreRenderResults)));
            }
        }
    }
}