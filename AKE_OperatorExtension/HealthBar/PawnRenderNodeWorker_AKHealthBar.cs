using AK_DLL;
using RimWorld;
using UnityEngine;
using Verse;

namespace AKE_OperatorExtension
{
    //懒得改了能用就行
    public class PawnRenderNodeWorker_AKHealthBar : PawnRenderNodeWorker
    {
        //locOffset
        private static float Width => AK_ModSettings.barWidth * 0.01f;
        private static float Height => AK_ModSettings.barHeight * 0.001f;
        private static float Margin => AK_ModSettings.barMargin * 0.01f;
        private static Vector2 BarSize => new(Width, Height);
        private static Vector3 BottomMargin => new(0f, 0f, Margin);
        //private static Vector3 IconMargin => BottomMargin + Vector3.left * 0.8f;
        //Mat
        private static Material BarFilledMat => AKE_BarUITool.HealthBarFilledMat;
        private static Material BarEnemyFilledMat => AKE_BarUITool.EnemyHealthBarFilledMat;
        private static Material BarUnfilledMat => AKE_BarUITool.BarUnfilledMat;
        private static Material HP_Icon => AKE_BarUITool.HP_Icon;

        private static float GetZoomRatio()
        {
            if (AK_ModSettings.zoomWithCamera)
            {
                return Mathf.Max(Find.CameraDriver.ZoomRootSize, 11) / 11;
            }
            return 1f;
        }

        private void DrawHealthBar(Material mat, Pawn pawn)
        {
            Vector3 drawPos = pawn.DrawPos;
            float percent = pawn.HealthPercentage();
            float zoomRatio = GetZoomRatio();
            float zoomWidthRatio;
            float zoomYRatio;
            if (AK_Mod.CameraPlusModEnabled || AK_Mod.SimpleCameraModEnabled)
            {
                zoomWidthRatio = zoomRatio > 4.35f ? 4.35f : zoomRatio;
                zoomYRatio = zoomRatio > 5f ? 5f : zoomRatio;
            }
            else
            {
                zoomWidthRatio = zoomRatio > 3.75f ? 3.75f : zoomRatio;
                zoomYRatio = zoomRatio > 3f ? 3f : zoomRatio;
            }
            //
            GenDraw.FillableBarRequest fbr = default;
            if (AK_Mod.CameraPlusModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + (BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.9f : zoomYRatio));
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 1.75f ? zoomRatio * 1.5f : zoomRatio;
            }
            else if (AK_Mod.SimpleCameraModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + (BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio));
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 3f ? zoomRatio * 1.05f : zoomRatio;
            }
            else
            {
                fbr.center = drawPos + (Vector3.up * 3f) + (BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio));
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 6.5f ? zoomRatio * 1.25f : zoomRatio;
            }
            fbr.fillPercent = (percent < 0f) ? 0f : percent;
            fbr.filledMat = mat;
            fbr.unfilledMat = BarUnfilledMat;
            fbr.rotation = Rot4.North;
            GenDraw.DrawFillableBar(fbr);
            Vector3 iconPos = new(fbr.center.x - (fbr.size.x / 2) - 0.075f, fbr.center.y, fbr.center.z);
            DrawIcon(iconPos);
        }

        private static void DrawIcon(Vector3 pos)
        {
            Matrix4x4 matrix = default;
            matrix.SetTRS(pos, Rot4.North.AsQuat, new Vector3(0.25f, 1f, 0.25f));
            Graphics.DrawMesh(MeshPool.plane025, matrix, material: HP_Icon, 2);
        }

        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            Pawn pawn = parms.pawn;
            if (!AK_ModSettings.enable_HealthBar || Current.ProgramState != ProgramState.Playing || pawn == null || pawn.Dead)
            {
                return false;
            }
            if (pawn.IsColonist)
            {
                if (AK_ModSettings.display_PlayerFaction && AK_ModSettings.display_Colonist)
                {
                    if (AK_ModSettings.display_Colonist_InjuryedOnly && pawn.HealthPercentage() < 1f)
                    {
                        return true;
                    }
                    if (!AK_ModSettings.display_Colonist_InjuryedOnly && AK_ModSettings.display_OnDraftedOnly && pawn.Drafted)
                    {
                        return true;
                    }
                    if (!AK_ModSettings.display_Colonist_InjuryedOnly && !AK_ModSettings.display_OnDraftedOnly)
                    {
                        return true;
                    }
                }
            }
            if (pawn.Faction == Faction.OfPlayer)
            {
                if (!AK_ModSettings.display_PlayerFaction)
                {
                    return false;
                }
                if (AK_ModSettings.display_ColonyAnimal && AK_ModSettings.display_ColonyAnimal_InjuryedOnly && pawn.RaceProps.Animal && pawn.HealthPercentage() < 1f)
                {
                    return true;
                }
                if (AK_ModSettings.display_ColonyAnimal && !AK_ModSettings.display_ColonyAnimal_InjuryedOnly && pawn.RaceProps.Animal)
                {
                    return true;
                }
                if (AK_ModSettings.display_ColonyMech && AK_ModSettings.display_ColonyMech_InjuryedOnly && pawn.IsColonyMech && pawn.HealthPercentage() < 1f)
                {
                    return true;
                }
                if (AK_ModSettings.display_ColonyMech && !AK_ModSettings.display_ColonyMech_InjuryedOnly && pawn.IsColonyMech)
                {
                    return true;
                }
            }
            if (!pawn.HostileTo(Faction.OfPlayer))
            {
                if (AK_ModSettings.display_AllyFaction && AK_ModSettings.display_AllyFaction_InjuryedOnly && pawn.IsAlly() && pawn.HealthPercentage() < 1f)
                {
                    return true;
                }
                if (AK_ModSettings.display_AllyFaction && !AK_ModSettings.display_AllyFaction_InjuryedOnly && pawn.IsAlly())
                {
                    return true;
                }
                if (AK_ModSettings.display_NeutralFaction && AK_ModSettings.display_NeutralFaction_InjuryedOnly && pawn.IsNeutral() && pawn.HealthPercentage() < 1f)
                {
                    return true;
                }
                if (AK_ModSettings.display_NeutralFaction && !AK_ModSettings.display_NeutralFaction_InjuryedOnly && pawn.IsNeutral())
                {
                    return true;
                }
            }
            if (AK_ModSettings.display_Enemy && pawn.HostileTo(Faction.OfPlayer))
            {
                if (pawn.IsEntity && pawn.InContainerEnclosed)
                {
                    return false;
                }
                if (!AK_ModSettings.display_Enemy_InjuryedOnly)
                {
                    return true;
                }
                if (AK_ModSettings.display_Enemy_InjuryedOnly && pawn.HealthPercentage() < 1f)
                {
                    return true;
                }
            }
            return false;
        }

        public override void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
        {
            Pawn pawn = parms.pawn;
            if (pawn.CarriedBy != null)
            {
                return;
            }
            if (pawn.HostileTo(Faction.OfPlayer))
            {
                DrawHealthBar(BarEnemyFilledMat, pawn);
                return;
            }
            DrawHealthBar(BarFilledMat, pawn);
        }
    }
}
