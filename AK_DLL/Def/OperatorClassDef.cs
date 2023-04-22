using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class OperatorClassDef : Def
    {
        public int sortingOrder = 0;
        public OperatorSeriesDef series = null;
        public string iconPath = null;
        public string textureFolder;

        public Texture2D Icon
        {
            get { return iconPath == null ? null : ContentFinder<Texture2D>.Get(iconPath); }
        }
    }
}
