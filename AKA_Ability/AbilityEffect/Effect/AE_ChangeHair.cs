using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AE_ChangeHair : AbilityEffectBase
    {
        public override void DoEffect_Pawn(Pawn user, Thing target, bool delayed)
        {
            if (!(target is Pawn p)) return;
            if (p.story == null) return;
            p.story.hairDef = DefDatabase<HairDef>.GetRandom();
            p.style.Notify_StyleItemChanged();
            p.style.MakeHairFilth();
        }
    }
}
