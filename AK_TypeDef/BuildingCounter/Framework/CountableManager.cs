using AK_DLL.Document;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Counter
{
    public class CountableManager
    {
        //原则上来说这里面所有东西都写成static都行，不爱这种写法。
        //现在这里只计数，不再合并处理独特(限1)相关逻辑
        public static CountableManager Instance => GC_Generic.instance.countableManager;

        //key1 层级; key2 ID; val(hashset) 该ID的所有建筑
        Dictionary<CountLevel, Dictionary<string, HashSet<IQuantityCountable>>> data = new()
        {
            { CountLevel.Global, new() },
            { CountLevel.Local, new() },
        };  

        //返回是否成功添加
        public bool TryAddCountable(IQuantityCountable countable, Map map = null)
        {
            Map mapSubstitute = countable.ParentThing?.Map;
            map ??= mapSubstitute;

            if (countable.CountLevel == CountLevel.Local && map == null)
            {
                Log.Error($"[AK.BC] 注册地图计数建筑时应该提供地图信息");
                return false;
            }

            string id = countable.ID;
            Dictionary<string, HashSet<IQuantityCountable>> levelDict = data[countable.CountLevel];

            if (!levelDict.ContainsKey(id))
            {
                levelDict[id] = new HashSet<IQuantityCountable>();
            }
            if (levelDict[id].Contains(countable))
            {
                //已经注册过了
                return false;
            }
            levelDict[id].Add(countable);
            return true;
        }

        public bool TryRemoveCountable(IQuantityCountable countable, Map map = null)
        {
            //销毁comp的时候，地图信息已经没了
            if (countable.CountLevel == CountLevel.Local && map == null)
            {
                Log.Error($"[AK.BC] 注销地图计数建筑时应该提供地图信息");
                return false;
            }
            string id = countable.ID;
            Dictionary<string, HashSet<IQuantityCountable>> levelDict = data[countable.CountLevel];
            if (!levelDict.ContainsKey(id))
            {
                //没有注册过
                return false;
            }
            if (!levelDict[id].Contains(countable))
            {
                //没有注册过
                return false;
            }
            levelDict[id].Remove(countable);
            return true;
        }

        public HashSet<IQuantityCountable> GetCountables(string id, CountLevel level)
        {
            Dictionary<string, HashSet<IQuantityCountable>> levelDict = data[level];

            if (!levelDict.ContainsKey(id))
            {
                return new HashSet<IQuantityCountable>();
            }

            return levelDict[id];
        }

        //get countables' count
        public int GetCountablesCount(string id, CountLevel level)
        {
            return GetCountables(id, level).Count;
        }
    }
}
