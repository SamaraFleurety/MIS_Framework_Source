using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_SpineExtention
{
    [StaticConstructorOnStartup]
    public class Patch_Main
    {
        static Patch_Main()
        {
            Harmony instance = new("AK_SpineExtention");
            instance.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("AK_SpineExtention Loaded");
        }
    }
}
