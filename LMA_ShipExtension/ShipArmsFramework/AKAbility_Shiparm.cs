using AKA_Ability;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LMA_Lib.Shipborne
{
    //舰娘用舰装火炮
    //此技能的gizmo可以手动确定目标覆盖，若无目标会使用一个共享目标
    //不需要冷却系统 -- verb有一套cd
    public class AKAbility_Shiparm : AKAbility_Targetor
    {
        Verb_LaunchProjectile shiparmVerb;

        public AKAbility_Shiparm(LMA_AbilityTracker tracker) : base(tracker)
        {
        }

        public AKAbility_Shiparm(AKAbilityDef def, LMA_AbilityTracker tracker) : base(def, tracker)
        {
        }

        public override void Tick()
        {
            base.Tick();
            shiparmVerb.VerbTick();
            //shiparmVerb.TryStartCastOn();
        }
    }
}
