using AK_DLL;
using AKA_Ability;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKE_OperatorExtension
{
    public class HCP_SkillUpbyTime : HediffCompProperties
    {
        public int interval = 1;
        public string TargetTrait;
        public HCP_SkillUpbyTime()
        {
            this.compClass = typeof(HC_SkillUpbyTime);
        }
    }

    public class HC_SkillUpbyTime : HediffComp
    {
        private int tick = 0;
        public HCP_SkillUpbyTime Props => (HCP_SkillUpbyTime)this.props;

        public TimeToTick intervalUnit = TimeToTick.year;
        private int Interval_year => this.Props.interval * (int)intervalUnit;
        private string TargetTrait => this.Props.TargetTrait;
        private List<SkillRecord> Skills => parent.pawn.skills.skills;
        public override void CompPostTick(ref float severityAdjustment)
        {
            tick++;
            if (tick >= (int)Interval_year)
            {
                tick = 0;
                if (!parent.pawn.HasSkillTracker())
                {
                    return;
                }
                if (parent.pawn.HasOperatorTraitDef(TargetTrait))
                {
                    parent.comps.Remove(this);
                    return;
                }
                foreach (SkillRecord Skill in Skills)
                {
                    Skill.Level += 1;
                }
            }
        }
    }
}
