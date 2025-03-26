using RimWorld;
using Verse;

namespace AKA_Ability
{
    public class AE_ChangeHair : AbilityEffectBase
    {
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            Pawn p = target.Pawn;
            if (p == null) return false;
            if (p.story == null) return false;
            p.story.hairDef = DefDatabase<HairDef>.GetRandom();
            p.style.Notify_StyleItemChanged();
            p.style.MakeHairFilth();
            return base.DoEffect(caster, target);
        }
    }
}
