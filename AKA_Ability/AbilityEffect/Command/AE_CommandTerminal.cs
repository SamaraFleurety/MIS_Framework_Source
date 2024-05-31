using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AKA_Ability
{
    public class AE_CommandTerminal : AbilityEffectBase
    {
        public int delaytime = 142;
        public int phase = 0;
        TimeToTick IntervalUnit = TimeToTick.day;
        int TimeMultiplier = 1;
        int LevelUPInterval => TimeMultiplier * (int)IntervalUnit;
        public override void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster = null)
        {
            AE_RewardTool.Delaytimer = delaytime;
            Find.WindowStack.Add(new AECommand_Window(map, target, phase));
            if (AE_RewardTool.PhaseCount % 2 == 0)
            {
                phase++;
            }

        }
    }
}
