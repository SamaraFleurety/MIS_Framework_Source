using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;
using AKA_Ability;

namespace AK_DLL
{
    /*public class Hediff_Operator : HediffWithComps
    {
        public override bool ShouldRemove => false;
        public override void Notify_PawnDied()
        {
            this.document.voicePack.diedSound.PlayOneShot(null);
            //this.document.RecordSkills();
            this.document.currentExist = false;
            this.comps.Clear();
            base.Notify_PawnDied();
        }

        public override void Tick()
        {
            base.Tick();
            return;
            Log.Message($"AK 执行旧存档干员身份迁移: {pawn.Name}");
            VAbility_Operator vAbility = AbilityUtility.MakeAbility(AKDefOf.AK_VAbility_Operator, pawn) as VAbility_Operator; 
            vAbility.AKATracker = new AK_AbilityTracker
            {
                doc = document,
                owner = pawn
            }; 
            pawn.abilities.abilities.Add(vAbility); 
            AKA_AbilityTracker tracker = vAbility.AKATracker;
            foreach (HediffComp i in this.comps)
            {
                if (i is HC_Ability j)
                {
                    string oldName = j.AbilityDef.defName.Split('_').Last();
                    if (oldName == "ignit") oldName = "Ignit";
                    string newName = "AKA_Ability_" + oldName;
                    AKAbilityDef def = DefDatabase<AKAbilityDef>.GetNamedSilentFail(newName);
                    if (def != null)
                    {
                        Log.Message($"AK find reworked skill {newName}");
                        AKAbilityMaker.MakeAKAbility(def, tracker);
                    }
                    else Log.Message($"AK not find reworked skill {newName}");
                }
            }
            pawn.health.hediffSet.hediffs.Remove(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            return;
            /*Scribe_References.Look(ref this.document, "doc");
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
                    //if(i.AbilityDef.grouped) i.Document.groupedAbilities.Add(i);
                }
                compAbilities.Clear();
            }
            else 
            {
                Scribe_Collections.Look(ref this.compAbilities, "AK_abilities", LookMode.Deep);
            } 
        }


        public OperatorDocument document;
        //缓存。绕过存档时，本质是存的这类HC，而非所有HC。
        public List<HC_Ability> compAbilities = new List<HC_Ability>();
    }*/
}
