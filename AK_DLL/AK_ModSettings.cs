using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    //MOD设置 是跨存档全局的 细节没写
    public class AK_ModSettings : ModSettings
    {
        //开启一些正常游戏不会使用的测试功能
        public static bool debugOverride;

        #region 舟血条
        public static bool enable_HealthBar = true;
        public static bool display_PlayerFaction = true;
        public static bool display_AllyFaction = true;
        public static bool display_AllyFaction_InjuryedOnly = true;
        public static bool display_NeutralFaction = true;
        public static bool display_NeutralFaction_InjuryedOnly = true;
        public static bool display_Colonist = true;
        public static bool display_Colonist_InjuryedOnly;
        public static bool display_ColonyAnimal;
        public static bool display_ColonyAnimal_InjuryedOnly;
        public static bool display_ColonyMech;
        public static bool display_ColonyMech_InjuryedOnly;
        public static bool display_OnDraftedOnly;
        public static bool display_Enemy = true;
        public static bool display_Enemy_InjuryedOnly = true;
        public static bool disable_displayPawnLabelHealth = true;
        //
        public static bool enable_Skillbar = true;
        public static bool display_Skillbar_OnDraftedOnly = true;
        //
        public static int display_PawnDeathIndicator_Option = 1;
        public static bool display_Option_HHMMSS = true;
        public static bool Option_BindingHealthBar = false;
        public static bool Display_PawnDeathIndicator => display_PawnDeathIndicator_Option != 0;
        public static bool Display_PawnDeathIndicator_DeathTimeOnly => display_PawnDeathIndicator_Option == 1;
        public static bool Display_PawnDeathIndicator_BleedRateOnly => display_PawnDeathIndicator_Option == 2;
        public static bool Display_PawnDeathIndicator_Both => display_PawnDeathIndicator_Option == 3;
        //Offset
        public static int barWidth = 150;
        public static int barHeight = 75;
        public static int barMargin = -100;

        public static bool zoomWithCamera = true;
        public static bool drawOutOfCameraZoom = true;
        //RGB
        public static Color32 Color_RGB => new((byte)r, (byte)g, (byte)b, (byte)a);
        public static int r = 105;
        public static int g = 180;
        public static int b = 210;
        public static int a = 200;
        public static Color32 Color_RGB_enemy => new((byte)r_enemy, (byte)g_enemy, (byte)b_enemy, (byte)a_enemy);
        public static int r_enemy = 220;
        public static int g_enemy = 40;
        public static int b_enemy;
        public static int a_enemy = 200;
        #endregion
        //语音
        public static bool playOperatorVoice = true;
        public static int voiceIntervalTime = 2;

        #region 环世界殖民地界面 左下角显示小人立绘
        public static bool displayBottomLeftPortrait = true;
        public static bool displayAnimationLeftPortrait = true;
        //显示立绘的偏移参数
        //fixme:改成属性
        public static int xOffset; //实际效果是值*5
        public static int yOffset; //同上
        public static int ratio = 20;  //实际效果是值/20

        //不透明度 0-100，100就是完全不透，0就是完全透明
        public static int opacity = 100;
        #endregion

        #region 招募UI UGUI
        //主菜单
        public static string secretary = "Amiya";
        public static int secretarySkin = 1;
        public static Vector3 secretaryLoc = new(400, 0, 1); //(x坐标, y坐标, 缩放倍率)。坐标使用unity体系，即左下角是(0,0)，上右为正。
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

        #region 选择性不加载职业
        /*public static List<string> forbiddenLoadClasses = new();
        public static List<string> forbiddenXmls = new(); //禁止包含里面任一string的xml路径的读取
        public static List<string> forbiddenAssets = new(); //禁止包含里面任一string的多媒体文件路径的读取*/
        //在mod选项里面选完不游玩的职业后，需要手动确认
        /*public static void ConfirmForbiddenClasses()
        {
            forbiddenXmls = new();
            forbiddenAssets = new();

            HashSet<string> forbiddenXmlSet = new HashSet<string>();
            HashSet<string> forbiddenAssetSet = new();
            foreach (string defname in forbiddenLoadClasses)
            {
                LoadFolderDef def = DefDatabase<LoadFolderDef>.GetNamedSilentFail(defname);
                forbiddenXmlSet.AddRange(def.xmlFilePaths);
                forbiddenAssetSet.AddRange(def.assetPaths);
            }
            forbiddenXmls = forbiddenXmlSet.ToList();
            forbiddenAssets = forbiddenAssetSet.ToList();
        }*/
        #endregion

        public static bool allowManualRegister;

        public override void ExposeData()
        {
            //自动填充
            Scribe_Values.Look(ref enable_HealthBar, "displayBar", defaultValue: false);
            Scribe_Values.Look(ref display_PlayerFaction, "display_PlayerFaction", defaultValue: true);
            Scribe_Values.Look(ref display_AllyFaction, "display_AllyFaction", defaultValue: false);
            Scribe_Values.Look(ref display_AllyFaction_InjuryedOnly, "display_AllyFaction_InjuryedOnly", defaultValue: false);
            Scribe_Values.Look(ref display_NeutralFaction, "display_NeturalFaction", defaultValue: false);
            Scribe_Values.Look(ref display_NeutralFaction_InjuryedOnly, "display_NeutralFaction_InjuryedOnly", defaultValue: false);
            Scribe_Values.Look(ref display_Colonist, "display_Colonist", defaultValue: true);
            Scribe_Values.Look(ref display_Colonist_InjuryedOnly, "display_ColonistInjuryedOnly", defaultValue: false);
            Scribe_Values.Look(ref display_ColonyAnimal, "display_Animal", defaultValue: false);
            Scribe_Values.Look(ref display_ColonyAnimal_InjuryedOnly, "display_ColonyAnimalInjuryedOnly", defaultValue: false);
            Scribe_Values.Look(ref display_ColonyMech, "display_Mech", defaultValue: false);
            Scribe_Values.Look(ref display_ColonyMech_InjuryedOnly, "display_ColonyMechInjuryedOnly", defaultValue: false);
            Scribe_Values.Look(ref display_OnDraftedOnly, "display_OnDraftedOnly", defaultValue: false);
            Scribe_Values.Look(ref display_Enemy, "display_Enemy", defaultValue: true);
            Scribe_Values.Look(ref display_Enemy_InjuryedOnly, "display_EnemyHurtOnly", defaultValue: true);
            Scribe_Values.Look(ref enable_Skillbar, "enable_Skillbar", defaultValue: true);
            Scribe_Values.Look(ref display_Skillbar_OnDraftedOnly, "display_Skillbar_OnDraftedOnly", defaultValue: false);
            Scribe_Values.Look(ref disable_displayPawnLabelHealth, "disable_displayPawnLabelHealth", defaultValue: false);
            Scribe_Values.Look(ref display_PawnDeathIndicator_Option, "display_PawnDeathIndicator_Option", defaultValue: 1);
            Scribe_Values.Look(ref display_Option_HHMMSS, "display_Option_HHMMSS", defaultValue: true);
            Scribe_Values.Look(ref barWidth, "barWidth", defaultValue: 150);
            Scribe_Values.Look(ref barHeight, "barHeight", defaultValue: 75);
            Scribe_Values.Look(ref barMargin, "barMargin", defaultValue: -100);
            Scribe_Values.Look(ref zoomWithCamera, "zoomWithCamera", defaultValue: true);
            Scribe_Values.Look(ref drawOutOfCameraZoom, "drawOutOfCameraZoom", defaultValue: true);

            Scribe_Values.Look(ref r, "r", defaultValue: 105);
            Scribe_Values.Look(ref g, "g", defaultValue: 180);
            Scribe_Values.Look(ref b, "b", defaultValue: 210);
            Scribe_Values.Look(ref a, "a", defaultValue: 200);
            Scribe_Values.Look(ref r_enemy, "r_enemy", defaultValue: 220);
            Scribe_Values.Look(ref g_enemy, "g_enemy", defaultValue: 40);
            Scribe_Values.Look(ref b_enemy, "b_enemy", defaultValue: 20);
            Scribe_Values.Look(ref a_enemy, "a_enemy", defaultValue: 200);
            //
            Scribe_Values.Look(ref playOperatorVoice, "playVoice");
            Scribe_Values.Look(ref voiceIntervalTime, "voiceInterTime", 1);
            Scribe_Values.Look(ref displayBottomLeftPortrait, "displayP");
            Scribe_Values.Look(ref displayAnimationLeftPortrait, "displayPAnimation", defaultValue: true);
            Scribe_Values.Look(ref xOffset, "xOff");
            Scribe_Values.Look(ref yOffset, "yOff");
            Scribe_Values.Look(ref ratio, "ratio");
            Scribe_Values.Look(ref opacity, "opacity", 100);
            Scribe_Values.Look(ref debugOverride, "dOverride");
            Scribe_Values.Look(ref secretary, "secretary", "Amiya", true);
            Scribe_Values.Look(ref secretarySkin, "secSkin", 1);
            Scribe_Values.Look(ref secretaryLoc, "secLoc", new Vector3(400, 0, 1), true);
            Scribe_Values.Look(ref secLocSensitive, "secSense", 1, true);
            Scribe_Values.Look(ref font, "font", "AK_Font_YouYuan", true);
            Scribe_Values.Look(ref lastViewedClass, "lastClass", -1);
            Scribe_Values.Look(ref lastViewedSeries, "lastSeries", -1);
            Scribe_Values.Look(ref allowManualRegister, "manualReg");
        }
    }

    public class AK_Mod : Mod
    {
        public static AK_ModSettings settings;

        public static readonly bool ArknightsEnabled;
        public static readonly bool CameraPlusModEnabled;
        public static readonly bool SimpleCameraModEnabled;

        static AK_Mod()
        {
            if (ModLister.GetActiveModWithIdentifier("brrainz.cameraplus") != null) CameraPlusModEnabled = true;
            if (ModLister.GetActiveModWithIdentifier("ray1203.SimpleCameraSetting") != null) SimpleCameraModEnabled = true;
            if (ModLister.GetActiveModWithIdentifier("MIS.Arknights") != null) ArknightsEnabled = true;
        }

        public AK_Mod(ModContentPack content) : base(content)
        {
            ParseHelper.Parsers<ItemOnSpawn>.Register(ItemOnSpawn.Parser);
            settings = GetSettings<AK_ModSettings>();
            Harmony instance = new("AK_DLL");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new();
            listingStandard.Begin(inRect);
            if (Prefs.DevMode) listingStandard.CheckboxLabeled("测试模式", ref AK_ModSettings.debugOverride, "开启明日方舟MOD的测试模式。如果您不是测试人员请勿勾选此选项。");
            if (Prefs.DevMode || AK_ModSettings.allowManualRegister) listingStandard.CheckboxLabeled("AK_Option_AllowReg".Translate(), ref AK_ModSettings.allowManualRegister, "AK_Option_AllowRegDesc".Translate());

            //舟血条
            if (ArknightsEnabled && listingStandard.ButtonTextLabeled("AK_GUIBar_Setting".Translate(), "Open".Translate()))
            {
                Find.WindowStack.Add(new DoBarSetting_Window());
            }

            listingStandard.CheckboxLabeled("AK_Option_Play".Translate(), ref AK_ModSettings.playOperatorVoice, "AK_Option_PlayD".Translate());
            AK_ModSettings.voiceIntervalTime = (int)listingStandard.SliderLabeled("AK_Option_Interval".Translate() + $"{AK_ModSettings.voiceIntervalTime / 2.0}", AK_ModSettings.voiceIntervalTime, 0, 60f);

            listingStandard.CheckboxLabeled("AK_Option_DisP".Translate(), ref AK_ModSettings.displayBottomLeftPortrait);
            listingStandard.CheckboxLabeled("AK_Option_DisPAnimation".Translate(), ref AK_ModSettings.displayAnimationLeftPortrait, "AK_Option_DisPAnimationD".Translate());
            AK_ModSettings.xOffset = (int)listingStandard.SliderLabeled("AK_Option_xOffset".Translate() + $"{AK_ModSettings.xOffset * 5}", AK_ModSettings.xOffset, 0, 600);
            AK_ModSettings.yOffset = (int)listingStandard.SliderLabeled("AK_Option_yOffset".Translate() + $"{AK_ModSettings.yOffset * 5}", AK_ModSettings.yOffset, 0, 600);
            AK_ModSettings.ratio = (int)listingStandard.SliderLabeled("AK_Option_ratio".Translate() + $"{AK_ModSettings.ratio * 0.05f}", AK_ModSettings.ratio, 1, 40);
            AK_ModSettings.opacity = (int)listingStandard.SliderLabeled("AK_Option_opacity".Translate(AK_ModSettings.opacity), AK_ModSettings.opacity, 0, 100);

            AK_ModSettings.secLocSensitive = (int)listingStandard.SliderLabeled("AK_Option_SecSensetive".Translate() + $"{(float)AK_ModSettings.secLocSensitive * 10}", AK_ModSettings.secLocSensitive, 1, 10);

            if (AK_ModSettings.font == null) AK_ModSettings.Font = AKDefOf.AK_Font_YouYuan;
            List<FontDef> allFontDefs = DefDatabase<FontDef>.AllDefsListForReading;
            if (listingStandard.ButtonTextLabeled("AK_Option_selectFont".Translate(), AK_ModSettings.Font.label.Translate()))
            {
                List<FloatMenuOption> list = new();
                foreach (FontDef i in allFontDefs)
                {
                    FontDef j = i;
                    list.Add(new FloatMenuOption(j.label.Translate(), delegate
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
