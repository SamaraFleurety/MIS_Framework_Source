using Verse;

namespace AK_DLL
{
    public class RenderNode_FrontHair : RenderNode_ExtraHair
    {
        public RenderNode_FrontHair(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Ext_ExtraGraphicData GetExt(Pawn pawn)
        {
            Ext_FrontHair ext_FrontHair = null;

            if (pawn.GetDoc() is OperatorDocument doc)
            {
                ext_FrontHair = doc.pendingFashionDef.GetModExtension<Ext_FrontHair>();
            }

            ext_FrontHair ??= pawn.story?.hairDef?.GetModExtension<Ext_FrontHair>();
            return ext_FrontHair;
        }

        /*public override Graphic GraphicFor(Pawn pawn)
        {
            Ext_FrontHair ext_FrontHair = null;

            if (pawn.GetDoc() is OperatorDocument doc)
            {
                ext_FrontHair = doc.operatorDef.GetModExtension<Ext_FrontHair>();
            }
            
            ext_FrontHair ??= pawn.story?.hairDef?.GetModExtension<Ext_FrontHair>();
            return ext_FrontHair?.GraphicFor(pawn);
        }*/
    }
}
