using HarmonyLib;
using System.Reflection;
using Verse;

namespace SFLib
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            var instance = new Harmony("SFLib");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
