using HarmonyLib;
using System.Reflection;
using Verse;

//AK_Compatiblity
namespace AKC_AlienRace
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            Harmony instance = new Harmony("AKC_Alien");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
