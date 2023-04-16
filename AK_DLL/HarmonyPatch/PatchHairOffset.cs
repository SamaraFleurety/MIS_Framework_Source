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
    /*[HarmonyPatch(typeof(Graphic_Multi), "Init")]
    public class testp
    {
        [HarmonyPrefix]
        public static void prefix(GraphicRequest req)
        {
            Log.Message(req.path + "south");
        }
    }

    [HarmonyPatch(typeof(PawnRenderer), "DrawHeadHair")]
    public class testp
    {
        [HarmonyPrefix]
        public static bool Prefix(PawnRenderer __instance, Vector3 rootLoc, Vector3 headOffset, float angle, Rot4 bodyFacing, Rot4 headFacing, RotDrawMode bodyDrawType, PawnRenderFlags flags, bool bodyDrawn)
        {
            Traverse instance = new Traverse(__instance);
            Pawn pawn = instance.Field("pawn").GetValue<Pawn>();
            Vector3 onHeadLoc = rootLoc + headOffset;
            onHeadLoc.y += 0.0289575271f;
            List<ApparelGraphicRecord> apparelGraphics = __instance.graphics.apparelGraphics;
            List<GeneGraphicRecord> geneGraphics = __instance.graphics.geneGraphics;
            Quaternion quat = Quaternion.AngleAxis(angle, Vector3.up);
            int num;
            if (!pawn.DevelopmentalStage.Baby() && bodyDrawType != RotDrawMode.Dessicated)
            {
                num = (flags.FlagSet(PawnRenderFlags.HeadStump) ? 1 : 0);
                if (num == 0)
                {
                    goto IL_0103;
                }
            }
            else
            {
                num = 1;
            }
            if (pawn.story?.hairDef == null)
            {
                goto IL_0103;
            }
            int num2 = (pawn.story.hairDef.noGraphic ? 1 : 0);
            goto IL_0104;
        IL_0103:
            num2 = 1;
            goto IL_0104;
        IL_0104:
            bool flag = (byte)num2 != 0;
            bool flag2 = num == 0 && bodyFacing != Rot4.North && pawn.DevelopmentalStage.Adult() && (pawn.style?.beardDef ?? BeardDefOf.NoBeard) != BeardDefOf.NoBeard;
            bool allFaceCovered = false;
            bool drawEyes = true;
            bool middleFaceCovered = false;
            bool flag3 = pawn.CurrentBed() != null && !pawn.CurrentBed().def.building.bed_showSleeperBody;
            bool flag4 = !flags.FlagSet(PawnRenderFlags.Portrait) && flag3;
            bool flag5 = flags.FlagSet(PawnRenderFlags.Headgear) && (!flags.FlagSet(PawnRenderFlags.Portrait) || !Prefs.HatsOnlyOnMap || flags.FlagSet(PawnRenderFlags.StylingStation));
            if (instance.Field("leftEyeCached").GetValue() == null)
            {
                instance.Field("leftEyeCached").SetValue(pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord p) => p.woundAnchorTag == "LeftEye"));
            }
            if (instance.Field("rightEyeCached").GetValue() == null)
            {
                instance.Field("rightEyeCached").SetValue(pawn.def.race.body.AllParts.FirstOrDefault((BodyPartRecord p) => p.woundAnchorTag == "RightEye"));
            }
            bool hasLeftEye = instance.Field("leftEyeCached").GetValue() != null && !pawn.health.hediffSet.PartIsMissing(instance.Field("leftEyeCached").GetValue<BodyPartRecord>());
            bool hasRightEye = instance.Field("rightEyeCached").GetValue() != null && !pawn.health.hediffSet.PartIsMissing(instance.Field("rightEyeCached").GetValue<BodyPartRecord>());
            if (flag5)
            {
                for (int i = 0; i < apparelGraphics.Count; i++)
                {
                    if ((flag4 && !apparelGraphics[i].sourceApparel.def.apparel.hatRenderedFrontOfFace) || (apparelGraphics[i].sourceApparel.def.apparel.LastLayer != ApparelLayerDefOf.Overhead && apparelGraphics[i].sourceApparel.def.apparel.LastLayer != ApparelLayerDefOf.EyeCover))
                    {
                        continue;
                    }
                    if (apparelGraphics[i].sourceApparel.def.apparel.bodyPartGroups.Contains(BodyPartGroupDefOf.FullHead))
                    {
                        flag2 = false;
                        allFaceCovered = true;
                        if (!apparelGraphics[i].sourceApparel.def.apparel.forceEyesVisibleForRotations.Contains(headFacing.AsInt))
                        {
                            drawEyes = false;
                        }
                    }
                    if (!apparelGraphics[i].sourceApparel.def.apparel.hatRenderedFrontOfFace && !apparelGraphics[i].sourceApparel.def.apparel.forceRenderUnderHair)
                    {
                        flag = false;
                    }
                    if (apparelGraphics[i].sourceApparel.def.apparel.coversHeadMiddle)
                    {
                        middleFaceCovered = true;
                    }
                }
            }
            TryDrawGenes(GeneDrawLayer.PostSkin);
            if (ModsConfig.IdeologyActive && __instance.graphics.faceTattooGraphic != null && bodyDrawType != RotDrawMode.Dessicated && !flags.FlagSet(PawnRenderFlags.HeadStump) && (bodyFacing != Rot4.North || pawn.style.FaceTattoo.visibleNorth))
            {
                Vector3 loc = rootLoc + headOffset;
                loc.y += 0.0231660213f;
                if (bodyFacing == Rot4.North)
                {
                    loc.y -= 0.001f;
                }
                else
                {
                    loc.y += 0.001f;
                }
                GenDraw.DrawMeshNowOrLater(__instance.graphics.HairMeshSet.MeshAt(headFacing), loc, quat, __instance.graphics.faceTattooGraphic.MatAt(headFacing), flags.FlagSet(PawnRenderFlags.DrawNow));
            }
            TryDrawGenes(GeneDrawLayer.PostTattoo);
            if (headFacing != Rot4.North && (!allFaceCovered || drawEyes))
            {
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff.def.eyeGraphicSouth != null && hediff.def.eyeGraphicEast != null)
                    {
                        GraphicData graphicData = (headFacing.IsHorizontal ? hediff.def.eyeGraphicEast : hediff.def.eyeGraphicSouth);
                        bool flag6 = hediff.Part.woundAnchorTag == "LeftEye";
                        DrawExtraEyeGraphic(graphicData.Graphic, hediff.def.eyeGraphicScale * pawn.ageTracker.CurLifeStage.eyeSizeFactor.GetValueOrDefault(1f), 0.0014f, flag6, !flag6);
                    }
                }
            }
            if (flag2)
            {
                Vector3 loc2 = instance.Property("OffsetBeardLocationForHead").GetValue<Vector3>(pawn.style.beardDef, pawn.story.headType, headFacing, rootLoc + headOffset);
                Mesh mesh = __instance.graphics.BeardMeshSet.MeshAt(headFacing);
                Material material = __instance.graphics.BeardMatAt(headFacing, flags.FlagSet(PawnRenderFlags.Portrait), flags.FlagSet(PawnRenderFlags.Cache));
                if (material != null)
                {
                    GenDraw.DrawMeshNowOrLater(mesh, loc2, quat, material, flags.FlagSet(PawnRenderFlags.DrawNow));
                }
            }
            if (flag5)
            {
                for (int j = 0; j < apparelGraphics.Count; j++)
                {
                    if ((!flag4 || apparelGraphics[j].sourceApparel.def.apparel.hatRenderedFrontOfFace) && apparelGraphics[j].sourceApparel.def.apparel.forceRenderUnderHair)
                    {
                        DrawApparel(apparelGraphics[j]);
                    }
                }
            }
            if (flag)
            {
                Mesh mesh2 = __instance.graphics.HairMeshSet.MeshAt(headFacing);
                Material material2 = __instance.graphics.HairMatAt(headFacing, flags.FlagSet(PawnRenderFlags.Portrait), flags.FlagSet(PawnRenderFlags.Cache));
                if (material2 != null)
                {
                    LocalBuilder a;
                    a.LocalType
                    onHeadLoc.x += 0.2f;
                    onHeadLoc.y += 0.2f;
                    GenDraw.DrawMeshNowOrLater(mesh2, onHeadLoc, quat, material2, flags.FlagSet(PawnRenderFlags.DrawNow));
                    onHeadLoc.x -= 0.2f;
                    onHeadLoc.y -= 0.2f;
                }
            }
            TryDrawGenes(GeneDrawLayer.PostHair);
            if (flag5)
            {
                for (int k = 0; k < apparelGraphics.Count; k++)
                {
                    if ((!flag4 || apparelGraphics[k].sourceApparel.def.apparel.hatRenderedFrontOfFace) && (apparelGraphics[k].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.Overhead || apparelGraphics[k].sourceApparel.def.apparel.LastLayer == ApparelLayerDefOf.EyeCover) && !apparelGraphics[k].sourceApparel.def.apparel.forceRenderUnderHair)
                    {
                        DrawApparel(apparelGraphics[k]);
                    }
                }
            }
            TryDrawGenes(GeneDrawLayer.PostHeadgear);
            void DrawApparel(ApparelGraphicRecord apparelRecord)
            {
                Mesh mesh3 = __instance.graphics.HairMeshSet.MeshAt(headFacing);
                if (!apparelRecord.sourceApparel.def.apparel.hatRenderedFrontOfFace)
                {
                    Material material3 = apparelRecord.graphic.MatAt(bodyFacing);
                    material3 = (flags.FlagSet(PawnRenderFlags.Cache) ? material3 : instance.Property("OverrideMaterialIfNeeded").GetValue<Material>(material3, pawn, flags.FlagSet(PawnRenderFlags.Portrait)));
                    GenDraw.DrawMeshNowOrLater(mesh3, onHeadLoc, quat, material3, flags.FlagSet(PawnRenderFlags.DrawNow));
                }
                else
                {
                    Material material4 = apparelRecord.graphic.MatAt(bodyFacing);
                    material4 = (flags.FlagSet(PawnRenderFlags.Cache) ? material4 : instance.Property("OverrideMaterialIfNeeded").GetValue<Material>(material4, pawn, flags.FlagSet(PawnRenderFlags.Portrait)));
                    Vector3 loc3 = rootLoc + headOffset;
                    if (apparelRecord.sourceApparel.def.apparel.hatRenderedBehindHead)
                    {
                        loc3.y += 0.0221660212f;
                    }
                    else
                    {
                        loc3.y += ((bodyFacing == Rot4.North && !apparelRecord.sourceApparel.def.apparel.hatRenderedAboveBody) ? 0.00289575267f : 0.03185328f);
                    }
                    GenDraw.DrawMeshNowOrLater(mesh3, loc3, quat, material4, flags.FlagSet(PawnRenderFlags.DrawNow));
                }
            }
            void DrawExtraEyeGraphic(Graphic graphic, float scale, float yOffset, bool drawLeft, bool drawRight)
            {
                bool narrowCrown = pawn.story.headType.narrow;
                Vector3? eyeOffsetEastWest = pawn.story.headType.eyeOffsetEastWest;
                Vector3 vector = rootLoc + headOffset + new Vector3(0f, 0.0260617733f + yOffset, 0f) + quat * new Vector3(0f, 0f, -0.25f);
                BodyTypeDef.WoundAnchor woundAnchor = pawn.story.bodyType.woundAnchors.FirstOrDefault((BodyTypeDef.WoundAnchor a) => a.tag == "LeftEye" && a.rotation == headFacing && (headFacing == Rot4.South || a.narrowCrown.GetValueOrDefault() == narrowCrown));
                BodyTypeDef.WoundAnchor woundAnchor2 = pawn.story.bodyType.woundAnchors.FirstOrDefault((BodyTypeDef.WoundAnchor a) => a.tag == "RightEye" && a.rotation == headFacing && (headFacing == Rot4.South || a.narrowCrown.GetValueOrDefault() == narrowCrown));
                Material mat = graphic.MatAt(headFacing);
                if (headFacing == Rot4.South)
                {
                    if (woundAnchor == null || woundAnchor2 == null)
                    {
                        return;
                    }
                    if (drawLeft)
                    {
                        GenDraw.DrawMeshNowOrLater(MeshPool.GridPlaneFlip(Vector2.one * scale), Matrix4x4.TRS(vector + quat * woundAnchor.offset, quat, Vector3.one), mat, flags.FlagSet(PawnRenderFlags.DrawNow));
                    }
                    if (drawRight)
                    {
                        GenDraw.DrawMeshNowOrLater(MeshPool.GridPlane(Vector2.one * scale), Matrix4x4.TRS(vector + quat * woundAnchor2.offset, quat, Vector3.one), mat, flags.FlagSet(PawnRenderFlags.DrawNow));
                    }
                }
                if (headFacing == Rot4.East && drawRight)
                {
                    if (woundAnchor2 == null)
                    {
                        return;
                    }
                    Vector3 vector2 = eyeOffsetEastWest ?? woundAnchor2.offset;
                    GenDraw.DrawMeshNowOrLater(MeshPool.GridPlane(Vector2.one * scale), Matrix4x4.TRS(vector + quat * vector2, quat, Vector3.one), mat, flags.FlagSet(PawnRenderFlags.DrawNow));
                }
                if (headFacing == Rot4.West && drawLeft && woundAnchor != null)
                {
                    Vector3 vector3 = woundAnchor.offset;
                    if (eyeOffsetEastWest.HasValue)
                    {
                        vector3 = eyeOffsetEastWest.Value.ScaledBy(new Vector3(-1f, 1f, 1f));
                    }
                    GenDraw.DrawMeshNowOrLater(MeshPool.GridPlaneFlip(Vector2.one * scale), Matrix4x4.TRS(vector + quat * vector3, quat, Vector3.one), mat, flags.FlagSet(PawnRenderFlags.DrawNow));
                }
            }
            void DrawGene(GeneGraphicRecord geneRecord, GeneDrawLayer layer)
            {
                if ((bodyDrawType != RotDrawMode.Dessicated || geneRecord.sourceGene.def.graphicData.drawWhileDessicated) && (!(geneRecord.sourceGene.def.graphicData.drawLoc == GeneDrawLoc.HeadMiddle && allFaceCovered) || geneRecord.sourceGene.def.graphicData.drawIfFaceCovered) && (!(geneRecord.sourceGene.def.graphicData.drawLoc == GeneDrawLoc.HeadMiddle && middleFaceCovered) || geneRecord.sourceGene.def.graphicData.drawIfFaceCovered))
                {
                    Vector3 loc4 = instance.Property("HeadGeneDrawLocation").GetValue<Vector3>(geneRecord.sourceGene.def, pawn.story.headType, headFacing, rootLoc + headOffset, layer);
                    Material material5 = ((bodyDrawType == RotDrawMode.Rotting) ? geneRecord.rottingGraphic : geneRecord.graphic).MatAt(headFacing);
                    material5 = (flags.FlagSet(PawnRenderFlags.Cache) ? material5 : instance.Property("OverrideMaterialIfNeeded").GetValue<Material>(material5, pawn, flags.FlagSet(PawnRenderFlags.Portrait)));
                    GenDraw.DrawMeshNowOrLater(__instance.graphics.HairMeshSet.MeshAt(headFacing), loc4, quat, material5, flags.FlagSet(PawnRenderFlags.DrawNow));
                }
            }
            void DrawGeneEyes(GeneGraphicRecord geneRecord)
            {
                if (!(headFacing == Rot4.North) && (bodyDrawType != RotDrawMode.Dessicated || geneRecord.sourceGene.def.graphicData.drawWhileDessicated) && (!(geneRecord.sourceGene.def.graphicData.drawLoc == GeneDrawLoc.HeadMiddle && allFaceCovered) || geneRecord.sourceGene.def.graphicData.drawIfFaceCovered || drawEyes))
                {
                    Graphic graphic2 = ((bodyDrawType == RotDrawMode.Rotting) ? geneRecord.rottingGraphic : geneRecord.graphic);
                    float drawScale = geneRecord.sourceGene.def.graphicData.drawScale;
                    DrawExtraEyeGraphic(graphic2, drawScale * pawn.ageTracker.CurLifeStage.eyeSizeFactor.GetValueOrDefault(1f), 0.0012f, hasLeftEye, hasRightEye);
                }
            }
            void TryDrawGenes(GeneDrawLayer layer)
            {
                if (ModLister.BiotechInstalled && !flags.FlagSet(PawnRenderFlags.HeadStump))
                {
                    for (int l = 0; l < geneGraphics.Count; l++)
                    {
                        if (geneGraphics[l].sourceGene.def.CanDrawNow(bodyFacing, layer))
                        {
                            if (geneGraphics[l].sourceGene.def.graphicData.drawOnEyes)
                            {
                                DrawGeneEyes(geneGraphics[l]);
                            }
                            else
                            {
                                DrawGene(geneGraphics[l], layer);
                            }
                        }
                    }
                }
            }
            return false;
        }
    }


    */
    //转译器实验
    [HarmonyPatch(typeof(PawnRenderer), "DrawHeadHair")]

    public static class PatchHairOffset
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool foundHairRenderer = false;
            int[] branchs = new int[100];
            int bPtr = 0;
            int drawLocAfterBranchs = -1;

            List<CodeInstruction> codes = new List<CodeInstruction>(instructions);

            //var materialIndex = codes.FindIndex(code => code.opcode == OpCodes.Ldloc_S && code.operand is LocalBuilder localBuilder && localBuilder.LocalIndex == 27); 
            var materialIndex = codes.FindIndex(code => code.opcode == OpCodes.Ldloc_S && ((LocalBuilder)code.operand).LocalIndex == 18);
            if (materialIndex == -1)
            {
                Log.Error("Cannot find local variable material2 in PawnRenderer.DrawHeadHair transpiler.");
            }
            else
            {
                LocalBuilder lb = codes[materialIndex].operand as LocalBuilder;
                Log.Warning($"TARGET: {lb == null}");
                Log.Warning($"TARGET2: {(lb.LocalType == typeof(Material))}");
            }
            return codes.AsEnumerable();
            /*Log.Message("start");
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
            }
                }*/
            Log.Message("END");
            return codes;
        }
    }
}
