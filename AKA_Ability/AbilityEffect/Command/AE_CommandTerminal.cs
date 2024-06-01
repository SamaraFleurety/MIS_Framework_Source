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
        #region
        public string ID = null;
        public int delaytime = 142;
        public int phase = 0;
        public int phaseCount = 0;
        public int MaxPhaseLevel = 5;
        public bool Isphase = false;
        public bool IsGrowthSys = false;
        private int A_Counter = 0;
        private bool FindAllDefsListForReading = false;
        public int AE_Counter => A_Counter;
        TimeToTick IntervalUnit = TimeToTick.day;
        int TimeMultiplier = 1;
        int LevelUPInterval => TimeMultiplier * (int)IntervalUnit;
        #endregion
        public override void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster = null)
        {
            A_Counter++;
            if (!FindAllDefsListForReading)
            {
                AE_RewardTool.FindAllDefsListForReading(ID);
                FindAllDefsListForReading = true;
            }
            AE_RewardTool.Delaytimer = delaytime;
            Find.WindowStack.Add(new AECommand_Window(map, target, ID, phase));
            //为成长系统预留的坑位
            if (IsGrowthSys)
            {
                phase++;
            }
            if (Isphase && !IsGrowthSys && (A_Counter % phaseCount == 0) && (phase < MaxPhaseLevel))
            {
                phase++;
            }

        }
    }
}
