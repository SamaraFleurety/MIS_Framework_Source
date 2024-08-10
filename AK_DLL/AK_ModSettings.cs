using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    //MOD设置 是跨存档全局的 细节没写
    public class AK_ModSettings : ModSettings
    {
        //开启一些正常游戏不会使用的测试功能
        public static bool debugOverride = false;

        //数值条
        public static bool displayBarModel = true;

        //语音
        public static bool playOperatorVoice = true;
        public static int voiceIntervalTime = 2;

        #region 环世界殖民地界面 左下角显示小人立绘
        public static bool displayBottomLeftPortrait = true;
        //显示立绘的偏移参数
        public static int xOffset = 0; //实际效果是值*5
        public static int yOffset = 0; //同上
        public static int ratio = 20;  //实际效果是值/20
        #endregion

        #region 招募UI UGUI
        //主菜单
        public static string secretary = "Amiya";
        public static int secretarySkin = 1;
        public static Vector3 secretaryLoc = new Vector3(400, 0, 1); //(x坐标, y坐标, 缩放倍率)。坐标使用unity体系，即左下角是(0,0)，上右为正。
        public static int secLocSensitive = 1; //调整主界面秘书位置时，每次按钮移动的像素数量；实际效果是值*10
        public static string font = "AK_Font_YouYuan";
        public static FontDef Font
        {
            get
            {
                if (font == null || DefDatabase<FontDef>.GetNamedSilentFail(font) == null) font = "AK_Font_YouYuan";
                return DefDatabase<FontDef>.GetNamedSilentFail(font);
            }
            set { font = value.defName; }
        }

        //记录上次选择的系列以及职业
        public static int lastViewedSeries = -1;
        public static int lastViewedClass = -1;
        #endregion

        //public List<Pawn> exampleListOfPawns = new List<Pawn>();
        //public Dictionary<string, Pawn>;


        public override void ExposeData()
        {
            //自动填充
            Scribe_Values.Look(ref playOperatorVoice, "playVoice");
            Scribe_Values.Look(ref displayBarModel, "displayBar");
            Scribe_Values.Look(ref voiceIntervalTime, "voiceInterTime", 1);
            Scribe_Values.Look(ref displayBottomLeftPortrait, "displayP");
            Scribe_Values.Look(ref xOffset, "xOff");
            Scribe_Values.Look(ref yOffset, "yOff");
            Scribe_Values.Look(ref ratio, "ratio");
            Scribe_Values.Look(ref debugOverride, "dOverride", false);
            Scribe_Values.Look(ref secretary, "secretary", "Amiya", true);
            Scribe_Values.Look(ref secretarySkin, "secSkin", 1);
            Scribe_Values.Look(ref secretaryLoc, "secLoc", new Vector3(400, 0, 1), true);
            Scribe_Values.Look(ref secLocSensitive, "secSense", 1, true);
            Scribe_Values.Look(ref font, "font", "AK_Font_YouYuan", true);
            Scribe_Values.Look(ref lastViewedClass, "lastClass", -1);
            Scribe_Values.Look(ref lastViewedSeries, "lastSeries", -1);
            //Scribe_Collections.Look(ref exampleListOfPawns, "exampleListOfPawns", LookMode.Reference);
        }
    }

    public class AK_Mod : Mod
    {
        public static AK_ModSettings settings;

        public AK_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<AK_ModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            if (Prefs.DevMode) listingStandard.CheckboxLabeled("测试模式", ref AK_ModSettings.debugOverride, "开启明日方舟MOD的测试模式。如果您不是测试人员请勿勾选此选项。");
            listingStandard.CheckboxLabeled("AK_Option_DisplayBar".Translate(), ref AK_ModSettings.displayBarModel, "AK_Option_DisplayBarD".Translate());
            listingStandard.CheckboxLabeled("AK_Option_Play".Translate(), ref AK_ModSettings.playOperatorVoice, "AK_Option_PlayD".Translate()); ;
            AK_ModSettings.voiceIntervalTime = (int)listingStandard.SliderLabeled("AK_Option_Interval".Translate() + $"{(float)AK_ModSettings.voiceIntervalTime / 2.0}", AK_ModSettings.voiceIntervalTime, 0, 60f);

            listingStandard.CheckboxLabeled("AK_Option_DisP".Translate(), ref AK_ModSettings.displayBottomLeftPortrait);
            AK_ModSettings.xOffset = (int)listingStandard.SliderLabeled("AK_Option_xOffset".Translate() + $"{AK_ModSettings.xOffset * 5}", AK_ModSettings.xOffset, 0, 600);
            AK_ModSettings.yOffset = (int)listingStandard.SliderLabeled("AK_Option_yOffset".Translate() + $"{AK_ModSettings.yOffset * 5}", AK_ModSettings.yOffset, 0, 600);
            AK_ModSettings.ratio = (int)listingStandard.SliderLabeled("AK_Option_ratio".Translate() + $"{(float)AK_ModSettings.ratio * 0.05f}", AK_ModSettings.ratio, 1, 40);

            AK_ModSettings.secLocSensitive = (int)listingStandard.SliderLabeled("AK_Option_SecSensetive".Translate() + $"{(float)AK_ModSettings.secLocSensitive * 10}", AK_ModSettings.secLocSensitive, 1, 10);

            if (AK_ModSettings.font == null) AK_ModSettings.Font = AKDefOf.AK_Font_YouYuan;
            List<FontDef> allFontDefs = DefDatabase<FontDef>.AllDefsListForReading;
            if (listingStandard.ButtonTextLabeled("AK_Option_selectFont".Translate(), AK_ModSettings.Font.label.Translate()))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (FontDef i in allFontDefs)
                {
                    FontDef j = i;
                    list.Add(new FloatMenuOption(j.label.Translate(), delegate ()
                    {
                        AK_ModSettings.Font = j;
                    }));
                    Find.WindowStack.Add(new FloatMenu(list));
                }
            }

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "MIS.Arknights";
        }
    }
}
