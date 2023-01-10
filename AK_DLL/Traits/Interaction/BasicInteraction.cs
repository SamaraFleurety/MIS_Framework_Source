using RimWorld;
using Verse;

namespace AK_DLL.Traits.Interaction
{
    public class BasicInteraction : InteractionWorker
    {
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            if (AK_Tool.GetArkNightsHeDiffByPawn(initiator) is null || AK_Tool.GetArkNightsHeDiffByPawn(recipient) is null)
            {
                return 0f;
            }
            return 1f;
        }

    }
}
