using RimWorld;
using Verse;
using System.Linq;


namespace AK_DLL.Traits.Interaction
{
    public class SpecialInteraction : InteractionWorker
    {
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            var involvedPawns = (this.interaction as SpecialInteractionDef).involvedPawns;
            var realPawnNames = from interactionPawnName in involvedPawns select interactionPawnName.Split('_').Last();

            var initiatorDoc = initiator.GetDoc();
            var recipientDoc = recipient.GetDoc();
            if (initiatorDoc is null || recipientDoc is null)
            {
                return 0f;
            }
            if (!realPawnNames.Contains(initiatorDoc.defName) || !realPawnNames.Contains(recipientDoc.defName))
            {
                return 0f;
            }
            return 1f;
        }
    }
}
