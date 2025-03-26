using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class RenderNodeWorker_FrontHair : PawnRenderNodeWorker
    {
        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 offset = base.OffsetFor(node, parms, out pivot);
            offset += parms.pawn.story.hairDef.GetModExtension<Ext_FrontHair>().graphicData.DrawOffsetForRot(parms.facing);
            return offset;
        }

        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            /*if (StackTraceUtility.ExtractStackTrace().Contains("Portrait"))
            {
                Log.Message($"getting scale at {StackTraceUtility.ExtractStackTrace()}");
            }*/

            
            Vector3 scale = base.ScaleFor(node, parms);
            Vector2 graphicDataScale = parms.pawn.story.hairDef.GetModExtension<Ext_FrontHair>().graphicData.drawSize;
            scale.x *= graphicDataScale.x;
            scale.z *= graphicDataScale.y;
            return scale;
        }
    }
}
