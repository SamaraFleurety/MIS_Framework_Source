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
            /*t.initAction = delegate
            {
                OperatorDocument doc = pawn.GetDoc();
                if (doc == null) return;
                doc.operatorDef.ChangeFashion(doc.pendingFashion, pawn);
            };  */
            t.initAction = delegate
            {
                OperatorDocument doc = pawn.GetDoc();
                if (doc == null) return;
                doc.operatorDef.ChangeFashion(doc.pendingFashionDef, pawn);
            };
            yield return t;
            yield break;
        }
    }
}
