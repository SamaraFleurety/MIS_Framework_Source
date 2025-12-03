using Verse;

namespace AK_DLL
{
    public class RenderNode_BackHair : RenderNode_ExtraHair
    {
        public RenderNode_BackHair(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Ext_ExtraGraphicData GetExt(Pawn pawn)
        {
            Ext_BackHair ext = null;

            if (pawn.GetDoc() is OperatorDocument doc)
            {
                ext = doc.pendingFashionDef.GetModExtension<Ext_BackHair>();
            }

            ext ??= pawn.story?.hairDef?.GetModExtension<Ext_BackHair>();
            return ext;
        }

        /*public override Graphic GraphicFor(Pawn pawn)
        {
            Ext_BackHair ext_BackHair = pawn.story?.hairDef?.GetModExtension<Ext_BackHair>();
            return ext_BackHair?.graphicData.GraphicColoredFor(pawn);
        }*/
    }
}
