using Verse;

namespace AK_DLL
{
    public class RenderNode_FrontHair : PawnRenderNode
    {
        public RenderNode_FrontHair(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            Ext_FrontHair ext_FrontHair = pawn.story?.hairDef?.GetModExtension<Ext_FrontHair>();
            if (ext_FrontHair == null) return null;
            return ext_FrontHair.graphicData.GraphicColoredFor(pawn);
        }
    }
}
