using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //草 写错了 这个是右下角那个信件
    public class AE_SendLetter : AbilityEffectBase
    {
        public string label;
        public string content;
        LetterDef letterDef;

        public override bool DoEffect(AKAbility_Base caster, GlobalTargetInfo globalTargetInfo = default, LocalTargetInfo localTargetInfo = default)
        {
            Find.LetterStack.ReceiveLetter(label, content, letterDef);
            return true;
        }
    }
}
