using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            var instance = new Harmony("AK_DLL");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}