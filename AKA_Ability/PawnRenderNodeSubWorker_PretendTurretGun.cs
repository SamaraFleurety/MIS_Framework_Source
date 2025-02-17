using Verse;
using UnityEngine;

namespace AKA_Ability
{
    public class PawnRenderNodeSubWorker_PretendTurretGun : PawnRenderSubWorker
    {

        public override void TransformRotation(PawnRenderNode node, PawnDrawParms parms, ref Quaternion rotation)
        {
            var targ = parms.pawn.equipment?.PrimaryEq?.PrimaryVerb?.CurrentTarget;
            if (targ?.IsValid ?? false)
            {
                rotation *= (targ.Value.CenterVector3 - parms.pawn.DrawPos).AngleFlat().ToQuat();
            }
            else
            {
                rotation = 180f.ToQuat();
            }
        }
    }
}
