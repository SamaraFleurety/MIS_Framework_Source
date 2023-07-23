using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FS_LivelyRim
{
    public class FS_ModSettings : ModSettings
    {
        public static bool debugOverride = false;

        public static bool merchantSideLive = true;  //商人右移，并展示L2D

        public static bool autofillCubismCoreLib = true;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref debugOverride, "dOverride", false);
            Scribe_Values.Look(ref merchantSideLive, "merchantSide", true);
            Scribe_Values.Look(ref autofillCubismCoreLib, "fillCore", true, true);
        }
    }

    public class FS_Mod : Mod
    {
        FS_ModSettings settings;

        public FS_Mod(ModContentPack content) : base(content)
        {
            settings = GetSettings<FS_ModSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            if (Prefs.DevMode) listingStandard.CheckboxLabeled("测试模式", ref FS_ModSettings.debugOverride, "开启Live2D MOD的测试模式。如果您不是测试人员请勿勾选此选项。");
            listingStandard.CheckboxLabeled("merchantsidelive".Translate(), ref FS_ModSettings.merchantSideLive, "merchantsidelivedesc".Translate());
            listingStandard.CheckboxLabeled("fillCore".Translate(), ref FS_ModSettings.autofillCubismCoreLib, "fillCoreDesc".Translate());

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "FS. Live2D Cubism for Rim";
        }
    }
}
