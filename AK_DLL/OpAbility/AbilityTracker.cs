using Verse;
using AKA_Ability;

namespace AK_DLL
{
    public class AK_AbilityTracker : AbilityTracker
    {
        public OperatorDocument doc;

        public AK_AbilityTracker() : base()
        {
        }

        public AK_AbilityTracker(Pawn p) : base(p)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_References.Look(ref doc, "doc");
        }

        public override void Notify_AbilityCasted(AKAbility_Base ability)
        {
            base.Notify_AbilityCasted(ability);

            if (VoicePlayer.CanPlayNow()) return;
            doc?.voicePack.abilitySounds?.RandomElement().PlaySound();
        }

        /*public override void AbilityPostEffect_PlayAbilitySound(AKAbility ability)
        {
            if (VoicePlayer.CanPlayNow()) return;
            base.AbilityPostEffect_PlayAbilitySound(ability);
            if (doc != null) doc.voicePack.abilitySounds.RandomElement().PlaySound();
        }*/
    }
}
