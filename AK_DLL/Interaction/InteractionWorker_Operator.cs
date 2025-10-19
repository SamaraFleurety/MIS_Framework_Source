using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    public class InteractionWorker_Operator : InteractionWorker
    {
        public List<string> InvolvedPawns => (this.interaction as OperatorInteractionDef)?.involvedPawns;

        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            OperatorDocument initiatorDoc = initiator.GetDoc();
            OperatorDocument recipientDoc = recipient.GetDoc();
            if (initiatorDoc is null || recipientDoc is null)
            {
                return 0f;
            }

            if (this.interaction is OperatorInteractionDef)
            {
                if (!this.InvolvedPawns.Contains(initiatorDoc.operatorID) || !this.InvolvedPawns.Contains(recipientDoc.operatorID))
                {
                    return 0f;
                }
                return 100f;
            }
            return 1f;
        }

    }
}
