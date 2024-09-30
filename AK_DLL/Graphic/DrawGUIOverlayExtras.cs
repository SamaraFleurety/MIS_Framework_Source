using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public static class DrawGUIOverlayExtras
    {
        public static void AppendPawnGUIOverlayExtras(Pawn pawn)
        {
            DrawBleedLabel(pawn);
        }
        private static void DrawBleedLabel(Pawn pawn)
        {
            if (!AK_ModSettings.Display_PawnDeathIndicator)
            {
                return;
            }
            if (!AK_ModSettings.display_AllyFaction && pawn.IsAlly())
            {
                return;
            }
            if (!AK_ModSettings.display_NeutralFaction && pawn.IsNeutral())
            {
                return;
            }
            if (!AK_ModSettings.display_Enemy && pawn.HostileTo(Faction.OfPlayer))
            {
                return;
            }
            float bleedRateTotal = pawn.health.hediffSet.BleedRateTotal;
            if (bleedRateTotal < 0.01f)
            {
                return;
            }
            Vector2 pos = LabelDrawPosFor(pawn, 0.6f);
            DrawPawnLabel(pawn, pos);
            return;
        }
        private static Vector2 LabelDrawPosFor(Thing thing, float worldOffsetZ)
        {
            Vector3 drawPos = thing.DrawPos;
            drawPos.z += worldOffsetZ;
            Vector2 result = Find.Camera.WorldToScreenPoint(drawPos) / Prefs.UIScale;
            result.y = (float)UI.screenHeight - result.y;
            if (thing is Pawn pawn)
            {
                if (!pawn.RaceProps.Humanlike)
                {
                    result.y += 4f;
                }
                else if (pawn.DevelopmentalStage.Baby())
                {
                    result.y += 8f;
                }
            }
            return result;
        }
        private static void DrawPawnLabel(Pawn pawn, Vector2 pos, float alpha = 1f, GameFont font = GameFont.Tiny, bool alwaysDrawBg = true, bool alignCenter = true)
        {
            float pawnLabelNameWidth = GetPawnLabelTextWidth(pawn, font);
            float num = (Prefs.DisableTinyText ? 6f : 4f);
            float height = (Prefs.DisableTinyText ? 16f : 12f);
            Rect bgRect = new Rect(pos.x - pawnLabelNameWidth / 2f - num, pos.y, pawnLabelNameWidth + num * 2f, height);
            DrawPawnLabel(pawn, bgRect, alpha, font, alwaysDrawBg, alignCenter);
        }
        private static void DrawPawnLabel(Pawn pawn, Rect bgRect, float alpha = 1f, GameFont font = GameFont.Tiny, bool alwaysDrawBg = true, bool alignCenter = true)
        {
            GUI.color = new Color(1f, 1f, 1f, alpha);
            Text.Font = font;
            string pawnLabel = GetPawnLabel(pawn, font);
            float pawnLabelNameWidth = GetPawnLabelTextWidth(pawn, font);
            //背景框材质
            /*if (alwaysDrawBg)
            {
                GUI.DrawTexture(bgRect, TexUI.GrayTextBG);
            }*/
            //字体颜色
            Color color = Color.red;
            color.a = alpha;
            GUI.color = color;
            Rect rect;
            if (alignCenter)
            {
                Text.Anchor = TextAnchor.UpperCenter;
                rect = new Rect(bgRect.center.x - pawnLabelNameWidth / 2f, bgRect.y - 15f, pawnLabelNameWidth, 100f);
            }
            else
            {
                Text.Anchor = TextAnchor.UpperLeft;
                rect = new Rect(bgRect.x + 2f, bgRect.center.y - Text.CalcSize(pawnLabel).y / 2f, pawnLabelNameWidth, 100f);
            }
            Widgets.Label(rect, pawnLabel);
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        private static float GetPawnLabelTextWidth(Pawn pawn, GameFont font)
        {
            GameFont font2 = Text.Font;
            Text.Font = font;
            string pawnLabel = GetPawnLabel(pawn, font);
            float num = ((font != 0) ? Text.CalcSize(pawnLabel).x : pawnLabel.GetWidthCached());
            if (Math.Abs(Math.Round(Prefs.UIScale) - (double)Prefs.UIScale) > 1.401298464324817E-45)
            {
                num += 0.5f;
            }
            if (num < 20f)
            {
                num = 20f;
            }
            Text.Font = font2;
            return num;
        }
        private static string GetPawnLabel(Pawn pawn, GameFont font)
        {
            GameFont font2 = Text.Font;
            Text.Font = font;
            float bleedRateTotal = pawn.health.hediffSet.BleedRateTotal;
            //string result = pawn.LabelShortCap.Truncate(9999f);
            string result = null;
            if (bleedRateTotal > 0.01f)
            {
                //流血率
                string text = string.Concat("BleedingRate".Translate(), ": ", bleedRateTotal.ToStringPercent(), " / ", "Day".Translate());
                int num = HealthUtility.TicksUntilDeathDueToBloodLoss(pawn);
                string text2 = "TimeToDeath".Translate().Formatted(AK_ModSettings.display_Option_HHMMSS ? num.FormatTicksToHHMMSS() : num.ToStringTicksToPeriod());
                if (num >= 60000)
                {
                    result = (string)(text + " (" + "WontBleedOutSoon".Translate() + ")");
                }
                else
                {
                    if (AK_ModSettings.Display_PawnDeathIndicator_DeathTimeOnly)
                    {
                        result = text2;
                    }
                    else if (AK_ModSettings.Display_PawnDeathIndicator_BleedRateOnly)
                    {
                        result = text;
                    }
                    else if (AK_ModSettings.Display_PawnDeathIndicator_Both)
                    {
                        result = text2 + "  (" + text + ")";
                    }
                }
            }
            Text.Font = font2;
            return result;
        }
    }
}
