using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AKA_Ability;

namespace AK_DLL
{
    public class AK_AbilityTracker : AKAbility_Tracker
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
            Scribe_References.Look(ref doc, "doc");
        }

        public override void PostPlayAbilitySound(AKAbility ability)
        {
            if (VoicePlayer.CanPlayNow()) return;
            base.PostPlayAbilitySound(ability);
            if (doc != null) doc.voicePack.abilitySounds.RandomElement().PlaySound();
        }
    }
}
