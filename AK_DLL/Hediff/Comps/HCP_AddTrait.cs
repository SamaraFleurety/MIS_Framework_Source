using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class HCP_AddTrait : HediffCompProperties
    {
        public List<TraitAndDegree> traits;
        public HCP_AddTrait()
        {
            compClass = typeof(HC_AddTrait);
        }
    }

    public class HC_AddTrait : HediffComp
    {
        private HCP_AddTrait Props => props as HCP_AddTrait;

        private List<TraitAndDegree> Traits => Props.traits;

        public override void CompPostTick(ref float severityAdjustment)
        {
            foreach (TraitAndDegree TraitAndDegree in this.Traits)
            {
                TraitDegreeData data = TraitAndDegree.def.degreeDatas[TraitAndDegree.degree];
                parent.pawn.story.traits.GainTrait(new Trait(TraitAndDegree.def, TraitAndDegree.degree));
                if (data.skillGains != null)
                {
                    foreach (SkillGain skillGain in data.skillGains)
                    {
                        SkillRecord record = parent.pawn.skills.GetSkill(skillGain.skill);
                        if (record != null) record.levelInt += skillGain.amount;
                    }
                }
                /*if (data.skillGains != null)
                {
                    foreach (SkillRecord j in parent.pawn.skills.skills)
                    {
                        if (data.skillGains.ContainsKey(j.def)) j.Level += data.skillGains[j.def];
                    }
                }*/
            }
            parent.Severity = -1;
        }
    }
}
