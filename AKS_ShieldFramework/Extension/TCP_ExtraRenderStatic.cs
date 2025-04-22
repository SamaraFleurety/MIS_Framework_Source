using UnityEngine;
using Verse;

namespace AKS_Shield.Extension
{
    public class TCP_ExtraRenderStatic : CompProperties
    {
        public GraphicData bubbleStaticOverlay = null;          //静态护盾贴图
        public TCP_ExtraRenderStatic()
        {
            compClass = typeof(TC_ExtraRenderStatic);
        }
    }

    public class TC_ExtraRenderStatic : TC_ShieldExtension_Base
    {
        TCP_ExtraRenderStatic Props => props as TCP_ExtraRenderStatic;

        public override void PostDraw()
        {
            if (!CompShield.ShouldDisplay) return;

            DrawStaticOverlay(Props.bubbleStaticOverlay, Wearer, parent);
        }

        public override void CompDrawWornExtras()
        {
            if (!CompShield.ShouldDisplay) return;

            DrawStaticOverlay(Props.bubbleStaticOverlay, Wearer, parent);
        }

        public static void DrawStaticOverlay(GraphicData graphicData, Pawn wearer, Thing colorParent)
        {
            if (graphicData == null || wearer == null) return;
            Graphic graphic = graphicData.GraphicColoredFor(colorParent);
            if (graphic == null) return;

            Rot4 rot = wearer.Rotation;
            if (graphicData.graphicClass == typeof(Graphic_Single)) rot = Rot4.North;     //单图护盾不该旋转

            Vector3 loc = wearer.DrawPos;
            if (graphicData.graphicClass == typeof(Graphic_Single))           //单层贴图默认+1
            {
                loc.y += 1.05f;
            }
            else if (graphicData.graphicClass == typeof(Graphic_Multi))       //多向静态贴图，应用比如碧蓝的舰装
            {
                if (wearer.Rotation != Rot4.South) loc.y += 1.05f;         //显示在人物上面
                else loc.y -= 1.05f;                                       //只有朝南是显示在人物下面
            }

            loc += graphicData.DrawOffsetForRot(rot);

            graphic.Draw(loc, rot, wearer);
        }
    }
}
