using Verse;

namespace AK_DLL
{
    //手持武器时，贴图改变
    //偏移一律仅使用drawOffsetSouth
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