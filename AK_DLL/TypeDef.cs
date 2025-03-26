using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public static class TypeDef
    {
        //public static string[] operatorTypeStiring = new string[] { "Caster", "Defender", "Guard", "Vanguard", "Specialist", "Supporter", "Medic", "Sniper" };
        public static string[] romanNumber = new string[] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI" };
        //如果增加了要自动绑定的服装种类，只需要往这个字符串数组增加。
        public static string[] apparelType = new string[] { "Apparel", "Hat" };
        public static string ModID = "MIS.Framework";

        public static Dictionary<string, int> statType = new Dictionary<string, int>() {
            { "Shooting", 0 },
            { "Melee", 1},
            { "Construction", 2},
            { "Mining", 3},
            { "Cooking", 4 },
            { "Plants", 5 },
            { "Animals", 6 },
            { "Crafting", 7 },
            { "Artistic", 8 },
            { "Medicine", 9 },
            { "Social", 10 },
            { "Intellectual", 11 }
        };

        public static List<SkillDef> SortOrderSkill = new List<SkillDef>() { SkillDefOf.Shooting, SkillDefOf.Melee, SkillDefOf.Construction, SkillDefOf.Mining, SkillDefOf.Cooking, SkillDefOf.Plants, SkillDefOf.Animals, SkillDefOf.Crafting, SkillDefOf.Artistic, SkillDefOf.Medicine, SkillDefOf.Social, SkillDefOf.Intellectual};

        public static Vector3 defaultSecLoc => new Vector3(400, 0, 1);
        public static Vector3 defaultSecLocLive => new Vector3(10010, -10, 10000);

        public static Dictionary<string, GameObject> cachedLive2DModels = new Dictionary<string, GameObject>();

        public static Texture2D iconTeleTowerChangeName => ContentFinder<Texture2D>.Get("UI/Gizmo/ChangeName");

        public static Texture2D tempTexture => ContentFinder<Texture2D>.Get("Things/Item/RecruitTicket/RecruitTicket_a");

        static TypeDef()
        {
        }
    }

    [DefOf]
    public static class AKDefOf
    {
        public static HediffDef AK_Hediff_AlienRacePatch;

        public static AbilityDef AK_VAbility_Operator;

        public static JobDef AK_Job_UseRecruitConsole;
        public static JobDef AK_Job_OperatorChangeFashion;

        public static FontDef AK_Font_YouYuan;

        public static UIPrefabDef AK_Prefab_yccMainMenu;
        public static UIPrefabDef AK_Prefab_yccOpList;
        public static UIPrefabDef AK_Prefab_OpDetail;
    }

    public enum OpDetailType : byte
    {
        Recruit,
        Secretary,
        Fashion
    }

    public enum StandType : Byte
    {
        Static = 0,
        Live2D
    }
    public enum SkinType : Byte
    {
        Vanilla = 0,
        EliteII,
        SkinI,  //也可能是Elite I
        SkinII,
        SkinIII,
        SkinIV,
        SkinV
    }

    public enum RIWindowType : Byte
    {
        MainMenu = 0,
        Op_Series,
        Op_Gacha,
        Op_List,
        Op_Detail,
        Store,
        Option,
        Support,
        Quest
    }
    public enum OperatorSortType : Byte
    {
        Head = 11,
        Dps,
        Alphabet,
        Tail,
    }

    /*public enum TimeToTick
    {
        tick = 1,          //游戏中
        tickRare = 250,
        tickLong = 2000,
        hour = 2500,
        day = hour * 24,   //60k
        season = day * 15, //0.9M
        year = season * 4, //3.6M
        rSecond = 60       //现实中的1秒
    }*/
}