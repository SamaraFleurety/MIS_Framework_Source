using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace AK_DLL
{
    public class Hediff_Operator : HediffWithComps
    {
        public override bool ShouldRemove => false;
        public override void Notify_PawnDied()
        {
            this.document.voicePack.diedSound.PlayOneShot(null);
            this.document.RecordSkills();
            this.document.currentExist = false;
            this.comps.Clear();
            base.Notify_PawnDied();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref this.comps, "comps");
            Scribe_References.Look(ref this.document, "AK_Document");
        }

        public OperatorDocument document;

    }
}
