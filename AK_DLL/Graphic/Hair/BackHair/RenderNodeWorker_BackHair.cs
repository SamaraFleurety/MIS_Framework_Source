using Verse;

namespace AK_DLL
{
    public class RenderNodeWorker_BackHair : RenderNodeWorker_ExtraHair
    {
        protected override Ext_ExtraGraphicData Ext(PawnDrawParms parms)
        {
            Pawn pawn = parms.pawn;
            Ext_BackHair ext = null;

            if (pawn.GetDoc() is OperatorDocument doc)
            {
                ext = doc.pendingFashionDef?.GetModExtension<Ext_BackHair>();
            }

            ext ??= pawn.story?.hairDef?.GetModExtension<Ext_BackHair>();
            return ext;
            //return parms.pawn.story.hairDef.GetModExtension<Ext_BackHair>();
        }
    }
}
