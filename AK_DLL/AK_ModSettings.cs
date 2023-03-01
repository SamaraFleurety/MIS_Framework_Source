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
        //语音
        public static bool playOperatorVoice = true;
        public static int voiceIntervalTime = 2;
        //立绘
        public static bool displayBottomLeftPortrait = true;
        public static int xOffset = 0;
        public static int yOffset = 0;
        public static int ratio = 20;
        public static bool debugOverride = false;
        //public List<Pawn> exampleListOfPawns = new List<Pawn>();
        //public Dictionary<string, Pawn>;


        public override void ExposeData()
        {
            //自动填充
            Scribe_Values.Look(ref playOperatorVoice, "playVoice");
            Scribe_Values.Look(ref voiceIntervalTime, "voiceInterTime", 1);
            Scribe_Values.Look(ref displayBottomLeftPortrait, "displayP");
            Scribe_Values.Look(ref xOffset, "xOff");
            Scribe_Values.Look(ref yOffset, "yOff");
            Scribe_Values.Look(ref ratio, "ratio");
            Scribe_Values.Look(ref debugOverride, "dOverride", false, true);
            //Scribe_Collections.Look(ref exampleListOfPawns, "exampleListOfPawns", LookMode.Reference);
            base.ExposeData();
        }
    }

    public class AK_Mod : Mod
    {
        AK_ModSettings settings;

        public AK_Mod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<AK_ModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            if (Prefs.DevMode) listingStandard.CheckboxLabeled("测试模式", ref AK_ModSettings.debugOverride, "开启明日方舟MOD的测试模式。如果您不是测试人员请勿勾选此选项。");
            listingStandard.CheckboxLabeled("AK_Option_Play".Translate(), ref AK_ModSettings.playOperatorVoice, "AK_Option_PlayD".Translate());
            AK_ModSettings.voiceIntervalTime = (int)listingStandard.SliderLabeled("AK_Option_Interval".Translate() + $"{(float)AK_ModSettings.voiceIntervalTime / 2.0}", AK_ModSettings.voiceIntervalTime, 0, 60f);

            listingStandard.CheckboxLabeled("AK_Option_DisP".Translate(), ref AK_ModSettings.displayBottomLeftPortrait);
            AK_ModSettings.xOffset = (int)listingStandard.SliderLabeled("AK_Option_xOffset".Translate() + $"{AK_ModSettings.xOffset * 5}", AK_ModSettings.xOffset, 0, 600);
            AK_ModSettings.yOffset = (int)listingStandard.SliderLabeled("AK_Option_yOffset".Translate() + $"{AK_ModSettings.yOffset * 5}", AK_ModSettings.yOffset, 0, 600);
            AK_ModSettings.ratio = (int)listingStandard.SliderLabeled("AK_Option_ratio".Translate() + $"{(float)AK_ModSettings.ratio * 0.05f}", AK_ModSettings.ratio, 1, 40);

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        /// <summary>
        /// Override SettingsCategory to show up in the list of settings.
        /// Using .Translate() is optional, but does allow for localisation.
        /// </summary>
        /// <returns>The (translated) mod name.</returns>
        public override string SettingsCategory()
        {
            return "MIS.Arknights";
        }
    }
}
