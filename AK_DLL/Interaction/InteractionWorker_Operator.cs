using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    public class InteractionWorker_Operator : InteractionWorker
    {
        public List<string> InvolvedPawns
        {
            get { return (this.interaction as OperatorInteractionDef).involvedPawns; }
        }

        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            OperatorDocument initiatorDoc = initiator.GetDoc();
            OperatorDocument recipientDoc = recipient.GetDoc();
            if (initiatorDoc is null || recipientDoc is null)
            {
                return 0f;
            }
            else if (this.interaction is OperatorInteractionDef)
            {
                Log.Message("ISGROUPED");
                foreach (string i in InvolvedPawns) Log.Message(i);
                Log.Message(initiatorDoc.defName + recipientDoc.defName);
                if (!this.InvolvedPawns.Contains(initiatorDoc.defName) || !this.InvolvedPawns.Contains(recipientDoc.defName))
                {
                    return 0f;
                }
                return 100f;
            }
            return 1f;
        }

    }
}
