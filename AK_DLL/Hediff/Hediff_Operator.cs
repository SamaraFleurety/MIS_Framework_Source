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
            Scribe_References.Look(ref this.document, "doc");
            //HC_Ability的定制存档 绕过HediffComp不可存档
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                this.compAbilities.Clear();
                foreach (HediffComp i in this.comps)
                {
                    if (i is HC_Ability)
                    {
                        compAbilities.Add(i as HC_Ability);
                    }
                }
            }
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (HC_Ability i in this.compAbilities)
                {
                    this.comps.Add(i);
                    i.parent = this;
                    if(i.AbilityDef.grouped) i.Document.groupedAbilities.Add(i);
                }
                compAbilities.Clear();
            }
            else 
            {
                Scribe_Collections.Look(ref this.compAbilities, "AK_abilities", LookMode.Deep);
            } 
        }


        public OperatorDocument document;
        public List<HC_Ability> compAbilities = new List<HC_Ability>();
    }
}
