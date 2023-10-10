using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using System.Linq;

namespace AK_DLL
{
    //预防出错保留 大概率根本没啥用
    //绑定在干员Hediff上的入口。要求必须有干员ID。
    public class HCP_Ability : HediffCompProperties
    {
        public HCP_Ability()
        {
            this.compClass = typeof(HC_Ability);
        }
    }

    public class HC_Ability : HediffComp, IExposable
    {
        public OperatorDocument Document
        {
            get { return ((Hediff_Operator)this.parent).document; }
        }
        public HC_Ability()
        {
            this.CDandCharges = new CDandCharge();
        }
        public HC_Ability(OperatorAbilityDef abilityDef)
        {
            AbilityDef = abilityDef;
            this.CDandCharges = new CDandCharge(1, this.AbilityDef.maxCharge, this.AbilityDef.CD * (int)this.AbilityDef.CDUnit);
        }


        public string UniqueID
        {
            get
            {
                return this.AbilityDef.defName.Split('_').Last();
            }
        }
        public HCP_Ability Props => new HCP_Ability();
        public int MaxSummon = 0;
        Command_Abilities ability_Command = null;
        public void Summon()
        {
            this.summoned++;
        }

        public void SummonedDead()
        {
            this.summoned--;
        }

        /*public new int MaxCharges
        {
            get { return this.CDandCharges.maxCharge; }
        }*/

        public override void CompPostMake()
        {
            base.CompPostMake();
            //从def读取cd
            this.CDandCharges = new CDandCharge(1, this.AbilityDef.maxCharge, this.AbilityDef.CD * (int)this.AbilityDef.CDUnit);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!this.enabled) return;
            if (this.autoCast && this.CDandCharges.charge >= 1 && Find.TickManager.TicksGame % 180 == 0)
            {
                LocalTargetInfo target = this.Document.pawn.TargetCurrentlyAimingAt;
                if (!this.Document.pawn.Drafted || target == null) return;
                this.ability_Command.verb.TryStartCastOn(new LocalTargetInfo(this.Document.pawn), target);
            }
            if (this.CDandCharges.charge >= this.CDandCharges.maxCharge)
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

        public override IEnumerable<Gizmo> CompGetGizmos()
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
            ability_Command.pawn = this.Document.pawn;
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
            if (this.AbilityDef.targetMode == TargetMode.AutoEnemy)
            {

            }
            else if (this.CDandCharges.charge == 0)
            {
                ability_Command.Disable("AK_ChargeIsZero".Translate());
            }
            else if (!this.enabled)
            {
                ability_Command.Disable("AK_GroupedAbilityNotSelected".Translate());
            }
            else
            {
                ability_Command.disabled = false;
            }
            return new List<Gizmo>() { this.ability_Command };
        }
        public Verb GetVerb(VerbProperties verbProp, int num, bool isntReclaim)
        {
            Verb_Ability verb_var = (Verb_Ability)Activator.CreateInstance(verbProp.verbClass);
            verb_var.caster = this.Document.pawn;
            verb_var.verbProps = verbProp;
            verb_var.verbTracker = new VerbTracker(this.Document.pawn);
            verb_var.ability = this.AbilityDef;
            verb_var.i = num;  //这玩意儿干嘛的？
            verb_var.CDs = this.CDandCharges; //fixme：这有必要开个字段吗？
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


        public void ExposeData()
        {
            Scribe_Deep.Look(ref this.CDandCharges, "CD");
            Scribe_Values.Look(ref this.autoCast, "auto", true);
            //Scribe_Values.Look(ref summoned, "summoned");
            Scribe_Defs.Look(ref this.AbilityDef, "ability");
        }

        public bool enabled = true;
        public OperatorAbilityDef AbilityDef;
        public CDandCharge CDandCharges;
        public int summoned = 0;
        public bool autoCast = false;
    }
}