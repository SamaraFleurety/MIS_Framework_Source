using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class OperatorClassDef : Def
    {
        public int sortingOrder = 0;        //全局唯一排序和识别ID
        public OperatorSeriesDef series;
        public string iconPath = null;
        public string textureFolder;

        public Texture2D Icon => iconPath == null ? null : ContentFinder<Texture2D>.Get(iconPath);
    }
}
