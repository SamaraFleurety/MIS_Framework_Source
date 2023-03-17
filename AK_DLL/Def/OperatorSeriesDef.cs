using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class OperatorSeriesDef : Def
    {
        public string icon = null;
        public Texture2D Icon
        {
            get { return ContentFinder<Texture2D>.Get(icon); }
        }
    }
}
