using HarmonyLib;
using System.Reflection;
using Verse;

namespace LMA_Lib
{
    [StaticConstructorOnStartup]
    public class Patch_Main
    {
        static Patch_Main()
        {
            var instance = new Harmony("LMA_Library");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
