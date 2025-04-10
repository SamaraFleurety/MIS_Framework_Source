using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.HarmonyPatchs
{
    [HarmonyPatch(typeof(Graphic_Single), "Init")]
    public class patchtest
    {
        public static void fix(GraphicRequest req)
        {
            Log.Message($"{req.path}");
        }
    }
}
