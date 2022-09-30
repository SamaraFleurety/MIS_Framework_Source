using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AK_DLL
{
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
