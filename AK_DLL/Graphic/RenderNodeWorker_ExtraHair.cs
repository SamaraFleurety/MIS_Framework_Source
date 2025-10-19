using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AK_DLL
{
    public abstract class RenderNodeWorker_ExtraHair : PawnRenderNodeWorker
    {
        protected abstract Ext_ExtraGraphicData Ext(PawnDrawParms parms);
        public override Vector3 OffsetFor(PawnRenderNode node, PawnDrawParms parms, out Vector3 pivot)
        {
            Vector3 offset = base.OffsetFor(node, parms, out pivot);
            Ext_ExtraGraphicData ext = Ext(parms);
            offset += ext.graphicData.DrawOffsetForRot(parms.facing);

            if (SleepingOffset(parms))
            {
                offset += ext.sleepingDrawOffsets.DrawOffsetForRot(parms.facing);
            }

            return offset;
        }

        //碧蓝有些舰娘比较矮，睡觉的时候头位置偏低
        private bool SleepingOffset(PawnDrawParms parms)
        {
            if (!parms.Portrait)
            {
                if (parms.posture == PawnPosture.Standing)
                {
                    return false;
                }
                Pawn_MindState mindState = parms.pawn.mindState;
                if (mindState != null && mindState.duty?.def?.drawBodyOverride.HasValue == true)
                {
                    return !parms.pawn.mindState.duty.def.drawBodyOverride.Value;
                }
                if (parms.bed != null && parms.pawn.RaceProps.Humanlike)
                {
                    return !parms.bed.def.building.bed_showSleeperBody;
                }
            }

            return false;
        }

        public override Vector3 ScaleFor(PawnRenderNode node, PawnDrawParms parms)
        {
            Vector3 scale = base.ScaleFor(node, parms);
            Ext_ExtraGraphicData ext = Ext(parms);
            Vector2 graphicDataScale = ext.graphicData.drawSize;
            scale.x *= graphicDataScale.x;
            scale.z *= graphicDataScale.y;
            return scale;
        }
    }
}
