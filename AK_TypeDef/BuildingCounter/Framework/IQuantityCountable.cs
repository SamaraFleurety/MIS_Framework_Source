using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Counter
{
    //在哪种层级限量 地图限1的建筑在世界上可能有多个
    public enum CountLevel
    {
        Global,
        Local,
    }

    /// <summary>
    /// 本来是cp那的 独特建筑功能，后来整理了变成这样
    /// 主要用于对建筑进行计数。和原版lister thing计数器不同的是，这个计数基于string为key，有可能有多种建筑视为同一个物品
    /// 此外，此框架支持全局计数 你也不想每次请求都遍历所有地图吧
    /// 注：此框架不进存档，纯实时运算
    /// </summary>
    public interface IQuantityCountable
    {
        string ID { get; }                          //可以默认使用建筑defname存。当计数comp不唯一时，须使用string存（有可能一个建筑有多个不唯一的计数comp，比如需要做类似生存者游戏的词条系统）
        bool ShouldRegister { get; }                //可能因为禁用等情况 此建筑不该被注册（也就是视为没建造此建筑）

        CountLevel CountLevel { get; }    //层级

        void TryRegister();                         //注册时必发 在Manager里面会调用
        void RegisterEffect();                      //注册成功时发动。比如已经注册，重复注册就是不成功
        void TryDeregister(Map map = null);
        void DeregisterEffect();

        Thing ParentThing { get; }      //我觉得这玩意虽然可以扩展不过总是挂在某个thing上面的
    }
}
