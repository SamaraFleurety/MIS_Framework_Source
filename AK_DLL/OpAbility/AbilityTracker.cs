using AKA_Ability;
using Verse;

namespace AK_DLL
{
    public class AK_AbilityTracker : AbilityTracker
    {
        public OperatorDocument doc;

        public AK_AbilityTracker()
        {
        }

        public AK_AbilityTracker(Pawn p) : base(p)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref doc, "doc");
        }

        public override void Notify_AbilityCasted(AKAbility_Base ability)
        {
            base.Notify_AbilityCasted(ability);

            if (!VoicePlayer.CanPlayNow()) return;
            doc?.voicePack.abilitySounds?.RandomElement()?.PlaySound();
        }
    }
}
