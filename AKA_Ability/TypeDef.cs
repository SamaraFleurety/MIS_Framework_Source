using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKA_Ability
{
    public static class TypeDef
    {
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
}
