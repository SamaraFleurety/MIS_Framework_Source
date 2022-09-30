using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace AK_DLL
{
    public class Hediff_Operator : Hediff
    {
       
        public override bool ShouldRemove => false;
        public override void Notify_PawnDied()
        {
            this.voicePackDef.diedSound.PlayOneShot(null);
            document.RecordSkills();
            document.currentExist = false;
            base.Notify_PawnDied();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref this.voicePackDef, "AK_VoicePackDef");
            Scribe_References.Look(ref this.document, "AK_Document");
        }

        public OperatorDocument document;
        public VoicePackDef voicePackDef;

    }
}
