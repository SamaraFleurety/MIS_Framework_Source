using HarmonyLib;
using RimWorld;
using System;
using Verse;
using System.Reflection;
using AK_DLL;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Paluto22.AK.Patch
{
    [StaticConstructorOnStartup]
    public static class AKPatches
    {
        private static readonly Type patchType = typeof(AKPatches);
        static AKPatches()
        {
            //手动Patch才可以调用MakeByRefType
            Harmony harmony = new Harmony("paluto22.ak.compatibility");
            /*harmony.Patch(AccessTools.Method(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new[] { typeof(PawnGenerationRequest) }),
                 prefix: new HarmonyMethod(patchType, nameof(NewGeneratePawn_Prefix)));*/
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Log.Message("[Arknights Compability] Initialized");
            //
            //harmony.Patch(AccessTools.Method(typeof(OperatorDef), nameof(OperatorDef.Recruit), new[] { typeof(IntVec3), typeof(Map) }), postfix: new HarmonyMethod(patchType, nameof(Recruit_Postfix)));
            //动态表情
            #region 
            if (ModLister.GetActiveModWithIdentifier("Nals.FacialAnimation") != null)
            {
                Assembly FacialAnimation = Type.GetType("FacialAnimation.FacialAnimationMod, FacialAnimation")?.Assembly;
                Type type = FacialAnimation?.GetTypes().FirstOrDefault(t => t.Name == "DrawFaceGraphicsComp");

                MethodBase method = type?.GetMethod("CompRenderNodes");
                if (method != null)
                {
                    harmony.Patch(method, postfix: new HarmonyMethod(patchType, nameof(NewPawnRender_Postfix)));
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
        //动态表情Post方法
        /*public static void NewGeneratePawn_Prefix(ref PawnGenerationRequest request)
        {
            if (OperatorDef.currentlyGenerating == false && !ModsConfig.IsActive("erdelf.HumanoidAlienRaces"))
            {
                return;
            }
            if (Current.ProgramState == ProgramState.Playing && OperatorDef.currentlyGenerating && !AKC_ModSettings.disable_PawnKindDef)
            {
                request.KindDef = PawnKindDefOf.Colonist;
            }
        }
        public static void Recruit_Postfix(IntVec3 intVec, Map map, Pawn ___operator_Pawn)
        {
            Apparel apparel = ___operator_Pawn.apparel.WornApparel.Find(cloth => cloth.def.defName.StartsWith("AK_Apparel"));
            if (apparel != null)
            {
                TCP_HideBody hidebody = apparel.GetComp<TC_HideBody>().props as TCP_HideBody;
                if (AKC_ModSettings.disable_HideHead)
                {
                    if (hidebody.hideHead)
                    {
                        hidebody.hideHead = false;
                    }

                    if (Patch_TCPHideHead.registeredPawns.Contains(___operator_Pawn))
                    {
                        Patch_TCPHideHead.registeredPawns.Remove(___operator_Pawn);
                        Log.Message("[Arknights-Compatibility] " + ___operator_Pawn.Label + " SFLib.TC_HideHead " + hidebody.hideHead.ToString());
                    }
                }
                if (AKC_ModSettings.disable_HideBody)
                {
                    if (hidebody.hideBody)
                    {
                        hidebody.hideBody = false;
                    }

                    if (Patch_TCPHideBody.registeredPawns.Contains(___operator_Pawn))
                    {
                        Patch_TCPHideBody.registeredPawns.Remove(___operator_Pawn);
                        Log.Message("[Arknights-Compatibility] " + ___operator_Pawn.Label + " SFLib.TC_HideBody " + hidebody.hideBody.ToString());
                    }

                }

            }
        }*/
        public static void NewPawnRender_Postfix(ref List<PawnRenderNode> __result, Pawn ___pawn)
        {
            if (AKC_ModSettings.disable_FacialAnimation) return;
            if (___pawn.GetDoc() != null)
            {
                /*string name = ___pawn.GetDoc().operatorDef.defName;
                bool is_AK = name.StartsWith("AK");
                bool is_BA = name.StartsWith("BA");
                if (!is_AK && !is_BA) return;
                if (AKC_ModSettings.MIS_NoFace_Actived && is_AK && !AKC_ModSettings.disable_FacialAnimation_NoFace)
                {
                    return;
                }*/
                OperatorDocument doc = ___pawn.GetDoc();
                if (doc != null) return;
                if (AKC_ModSettings.MIS_NoFace_Actived && !AKC_ModSettings.disable_FacialAnimation_NoFace) 
                {
                    return;
                }
                __result = null;
            }
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
    //种族修复
    [HarmonyPatch(typeof(PawnGenerator), "GeneratePawn", new Type[] { typeof(PawnGenerationRequest) })]
    public class Patch_GeneratePawn 
    {
        [HarmonyPrefix]
        public static void NewGeneratePawn_Prefix(ref PawnGenerationRequest request)
        {
            if (OperatorDef.currentlyGenerating == false && !ModsConfig.IsActive("erdelf.HumanoidAlienRaces")) return;
            if (/*Current.ProgramState == ProgramState.Playing && */OperatorDef.currentlyGenerating && !AKC_ModSettings.disable_PawnKindDef)
            {
                request.KindDef = PawnKindDefOf.Colonist;
            }
        }
    }
    /*
    [HarmonyPatch(typeof(OperatorDef), nameof(OperatorDef.Recruit), new[] { typeof(IntVec3), typeof(Map) })]
    public class Patch_RecruitOp 
    {
        [HarmonyPostfix]
        public static void Recruit_Postfix(IntVec3 intVec, Map map, Pawn ___operator_Pawn)
        {
            Apparel apparel = ___operator_Pawn.apparel.WornApparel.Find(cloth => cloth.def.defName.StartsWith("AK_Apparel"));
            if (apparel != null)
            {
                TCP_HideBody hidebody = apparel.GetComp<TC_HideBody>().props as TCP_HideBody;
                if (AKC_ModSettings.disable_HideHead)
                {
                    if (hidebody.hideHead)
                    {
                        hidebody.hideHead = false;
                    }

                    if (Patch_TCPHideHead.registeredPawns.Contains(___operator_Pawn))
                    {
                        Log.Message("[Arknights-Compatibility] " + ___operator_Pawn.Label + " SFLib.TC_HideHead " + hidebody.hideHead.ToString());
                        Patch_TCPHideHead.registeredPawns.Remove(___operator_Pawn);
                    }
                }
                if (AKC_ModSettings.disable_HideBody)
                {
                    if (hidebody.hideBody)
                    {
                        hidebody.hideBody = false;
                    }

                    if (Patch_TCPHideBody.registeredPawns.Contains(___operator_Pawn))
                    {
                        Log.Message("[Arknights-Compatibility] " + ___operator_Pawn.Label + " SFLib.TC_HideBody " + hidebody.hideBody.ToString());
                        Patch_TCPHideBody.registeredPawns.Remove(___operator_Pawn);
                    }

                }

            }
        }
    }*/
}
