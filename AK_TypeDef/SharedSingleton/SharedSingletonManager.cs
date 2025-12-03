using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.SharedSingleton
{
    //有些地方需要xml里面写函数，但实际并不允许。所以改成xml里面写类，类里面有函数返回值。这种情况下不希望这个类被多次实例化。
    public class SharedSingletonManager
    {
        Dictionary<Type, ISharedSingleton> singletons = new();

        public T GetSharedSingleton<T>(Type type) where T : ISharedSingleton
        {
            if (!singletons.ContainsKey(type))
            {
                T singleton = (T)Activator.CreateInstance(type);
                singletons.Add(type, singleton);
            }
            return (T)singletons[type];
        }
    }
}
