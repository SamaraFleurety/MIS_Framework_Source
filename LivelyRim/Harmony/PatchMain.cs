﻿using HarmonyLib;
using System.Reflection;
using Verse;

namespace FS_LivelyRim
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            var instance = new Harmony("FS_LivelyRim");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}