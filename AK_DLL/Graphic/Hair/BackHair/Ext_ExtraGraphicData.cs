using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class DrawOffsets
    {
        public Vector3 drawOffset = Vector3.zero;

        public Vector3? drawOffsetNorth;

        public Vector3? drawOffsetEast;

        public Vector3? drawOffsetSouth;

        public Vector3? drawOffsetWest;

        public Vector3 DrawOffsetForRot(Rot4 rot)
        {
            return rot.AsInt switch
            {
                0 => drawOffsetNorth ?? drawOffset,
                1 => drawOffsetEast ?? drawOffset,
                2 => drawOffsetSouth ?? drawOffset,
                3 => drawOffsetWest ?? drawOffset,
                _ => drawOffset,
            };
        }
    }

    public class Ext_ExtraGraphicData : DefModExtension
    {
        public GraphicData graphicData;

        public DrawOffsets sleepingDrawOffsets = new();

        //为是的话，未征召状态就没有图片
        //public bool hideWhenUndraft = false;

        //不在床上就不显示
        //这里处理不了 在render node里面判断
        public bool showOnlyOnBed = false;
        public Graphic GraphicFor(Pawn pawn)
        {
            //if (hideWhenUndraft && !pawn.Drafted) return null;
            return graphicData.GraphicColoredFor(pawn);
        }
    }
}
