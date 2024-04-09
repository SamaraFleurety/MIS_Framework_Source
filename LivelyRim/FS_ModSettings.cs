using RimWorld;
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
        public static bool mainMenuLive = true;

        //在商店，主菜单等地方画的l2d的def
        public static LiveModelDef l2dDef = null;
        public static string l2dDefname = "AZ_Live_Janus";

        public static bool autofillCubismCoreLib = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref debugOverride, "dOverride", false);
            Scribe_Values.Look(ref merchantSideLive, "merchantSide", true);
            Scribe_Values.Look(ref autofillCubismCoreLib, "fillCore", true, true);
            Scribe_Values.Look(ref mainMenuLive, "menuLive", true);
            Scribe_Values.Look(ref l2dDefname, "l2dDef", "AZ_Live_Janus", true);
            base.ExposeData();
        }

    }

    public class FS_Mod : Mod
    {
        FS_ModSettings settings;

        LiveModelDef selectedLiveDef
        {
            get
            {
                if (FS_ModSettings.l2dDef == null) FS_ModSettings.l2dDef = DefDatabase<LiveModelDef>.GetNamed("AZ_Live_Janus");
                return FS_ModSettings.l2dDef;
            }
            set { FS_ModSettings.l2dDef = value; }
        }

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
            listingStandard.CheckboxLabeled("menulive".Translate(), ref FS_ModSettings.mainMenuLive, "menulivedesc".Translate());
            listingStandard.CheckboxLabeled("fillCore".Translate(), ref FS_ModSettings.autofillCubismCoreLib, "fillCoreDesc".Translate());

            List<LiveModelDef> allDefs = DefDatabase<LiveModelDef>.AllDefsListForReading;

            if (listingStandard.ButtonTextLabeled("selectl2ddef".Translate(), selectedLiveDef.label))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach(LiveModelDef i in allDefs)
                {
                    LiveModelDef local = i;
                    list.Add(new FloatMenuOption(i.label.Translate(), delegate ()
                    {
                        FS_Utilities.ChangeDefaultModel(local);
                    }));
                    Find.WindowStack.Add(new FloatMenu(list));
                }
            }

            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "FS. Live2D Cubism";
        }
    }
}
