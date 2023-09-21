using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public static class TypeDef
    {
        public static string[] operatorTypeStiring = new string[] { "Caster", "Defender", "Guard", "Vanguard", "Specialist", "Supporter", "Medic", "Sniper" };
        public static string[] romanNumber = new string[] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X", "XI" };
        //如果增加了要自动绑定的服装种类，只需要往这个字符串数组增加。
        public static string[] apparelType = new string[] { "Apparel", "Hat" };

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

        static TypeDef()
        {
        }
    }

    public static class HarmonyPrefixRet
    {
        public static bool skipOriginal = false;
        public static bool keepOriginal = true;
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
    public enum AbilityType : Byte
    {
        Summon,//召唤
        Trap,//陷阱
        Hediff,//状态
        Heal,//治疗
        Reclaim//回收
    }
    public enum TargetMode : Byte
    {
        Single = 0,
        Self,
        AutoEnemy, //没做
        Multi //没做，别用
    }
    public enum TimeToTick
    {
        tick = 1,          //游戏中
        tickRare = 250,
        tickLong = 2000,
        hour = 2500,
        day = hour * 24,   //60k
        season = day * 15, //0.9M
        year = season * 4, //3.6M
        rSecond = 60       //现实中的1秒
    }

    public enum SFXType : Byte
    {
        atk = 0,
        def,
        heal,
        tact
    }

    public enum RegrowType : Byte
    {
        replace = 0, //覆盖模式，无条件覆盖
        enhance,     //增强模式，对于每个属性，取最强的
        compare,     //比较模式（默认），两个属性比较总治疗量，取最多的
        chronic      //长期模式，会覆盖任何其他再生，也会被其他任何再生覆盖（即不可被强化）
    }
}
