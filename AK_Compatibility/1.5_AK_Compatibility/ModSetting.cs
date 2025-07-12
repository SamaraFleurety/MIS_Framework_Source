using UnityEngine;
using Verse;

namespace PA_AKPatch
{
    public class AKC_ModSettings : ModSettings
    {
        public static bool disable_PawnKindDef = false;
        public static bool disable_HideHead = false;
        public static bool disable_HideBody = false;
        public static bool disable_FacialAnimation = false;
        public static bool disable_FacialAnimation_NoFace = false;
        //
        public static bool MIS_NoFace_Actived => ModLister.GetActiveModWithIdentifier("MIS.NoFace") != null;
        public override void ExposeData() 
        {
            Scribe_Values.Look(ref disable_PawnKindDef, "disable_PawnKindDef", defaultValue: false);
            Scribe_Values.Look(ref disable_HideHead, "disable_HideHead", defaultValue: false);
            Scribe_Values.Look(ref disable_HideBody, "disable_HideBody", defaultValue: false);
            Scribe_Values.Look(ref disable_FacialAnimation, "disable_FacialAnimation", defaultValue: false);
            Scribe_Values.Look(ref disable_FacialAnimation_NoFace, "disable_FacialAnimation_NoFace", defaultValue: false);
        }
    }

    public class AKC_ModConfig : Mod
    {
        public static AKC_ModSettings settings;
        public AKC_ModConfig(ModContentPack content) : base(content)
        {
            settings = GetSettings<AKC_ModSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);
            list.CheckboxLabeled("禁用种族修复补丁".Translate(), ref AKC_ModSettings.disable_PawnKindDef, "招募干员时禁用PawnKind强制修复，非测试请勿打开");
            //list.CheckboxLabeled("禁用衣物隐藏头部".Translate(), ref AKC_ModSettings.disable_HideHead, "招募后的干员禁用方舟衣物的隐藏头部功能，非测试请勿打开");
            //list.CheckboxLabeled("禁用衣物隐藏身体".Translate(), ref AKC_ModSettings.disable_HideBody, "招募后的干员禁用方舟衣物的隐藏身体功能，非测试请勿打开");
            list.CheckboxLabeled("禁用NL动态表情补丁".Translate(), ref AKC_ModSettings.disable_FacialAnimation, "禁用所有MIS.Framework招募的pawn的强制动态补丁，非测试请勿打开");
            if (AKC_ModSettings.MIS_NoFace_Actived)
            {
                list.CheckboxLabeled("禁用无脸包NL动态表情兼容".Translate(), ref AKC_ModSettings.disable_FacialAnimation_NoFace, "禁用方舟干员无脸包动态表情补丁，非测试请勿打开");
            }
            list.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "Arknights-Compatibility";
        }
    }
}
