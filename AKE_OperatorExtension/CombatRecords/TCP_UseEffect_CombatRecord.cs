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
    public class CP_UseEffect_CombatRecord : CompProperties_UseEffect
    {
        public SkillDef skill;

        public float xpGainAmount = 1000f;

        public CP_UseEffect_CombatRecord()
        {
            compClass = typeof(CompUseEffect_CombatRecord);
        }
    }
    public class CompUseEffect_CombatRecord : CompUseEffect
    {
        public CP_UseEffect_CombatRecord Props => (CP_UseEffect_CombatRecord)props;

        public override void DoEffect(Pawn user)
        {
            user.skills.Learn(Props.skill, Props.xpGainAmount, direct: true);
        }
    }
}
