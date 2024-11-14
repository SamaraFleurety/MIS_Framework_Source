using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class Ext_WeaponWieldGraphics : DefModExtension
    {
        public GraphicData alterGraphics;

        Graphic graphicInt;
        public Graphic DefaultGraphic(Thing t)
        {
            if (graphicInt == null)
            {
                if (alterGraphics == null)
                {
                    return BaseContent.BadGraphic;
                }
                graphicInt = alterGraphics.GraphicColoredFor(t);
            }
            return graphicInt;
        }
    }
}