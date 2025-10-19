using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class OperatorSeriesDef : Def
    {
        public string icon = null;

        //用来在绘制UI时检索内含的职业。我觉得自动绑定就好，没必要手动写。
        public List<int> includedClasses = new();
        public Texture2D Icon => ContentFinder<Texture2D>.Get(icon);
    }
}
