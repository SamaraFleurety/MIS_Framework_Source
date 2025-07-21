using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AKE_OperatorExtension
{
    public class BreakGraphicPath
    {
        public string path;
        public float hpRatioLowerThan;  //hp比例低于这个值时，应用这个路径
    }
    public class Ext_ApparelBreakGraphic : DefModExtension
    {
        bool sorted = false;

        //数据量小 不需要用什么花哨结构
        private List<BreakGraphicPath> paths;

        //index和上次不一样才需要刷新
        public string WornGraphicPathByHPRatio(float ratio, out int index)
        {
            if (!sorted)
            {
                sorted = true;
                paths.SortBy(x => x.hpRatioLowerThan);
            }

            for (int i = 0; i < paths.Count; i++)
            {
                if (ratio <= paths[i].hpRatioLowerThan)
                {
                    index = i;
                    return paths[i].path;
                }
            }

            index = 99;
            return paths.Last().path;
        }
    }
}
