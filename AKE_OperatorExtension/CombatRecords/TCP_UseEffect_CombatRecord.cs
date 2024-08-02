using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static HarmonyLib.Code;

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
            user.skills.Learn(skill, Props.xpGainAmount, direct: true);
            switch (Props.rank)
            {
                case 1://初级
                    break;
                case 2://中级
                    if (user.skills.PassionCount <= 5 && user.skills.GetSkill(skill).passion == Passion.None)
                    {
                        user.skills.GetSkill(skill).passion = Passion.Minor;
                    }
                    break;
                case 3://高级
                    int numMajorSkills = user.skills.skills.Count(skillRecord => skillRecord.passion >= Passion.Major);
                    if (numMajorSkills <= 3 && (user.skills.GetSkill(skill).passion == Passion.None || user.skills.GetSkill(skill).passion == Passion.Minor))
                    {
                        user.skills.GetSkill(skill).passion = Passion.Major;
                    }
                    break;
            }
        }

        public override string TransformLabel(string label)
        {
            return base.TransformLabel(label) + $"({skill.label})";
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
            {
                return false;
            }
            TC_UseEffect_CombatRecord TC_useEffect_CombatRecord = other.TryGetComp<TC_UseEffect_CombatRecord>();
            if (TC_useEffect_CombatRecord == null || TC_useEffect_CombatRecord.skill != skill)
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
