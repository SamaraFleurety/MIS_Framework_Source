using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class ThoughtWorker_AlwaysActiveOpinion : ThoughtWorker
    {

        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn otherPawn)
        {
            if (otherPawn.story.traits.HasTrait(TypeDef.operatorTrait[0]))
            {
                return ThoughtState.ActiveAtStage(0);
            }
            return false;
        }

    }
}
