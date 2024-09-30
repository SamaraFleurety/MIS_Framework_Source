using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    internal class DoBarSetting_Window : Window
    {
        public override Vector2 InitialSize => new Vector2(800f, 800f);
        private static string ValueSpacing => "  ";
        public enum Option_TimeDisplay
        {
            None = 0,
            DeathTimeOnly = 1,
            BleedRateOnly = 2,
            Both = 3
        }
        public DoBarSetting_Window()
        {
            forcePause = false;
            absorbInputAroundWindow = false;
            closeOnCancel = true;
            soundAppear = SoundDefOf.CommsWindow_Open;
            soundClose = SoundDefOf.CommsWindow_Close;
            doCloseButton = false;
            doCloseX = true;
            draggable = true;
            drawShadow = false;
            preventCameraMotion = false;
            onlyOneOfTypeAllowed = true;
            resizeable = false;
        }
        public override void DoWindowContents(Rect inRect)
        {
            DoOptions(inRect);
        }

        public static void DoOptions(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard
            {
                ColumnWidth = inRect.width / 2f - 20f
            };
            list.Begin(inRect);
            list.Gap(12f);
            Text.Font = GameFont.Medium;
            list.Label("PawnHealthBar_Option_TitleSwitches".Translate());
            Text.Font = GameFont.Small;
            list.GapLine();
            list.CheckboxLabeled(AK_ModSettings.displayBar ? "PawnHealthBar_displayBar".Translate() + " " + "PawnHealthBar_Option_Expanded".Translate() : "PawnHealthBar_displayBar".Translate() + " " + "PawnHealthBar_Option_Folded".Translate(), ref AK_ModSettings.displayBar, "PawnHealthBar_displayBarD".Translate());
            if (AK_ModSettings.displayBar)
            {
                Rect faction = list.GetRect(inRect.width / 24f, 0.5f);
                faction.height -= 4f;
                Widgets.CheckboxLabeled(faction, "PawnHealthBar_displayAllyFaction".Translate(), ref AK_ModSettings.display_AllyFaction);
                if (AK_ModSettings.display_AllyFaction)
                {
                    Rect faction1 = faction;
                    faction1.x += faction1.width;
                    Widgets.CheckboxLabeled(faction1, "PawnHealthBar_Option_InjuryedOnly".Translate(), ref AK_ModSettings.display_AllyFaction_InjuryedOnly);
                }
                //
                Rect faction2 = faction;
                faction2.y += faction.height;
                Widgets.CheckboxLabeled(faction2, "PawnHealthBar_displayNeutralFaction".Translate(), ref AK_ModSettings.display_NeutralFaction);
                if (AK_ModSettings.display_NeutralFaction)
                {
                    Rect faction3 = faction2;
                    faction3.x += faction3.width;
                    Widgets.CheckboxLabeled(faction3, "PawnHealthBar_Option_InjuryedOnly".Translate(), ref AK_ModSettings.display_NeutralFaction_InjuryedOnly);
                }

                list.GetRect((inRect.width / 24f) - 4f);
                list.Gap(4f);
                list.CheckboxLabeled(AK_ModSettings.display_PlayerFaction ? "PawnHealthBar_displayPlayerFaction".Translate() + " " + "PawnHealthBar_Option_Expanded".Translate() : "PawnHealthBar_displayPlayerFaction".Translate() + " " + "PawnHealthBar_Option_Folded".Translate(), ref AK_ModSettings.display_PlayerFaction, "PawnHealthBar_displayPlayerFactionD".Translate());
                if (AK_ModSettings.display_PlayerFaction)
                {
                    list.CheckboxLabeled(AK_ModSettings.display_Colonist ? "PawnHealthBar_display_Colonist".Translate() + " " + "PawnHealthBar_Option_Expanded".Translate() : "PawnHealthBar_display_Colonist".Translate() + " " + "PawnHealthBar_Option_Folded".Translate(), ref AK_ModSettings.display_Colonist, "PawnHealthBar_display_ColonistD".Translate());
                    if (AK_ModSettings.display_Colonist)
                    {
                        Rect rect1 = list.GetRect(inRect.width / 24f, 0.5f);
                        rect1.height -= 4f;
                        Widgets.CheckboxLabeled(rect1, "PawnHealthBar_display_OnDraftedOnly".Translate(), ref AK_ModSettings.display_OnDraftedOnly);
                        Rect rect2 = rect1;
                        rect2.x += rect2.width;
                        Widgets.CheckboxLabeled(rect2, "PawnHealthBar_Option_InjuryedOnly".Translate(), ref AK_ModSettings.display_Colonist_InjuryedOnly);

                    }
                    list.Gap(4f);
                    Rect animal1 = list.GetRect(inRect.width / 24f, 0.5f);
                    animal1.height -= 4f;
                    Widgets.CheckboxLabeled(animal1, "PawnHealthBar_display_ColonyAnimal".Translate(), ref AK_ModSettings.display_ColonyAnimal);
                    if (AK_ModSettings.display_ColonyAnimal)
                    {
                        Rect animal2 = animal1;
                        animal2.x += animal2.width;
                        Widgets.CheckboxLabeled(animal2, "PawnHealthBar_Option_InjuryedOnly".Translate(), ref AK_ModSettings.display_ColonyAnimal_InjuryedOnly);
                    }
                    //list.Gap(4f);
                    Rect mech1 = list.GetRect(inRect.width / 24f, 0.5f);
                    mech1.height -= 4f;
                    Widgets.CheckboxLabeled(mech1, "PawnHealthBar_display_ColonyMech".Translate(), ref AK_ModSettings.display_ColonyMech);
                    if (AK_ModSettings.display_ColonyMech)
                    {
                        Rect mech2 = mech1;
                        mech2.x += mech2.width;
                        Widgets.CheckboxLabeled(mech2, "PawnHealthBar_Option_InjuryedOnly".Translate(), ref AK_ModSettings.display_ColonyMech_InjuryedOnly);
                    }
                }
                list.Gap(8f);
                Rect enemy1 = list.GetRect(inRect.width / 24f, 0.5f);
                enemy1.height -= 4f;
                Widgets.CheckboxLabeled(enemy1, "PawnHealthBar_display_Enemy".Translate(), ref AK_ModSettings.display_Enemy);
                if (AK_ModSettings.display_Enemy)
                {
                    Rect enemy2 = enemy1;
                    enemy2.x += enemy2.width;
                    Widgets.CheckboxLabeled(enemy2, "PawnHealthBar_Option_InjuryedOnly".Translate(), ref AK_ModSettings.display_Enemy_InjuryedOnly);
                }
            }
            list.Gap(8f);
            list.CheckboxLabeled("PawnHealthBar_disable_displayPawnLabelHealth".Translate(), ref AK_ModSettings.disable_displayPawnLabelHealth, "PawnHealthBar_disable_displayPawnLabelHealthD".Translate());
            list.GapLine();
            list.CheckboxLabeled("Enable HH:MM:SS Format", ref AK_ModSettings.display_Option_HHMMSS);
            if (list.ButtonTextLabeled("PawnHealthBar_display_PawnDeathIndicator".Translate(), ("PawnHealthBar_display_PawnDeathIndicator_" + ((Option_TimeDisplay)AK_ModSettings.display_PawnDeathIndicator_Option).ToString()).Translate()))
            {
                FloatMenu window = new FloatMenu(new List<FloatMenuOption>
                {
                    new FloatMenuOption("PawnHealthBar_display_PawnDeathIndicator_None".Translate(), delegate
                    {
                        AK_ModSettings.display_PawnDeathIndicator_Option = 0;
                    }),
                    new FloatMenuOption("PawnHealthBar_display_PawnDeathIndicator_DeathTimeOnly".Translate(), delegate
                    {
                        AK_ModSettings.display_PawnDeathIndicator_Option = 1;
                    }),
                    new FloatMenuOption("PawnHealthBar_display_PawnDeathIndicator_BleedRateOnly".Translate(), delegate
                    {
                        AK_ModSettings.display_PawnDeathIndicator_Option = 2;
                    }),
                    new FloatMenuOption("PawnHealthBar_display_PawnDeathIndicator_Both".Translate(), delegate
                    {
                        AK_ModSettings.display_PawnDeathIndicator_Option = 3;
                    })
                });
                Find.WindowStack.Add(window);
            }
            list.NewColumn();
            list.Gap(12f);
            Text.Font = GameFont.Medium;
            list.Label("PawnHealthBar_Option_TitlePositon".Translate());
            Text.Font = GameFont.Small;
            list.GapLine();
            AK_ModSettings.barWidth = (int)list.SliderLabeled("PawnHealthBar_Option_barWidth".Translate() + ValueSpacing + $"{AK_ModSettings.barWidth}", AK_ModSettings.barWidth, 50, 300);
            AK_ModSettings.barHeight = (int)list.SliderLabeled("PawnHealthBar_Option_barHeight".Translate() + ValueSpacing + $"{AK_ModSettings.barHeight * 0.1f}", AK_ModSettings.barHeight, 50, 200);
            AK_ModSettings.barMargin = (int)list.SliderLabeled("PawnHealthBar_Option_barMargin".Translate() + ValueSpacing + $"{AK_ModSettings.barMargin}", AK_ModSettings.barMargin, -150, 150);
            list.CheckboxLabeled("PawnHealthBar_Option_zoomWithCamera".Translate(), ref AK_ModSettings.zoomWithCamera, "PawnHealthBar_Option_zoomWithCameraD".Translate());
            list.Gap(12f);
            //SubTitle
            Text.Font = GameFont.Medium;
            list.Label("PawnHealthBar_Option_TitleRGB".Translate());
            Text.Font = GameFont.Small;
            list.GapLine();
            list.Gap(4f);
            Rect rectLabel = list.GetRect(12f, 1f);
            rectLabel.height *= 2;
            Widgets.Label(rectLabel, "PawnHealthBar_Label_Colony".Translate());
            //颜色1
            Rect rectColor1 = list.GetRect(12f, 0.5f);
            rectColor1.x += rectColor1.width;
            rectColor1.y -= 8f;
            Widgets.DrawBoxSolid(new Rect(rectColor1), AK_ModSettings.Color_RGB);
            AK_ModSettings.r = (int)list.SliderLabeled("PawnHealthBar_Option_r".Translate() + ValueSpacing + $"{AK_ModSettings.r}", AK_ModSettings.r, 0, 255);
            AK_ModSettings.g = (int)list.SliderLabeled("PawnHealthBar_Option_g".Translate() + ValueSpacing + $"{AK_ModSettings.g}", AK_ModSettings.g, 0, 255);
            AK_ModSettings.b = (int)list.SliderLabeled("PawnHealthBar_Option_b".Translate() + ValueSpacing + $"{AK_ModSettings.b}", AK_ModSettings.b, 0, 255);
            AK_ModSettings.a = (int)list.SliderLabeled("PawnHealthBar_Option_a".Translate() + ValueSpacing + $"{AK_ModSettings.a}", AK_ModSettings.a, 0, 255);
            list.Gap(6f);
            //颜色2
            Rect rectLabel2 = list.GetRect(12f, 1f);
            rectLabel2.height *= 2;
            Widgets.Label(rectLabel2, "PawnHealthBar_Label_Enemy".Translate());
            Rect rectColor2 = list.GetRect(12f, 0.5f);
            //rectColor2.height += 8f;
            rectColor2.x += rectColor2.width;
            rectColor2.y -= 8f;
            Widgets.DrawBoxSolid(new Rect(rectColor2), AK_ModSettings.Color_RGB_enemy);
            AK_ModSettings.r_enemy = (int)list.SliderLabeled("PawnHealthBar_Option_r_enemy".Translate() + ValueSpacing + $"{AK_ModSettings.r_enemy}", AK_ModSettings.r_enemy, 0, 255);
            AK_ModSettings.g_enemy = (int)list.SliderLabeled("PawnHealthBar_Option_g_enemy".Translate() + ValueSpacing + $"{AK_ModSettings.g_enemy}", AK_ModSettings.g_enemy, 0, 255);
            AK_ModSettings.b_enemy = (int)list.SliderLabeled("PawnHealthBar_Option_b_enemy".Translate() + ValueSpacing + $"{AK_ModSettings.b_enemy}", AK_ModSettings.b_enemy, 0, 255);
            AK_ModSettings.a_enemy = (int)list.SliderLabeled("PawnHealthBar_Option_a_enemy".Translate() + ValueSpacing + $"{AK_ModSettings.a_enemy}", AK_ModSettings.a_enemy, 0, 255);

            list.End();
        }
    }
}
