using HarmonyLib;
using RimWorld;
using System;
using Verse;
using System.Reflection;
using AK_DLL;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection.Emit;
using System.Linq;

namespace Paluto22.AK.Patch
{
    [StaticConstructorOnStartup]
    public static class AKPatches
    {
        private static readonly Type patchType = typeof(AKPatches);
        static AKPatches()
        {
            Harmony harmony = new Harmony("paluto22.ak.compatibility");
            harmony.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) }),
                 prefix: new HarmonyMethod(patchType, nameof(NewGeneratePawn_Prefix)));
            Log.Message("[Arknights-AlienRaces Compability] Initialized");

            //动态表情
            #region 
            if (ModLister.GetActiveModWithIdentifier("Nals.FacialAnimation") != null)
            {
                Assembly FacialAnimation = Type.GetType("FacialAnimation.FacialAnimationMod, FacialAnimation")?.Assembly;
                Type HarmonyPatches = FacialAnimation?.GetTypes().FirstOrDefault(t => t.Name == "HarmonyPatches");

                MethodBase method = HarmonyPatches?.GetMethod("PrefixRenderPawnInternal");
                if (method != null)
                {
                    harmony.Patch(method, transpiler: new HarmonyMethod(patchType, nameof(Transpiler_RenderPawn), new[] { HarmonyPatches }));
                    Log.Message("[Arknights-FacialAnimation Compability] Initialized");
                }
            }
            #endregion
            //时装柜
            #region
            if (ModLister.GetActiveModWithIdentifier("aedbia.fashionwardrobe") != null)
            {
                Assembly Fashion_Wardrobe = Type.GetType("Fashion_Wardrobe.FWSetting, Fashion Wardrobe")?.Assembly;
                Type type = Fashion_Wardrobe?.GetTypes().FirstOrDefault(t => t.FullName == "Fashion_Wardrobe.FW_Windows+SelApparelWindow");
                MethodBase DoWindowContents = type.GetMethod("DoWindowContents");
                MethodBase Close = type.GetMethod("Close");

                if (DoWindowContents != null && Close != null)
                {
                    harmony.Patch(DoWindowContents, prefix: new HarmonyMethod(patchType, nameof(FashionWardrobe_Prefix)));
                    harmony.Patch(Close, prefix: new HarmonyMethod(patchType, nameof(FashionWardrobe_Postfix)));
                    Log.Message("[Arknights-FashionWardrobe Compability] Initialized");
                }
            }
            #endregion
        }
        //方法
        public static void NewGeneratePawn_Prefix(ref PawnGenerationRequest request)
        {
            if (OperatorDef.currentlyGenerating == false && !ModsConfig.IsActive("erdelf.HumanoidAlienRaces"))
            {
                return;
            }
            if (Current.ProgramState == ProgramState.Playing && OperatorDef.currentlyGenerating == true)
            {
                request.KindDef = PawnKindDefOf.Colonist;
            }
        }
        public static IEnumerable<CodeInstruction> Transpiler_RenderPawn(IEnumerable<CodeInstruction> instructions, ILGenerator il, Type HarmonyPatches)
        {
            List<CodeInstruction> codes = instructions.ToList();
            //IL_0007: stsfld
            int index = codes.FindIndex(code => code.opcode == OpCodes.Stsfld) + 1;
            //IL_000c: ldc.i4.0
            Label IL_000c = il.DefineLabel();
            codes[index].labels.Add(IL_000c);
            MethodInfo methodInfo = patchType.GetMethod(nameof(GetDocMethod));
            codes.InsertRange(index, new CodeInstruction[]
            {
                new CodeInstruction(OpCodes.Ldarg, 3),
                new CodeInstruction(OpCodes.Ldind_Ref),
                new CodeInstruction(OpCodes.Call, methodInfo),
                new CodeInstruction(OpCodes.Brfalse_S, IL_000c),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Stsfld, HarmonyPatches.GetField("comp",BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance)),
            });
            return codes;
        }
        public static bool GetDocMethod(this Pawn p)
        {
            OperatorDocument doc = p.GetDoc();
            if (doc == null)
            {
                return false;
            }
            if (doc.operatorDef.defName.StartsWith("AK"))
            {
                return true;
            }
            return false;
        }
        public static void FashionWardrobe_Prefix(Rect inRect)
        {
            if (!OperatorDef.currentlyGenerating)
            {
                OperatorDef.currentlyGenerating = true;
            }
        }
        public static void FashionWardrobe_Postfix(bool doCloseSound = true)
        {
            if (OperatorDef.currentlyGenerating)
            {
                OperatorDef.currentlyGenerating = false;
            }
        }
    }
}
