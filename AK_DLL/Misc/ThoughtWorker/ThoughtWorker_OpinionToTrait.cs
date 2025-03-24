using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AK_DLL
{
    //当前仅用于和弦的，所有人对和弦看法下降。它虽然是个ThoughtWorker，但是和一般的Thought不太一样。
    public class ThoughtWorker_OpinionToOperator : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            OperatorDocument doc = otherPawn.GetDoc();
            if (doc == null || doc.operatorDef == null) return false;
            if (true && doc.operatorDef.thoughtReceived == this.def)
            {
                return ThoughtState.ActiveAtStage(doc.operatorDef.TRStage);
            }
            return false;
        }

    }
}
