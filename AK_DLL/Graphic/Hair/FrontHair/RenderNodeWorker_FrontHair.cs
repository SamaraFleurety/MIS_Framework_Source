using Verse;

namespace AK_DLL
{
    public class RenderNodeWorker_FrontHair : RenderNodeWorker_ExtraHair
    {
        /*public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 offset = base.OffsetFor(node, parms, out pivot);
            offset += parms.pawn.story.hairDef.GetModExtension<Ext_FrontHair>().graphicData.DrawOffsetForRot(parms.facing);
            return offset;
        }

        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Vector3 scale = base.ScaleFor(node, parms);
            Vector2 graphicDataScale = parms.pawn.story.hairDef.GetModExtension<Ext_FrontHair>().graphicData.drawSize;
            scale.x *= graphicDataScale.x;
            scale.z *= graphicDataScale.y;
            return scale;
        }*/
        protected override Ext_ExtraGraphicData Ext(PawnDrawParms parms)
        {
            Pawn pawn = parms.pawn;
            Ext_FrontHair ext_FrontHair = null;

            if (pawn.GetDoc() is OperatorDocument doc)
            {
                ext_FrontHair = doc.pendingFashionDef?.GetModExtension<Ext_FrontHair>();
            }

            ext_FrontHair ??= pawn.story?.hairDef?.GetModExtension<Ext_FrontHair>();
            return ext_FrontHair;
        }
    }
}
