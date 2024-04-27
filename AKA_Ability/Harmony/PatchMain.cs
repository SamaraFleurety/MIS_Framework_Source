using System.Reflection;
using HarmonyLib;
using Verse;

namespace AKA_Ability
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            var instance = new Harmony("AKAbility");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}