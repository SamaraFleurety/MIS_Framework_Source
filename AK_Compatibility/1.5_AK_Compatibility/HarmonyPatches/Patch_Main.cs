using HarmonyLib;
using System;
using System.Reflection;
using Verse;

namespace PA_AKPatch
{
    [StaticConstructorOnStartup]
    public static class AKPatches
    {
        public static Type[] FacialAnimation => Assembly.Load("FacialAnimation")?.GetTypes();
        public static Type[] FashionWardrobe => Assembly.Load("Fashion Wardrobe")?.GetTypes();

        static AKPatches()
        {
            //手动Patch才可以调用MakeByRefType
            Harmony harmony = new("paluto22.ak.compatibility");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("[Arknights Compability] Initialized");
        }
    }
}
