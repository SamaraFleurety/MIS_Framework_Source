using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AKE_OperatorExtension
{
    public class TCP_UseEffect_CombatRecord : CompProperties_UseEffect
    {
        //public SkillDef skill;

        public float xpGainAmount = 1000f;

        public int rank = 0;

        public TCP_UseEffect_CombatRecord()
        {
            compClass = typeof(TC_UseEffect_CombatRecord);
        }
    }
    public class TC_UseEffect_CombatRecord : CompUseEffect
    {
        public TCP_UseEffect_CombatRecord Props => (TCP_UseEffect_CombatRecord)props;

        private SkillDef skill;

        //public Passion passion;

        public TC_UseEffect_CombatRecord()
        {
            skill = DefDatabase<SkillDef>.GetRandom();
        }

        public override void DoEffect(Pawn user)//使用作战记录
        {
            string userName = user.Name.ToString();
            switch (Props.rank)
            {
                case 0:
                    user.skills.Learn(skill = DefDatabase<SkillDef>.GetRandom(), Props.xpGainAmount, true);
                    return;
                case 1://初级
                    break;
                case 2://中级
                    int numFireSkills = user.skills.skills.Count(skillRecord => skillRecord.passion >= Passion.Major && skillRecord.passion >= Passion.Minor);
                    if ((user.skills.PassionCount <= 5 && user.skills.GetSkill(skill).passion == Passion.None) || userName.Contains("Jessica"))
                    {
                        user.skills.GetSkill(skill).passion = Passion.Minor;
                    }
                    break;
                case 3://高级
                    int numMajorSkills = user.skills.skills.Count(skillRecord => skillRecord.passion >= Passion.Major);
                    if ((numMajorSkills <= 3 && (user.skills.GetSkill(skill).passion == Passion.None || user.skills.GetSkill(skill).passion == Passion.Minor)) || userName.Contains("Jessica"))
                    {
                        user.skills.GetSkill(skill).passion = Passion.Major;
                    }
                    break;
            }

            user.skills.Learn(skill, Props.xpGainAmount, direct: true);
        }

        public override string TransformLabel(string label)
        {
            if (Props.rank == 0) return base.TransformLabel(label);
            return base.TransformLabel(label) + $"({skill.label})";
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
            {
                return false;
            }
            TC_UseEffect_CombatRecord TC_useEffect_CombatRecord = other.TryGetComp<TC_UseEffect_CombatRecord>();
            if (TC_useEffect_CombatRecord == null || TC_useEffect_CombatRecord.Props.rank == 0 || TC_useEffect_CombatRecord.skill != skill)
            {
                return false;
            }
            return true;
        }

        public void ExposeData()
        {
            ExposeData();
            Scribe_Defs.Look(ref skill, "skill");
        }
    }
}
