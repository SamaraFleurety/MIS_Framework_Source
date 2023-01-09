using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    //转译器实验
    /*[HarmonyPatch(typeof(PawnRenderer), "DrawHeadHair")]

    public static class PatchHairOffset
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool foundHairRenderer = false;
            int[] branchs = new int[100];
            int bPtr = 0;
            int drawLocAfterBranchs = -1;

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            Log.Message("start");
            for (int i = 0; i < codes.Count; ++i)
            {
                if (codes[i].opcode == OpCodes.Ldc_R4 && codes[i].operand is float f && f == 0.0289575271f)
                {
                    Log.Error("ccccc");
                    codes[i].operand = -5.0f;
                    Log.Message(((float)codes[i].operand).ToString());
                }
                /*if (codes[i].opcode == OpCodes.Brfalse_S)
                {
                    branchs[bPtr] = i - 1;
                    ++bPtr;
                    Log.Message("FOUND IF AT: " + i);
                }
                else if (codes[i].opcode == OpCodes.Ldfld /*&& ((int)codes[i].operand) == 17*//*)
                {
                    Log.Warning(codes[i].operand.ToString());
                    if (codes[i].operand is FieldInfo info)
                    {
                        if (info.Name == "onHeadLoc")
                        {
                            Log.Error("true name");
                            Log.Error(info.FieldType.ToString());
                            Log.Error(info.Attributes.ToString());
                        }
                        if (info == AccessTools.Field(typeof(PawnRenderer), "onHeadLoc"))
                        {
                            Log.Warning("found vec3");
                            if (info.Name == "onHeadLoc")
                                Log.Error("!!!found onHeadLoc!!!");
                        }
                    }
                    else Log.Warning("false");
                    if (codes[i].operand is UnityEngine.Mesh)
                    {
                        Mesh m = codes[i].operand as Mesh;
                        Log.Warning(m.ToString());
                        Log.Error("FOUND MESH2 USE AT: " + i);
                        drawLocAfterBranchs = bPtr;
                    }
                }*/
           /* }
            Log.Message("END");
            return codes;
        }
    }*/
}
