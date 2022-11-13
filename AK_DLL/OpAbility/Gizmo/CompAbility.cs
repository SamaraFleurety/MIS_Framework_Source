using System;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace AK_DLL
{
    public class CompAbility : CompReloadable
    {
        public string UniqueID { 
            get
            {
                return this.AbilityDef.defName.Split('_').Last();
            }
        }
        public new CompProperties_Ability Props => (CompProperties_Ability)this.props;
        public OperatorAbilityDef AbilityDef => this.Props.abilityDef;
        public int MaxSummon => Props.maxSummoned;
        Command_Abilities ability_Command = null;
        public void Summon()
        {
            this.summoned++;
        }

        public void SummonedDead()
        {
            this.summoned--;
        }

        public new int MaxCharges
        {
            get { return this.CDandCharges.maxCharge; }
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            //从def读取cd
            this.CDandCharges = new CDandCharge(1, this.AbilityDef.maxCharge, this.AbilityDef.CD * (int)this.AbilityDef.CDUnit);
            /*int maxCharge_var = this.AbilityDef.maxCharge == 0 ? 1 : AbilityDef.maxCharge;
            int CD_var = 0;
            CDandCharge CDandCharge = new CDandCharge(CD_var, maxCharge_var, AbilityDef.CD);
            this.CDandChargesList.Add(CDandCharge);
            
            CDandCharge num0 = new CDandCharge(1,1,1);
            this.CDandChargesList.Append(num0);*/
        }

        public override void CompTick()
        {
            if (!this.Props.enabled) return;
            base.CompTick(); 
            if (this.autoCast && this.CDandCharges.charge >= 1 && Find.TickManager.TicksGame % 180 == 0)
            {
                LocalTargetInfo target = this.Wearer.TargetCurrentlyAimingAt;
                if (!this.Wearer.Drafted || target == null) return;
                this.ability_Command.verb.TryStartCastOn(new LocalTargetInfo(this.Wearer), target);
            }
            if (this.CDandCharges.charge == this.CDandCharges.maxCharge)
            {
                return;
            }
            
            this.CDandCharges.CD -= 1;
            if (this.CDandCharges.CD <= 0)
            {
                this.CDandCharges.charge += 1;
                if (this.CDandCharges.charge < this.CDandCharges.maxCharge)
                {
                    this.CDandCharges.CD = this.CDandCharges.maxCD;
                }
            }
        }

        /*public override void CompTick()
        {
            base.CompTick();
            List<CDandCharge> CDandCharge_var = new List<CDandCharge>();
            foreach (CDandCharge CDandCharge in this.CDandChargesList)
            {
                if (CDandCharge.charge != CDandCharge.maxCharge)
                {
                    int CD_var = (CDandCharge.CD <= 0) ? 0 : CDandCharge.CD - 1;
                    int charge_var = CDandCharge.charge;
                    if (CD_var == 0)
                    {
                        charge_var = CDandCharge.charge + 1;
                    }
                    CDandCharge CDandCharge_Loop = new CDandCharge(CD_var, CDandCharge.maxCharge, CDandCharge.maxCD);
                    CDandCharge_Loop.charge = charge_var;
                    CDandCharge_var.Add(CDandCharge_Loop);
                }
                else
                {
                    int CD_var = 0;
                    CDandCharge CDandCharge_Loop = new CDandCharge(CD_var, CDandCharge.maxCharge, CDandCharge.maxCD);
                    CDandCharge_var.Add(CDandCharge_Loop);
                }
            }
            this.CDandChargesList.Clear();
            this.CDandChargesList.AddRange(CDandCharge_var);
        }*/
        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            return DrawGizmo();
        }

        private void InitGizmo()
        {
            this.ability_Command = new Command_Abilities();
            ability_Command.parent = this;
            ability_Command.defaultLabel = AbilityDef.label;
            ability_Command.defaultDesc = AbilityDef.description;
            ability_Command.verb = this.GetVerb(AbilityDef.verb, 0, true);
            ability_Command.abilityDef = this.AbilityDef;
            ability_Command.iconAngle = 0f;
            ability_Command.iconOffset = new Vector2(0, 0);
            ability_Command.pawn = ((Apparel)parent).Wearer;
            if (AbilityDef.icon != null)
            {
                ability_Command.icon = ContentFinder<Texture2D>.Get(AbilityDef.icon);
            }
        }
        public IEnumerable<Gizmo> DrawGizmo()
        {
            if (this.ability_Command == null) InitGizmo();

            //FIXME:召唤的回收技能 移动到生物的comp上面去。
            /*if (AbilityDef.abilityType == AbilityType.Summon)
            {
                if (this.summoned == this.MaxSummon)
                {
                    ability_Command.Disable("AK_SummonedReachedMax".Translate(this.MaxSummon.ToString()));
                }
                Command_Abilities reclaim = ability_Command;
                Verb_Reclaim verb_Reclaim = this.GetVerb(AbilityDef.verb_Reclaim, i, false) as Verb_Reclaim;
                verb_Reclaim.pawn = AbilityDef.canReclaimPawn;
                reclaim.verb = verb_Reclaim;
                reclaim.icon = ContentFinder<Texture2D>.Get(AbilityDef.iconReclaim);
                reclaim.defaultLabel = AbilityDef.reclaimLabel;
                reclaim.defaultDesc = AbilityDef.reclaimDesc;
                reclaim.targetMode = TargetMode.Single;
                commandList.Add(reclaim);
                i += 1;
            }
            else
            {*/
            if (this.CDandCharges.charge == 0)
            {
                ability_Command.Disable("AK_ChargeIsZero".Translate());
            }
            if (!this.Props.enabled) {
                ability_Command.Disable("AK_GroupedAbilityNotSelected".Translate());
            }
            return new List<Gizmo>() { this.ability_Command };
        }
        public Verb GetVerb(VerbProperties verbProp, int num, bool isntReclaim)
        {
            Verb_Ability verb_var = (Verb_Ability)Activator.CreateInstance(verbProp.verbClass);
            verb_var.caster = ((Apparel)parent).Wearer;
            verb_var.verbProps = verbProp;
            verb_var.verbTracker = new VerbTracker(this);
            verb_var.ability = this.AbilityDef;
            verb_var.i = num;
            verb_var.CDs = this.CDandCharges;
            return verb_var;
            /*if (isntReclaim)
            {
            }
            else
            {
                Verb verb = (Verb)Activator.CreateInstance(verbProp.verbClass);
                verb.caster = ((Apparel)parent).Wearer;
                verb.verbProps = verbProp;
                verb.verbTracker = new VerbTracker(this);
                return verb;
            }*/
        }

        public override string CompInspectStringExtra()
        {
            return null;
        }

        public override void PostExposeData()
        {
            base.PostExposeData(); 
            //Scribe_References.Look(ref this.doc, "doc");
            Scribe_Deep.Look(ref this.CDandCharges, UniqueID + "CD");
            Scribe_Values.Look(ref this.autoCast, UniqueID + "auto");
            Scribe_Values.Look(ref summoned, UniqueID + "summoned");
        }

        //public OperatorDocument doc;
        public CDandCharge CDandCharges;
        public int summoned = 0;
        public bool autoCast = false;
    }
}
