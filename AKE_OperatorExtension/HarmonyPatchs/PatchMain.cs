using HarmonyLib;
using System.Reflection;
using Verse;

namespace AKE_OperatorExtension.HarmonyPatchs
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            Harmony instance = new Harmony("AKE_Operator");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}