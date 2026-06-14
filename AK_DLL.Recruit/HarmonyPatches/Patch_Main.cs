using HarmonyLib;
using System.Reflection;
using Verse;

namespace AK_DLL.Recruit
{
    [StaticConstructorOnStartup]
    public class Patch_Main
    {
        static Patch_Main()
        {
            Harmony instance = new("AK_DLL.Recruit");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}