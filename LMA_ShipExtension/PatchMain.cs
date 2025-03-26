using HarmonyLib;
using System.Reflection;
using Verse;

namespace LMA_Lib
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            var instance = new Harmony("LMA_Library");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}