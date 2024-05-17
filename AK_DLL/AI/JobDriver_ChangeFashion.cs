using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;
using Verse;

namespace AK_DLL
{
    public class JobDriver_ChangeFashion : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell);
            Toil t = new Toil();
            t.initAction = delegate
            {
                OperatorDocument doc = pawn.GetDoc();
                if (doc == null) return;
                doc.operatorDef.ChangeFashion(doc.pendingFashionDef, pawn);
                //if (doc.pendingFashionDef.standIndex is int standIndexInt) doc.preferedSkin = standIndexInt;
                if (doc.pendingFashionDef == null) doc.preferedSkin = 1;
                else if (doc.pendingFashionDef.standIndex is int standIndexInt) doc.preferedSkin = standIndexInt;
            };
            t.defaultCompleteMode = ToilCompleteMode.Instant;
            yield return t;
            yield break;
        }
    }
}
