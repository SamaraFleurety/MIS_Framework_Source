using RimWorld;
using System;

namespace AKA_Ability
{
    public static class TypeDef
    {
        //public static SoundDef[] abilitySFX = new SoundDef[4] { AKADefOf.AK_SFX_Atkboost, AKADefOf.AK_SFX_Defboost, AKADefOf.AK_SFX_Healboost, AKADefOf.AK_SFX_Tactboost };
    }

    [DefOf]
    public static class AKADefOf
    {

        public static StatDef AKA_Stat_RangedDamageFactor; //远程伤害倍率
        public static StatDef AK_Stat_CraftQualityOffset;  //制作质量偏移（整数）

        public static StatDef AK_Stat_MoveSpeedFactor; //移动速度倍率，默认是1 仍然可以写在衣服的offset。

    }

    public enum RegrowType : Byte
    {
        replace = 0, //覆盖模式，无条件覆盖
        enhance,     //增强模式，对于每个属性，取最强的
        compare,     //比较模式（默认），两个属性比较总治疗量，取最多的
        chronic      //长期模式，会覆盖任何其他再生，也会被其他任何再生覆盖（即不可被强化）
    }
    public enum AbilityType : Byte
    {
        Summon,//召唤
        Trap,//陷阱
        Hediff,//状态
        Heal,//治疗
        Reclaim//回收
    }

    /*public enum TargetMode : Byte
    {
        VerbSingle = 0,
        Self,
        AutoEnemy, //没做
        Multi //没做，别用
    }*/

}
