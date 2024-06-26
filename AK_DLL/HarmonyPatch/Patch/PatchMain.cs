﻿using System.Reflection;
using HarmonyLib;
using Verse;
using AlienRace;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            Harmony instance = new Harmony("AK_DLL");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}