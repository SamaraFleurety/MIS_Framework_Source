using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public abstract class RenderNode_ExtraHair : PawnRenderNode
    {
        protected RenderNode_ExtraHair(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public abstract Ext_ExtraGraphicData GetExt(Pawn pawn);

        public override Graphic GraphicFor(Pawn pawn)
        {
            return GetExt(pawn)?.GraphicFor(pawn);
        }
    }
}
