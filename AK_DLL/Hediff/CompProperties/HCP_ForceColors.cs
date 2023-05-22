using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AK_DLL
{

    public class HCP_ForceColors : HediffCompProperties
    {
        public Color skinColor = new Color();
        public Color hairColor = new Color();

        public HCP_ForceColors()
        {
            this.compClass = typeof(HC_ForceColors);
        }
    }
}
