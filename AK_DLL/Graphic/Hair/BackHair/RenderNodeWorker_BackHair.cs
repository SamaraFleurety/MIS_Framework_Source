using Verse;

namespace AK_DLL
{
    public class RenderNodeWorker_BackHair : RenderNodeWorker_ExtraHair
    {
        protected override Ext_ExtraGraphicData Ext(PawnDrawParms parms)
        {
            return parms.pawn.story.hairDef.GetModExtension<Ext_BackHair>();
        }
    }
}
