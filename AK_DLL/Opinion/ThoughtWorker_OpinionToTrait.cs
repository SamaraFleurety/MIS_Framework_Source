using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class ThoughtWorker_OpinionToOperator : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            OperatorDocument doc = otherPawn.GetDoc();
            if (true && doc != null && doc.operatorDef.thoughtReceived == this.def)
            {
                return ThoughtState.ActiveAtStage(doc.operatorDef.TRStage);
            }
            return false;
        }

    }
}
