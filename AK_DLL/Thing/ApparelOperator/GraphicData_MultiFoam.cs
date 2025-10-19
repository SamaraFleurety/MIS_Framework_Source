using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    //此graphic data含有别的多个data，以数字key检索。如果无法检索会返回本data的默认图片
    public class GraphicData_MultiFoam : GraphicData
    {
        public Dictionary<int, GraphicData> innerGraphicData_Drop = new(); //掉地上的贴图

        public Dictionary<int, string> innerGraphicPath_Worn = new(); //穿身上的贴图的路径，同wornGraphicPath
        public virtual Graphic GetGraphicWithIndex(int key, Thing source)
        {
            return !innerGraphicData_Drop.TryGetValue(key, out GraphicData data) ? Graphic : data.Graphic;
        }

        public virtual string GetWornGraphicPathWithIndex(int key, Apparel source)
        {
            bool flag = innerGraphicPath_Worn.TryGetValue(key, out string path);
            return flag ? path : source.WornGraphicPath;
        }
    }
}
