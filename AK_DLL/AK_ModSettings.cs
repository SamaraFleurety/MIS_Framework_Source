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
        public static bool playOperatorVoice = true;
        public static int voiceIntervalTime = 2;
        public List<Pawn> exampleListOfPawns = new List<Pawn>();
        //public Dictionary<string, Pawn>;


        /// <summary>
        /// The part that writes our settings to file. Note that saving is by ref.
        /// </summary>
        public override void ExposeData()
        {
            //自动填充
            Scribe_Values.Look(ref playOperatorVoice, "playVoice");
            Scribe_Values.Look(ref voiceIntervalTime, "voiceInterTime", 1);
            Scribe_Collections.Look(ref exampleListOfPawns, "exampleListOfPawns", LookMode.Reference);
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
            listingStandard.CheckboxLabeled("AK_Option_Play".Translate(), ref AK_ModSettings.playOperatorVoice, "Play operators' voice when: recruting, selecting, drafting and died".Translate());
            listingStandard.Label($"Interval time between voices: {(float)AK_ModSettings.voiceIntervalTime / 2.0}");
            AK_ModSettings.voiceIntervalTime = (int)listingStandard.Slider(AK_ModSettings.voiceIntervalTime, 0, 60f);
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
