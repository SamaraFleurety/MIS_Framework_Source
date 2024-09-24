using System;
using System.Collections.Generic;
using Verse;

namespace AKA_Ability.Cooldown
{
    //极堕残光专用
    //充能会趋近于1。在不刚好为1的情况，每秒会减少当前总能量的1%或者增加充到1层的5%
    public class CD_Illustrious_Dilapidate : CD_Stricken
    {
        int tick = 0;

        //到达每层后，需要多少能量才能获得下一级充能
        static List<int> spToNextCharge = new List<int>
        {
            40,
            40,
            60,
            120,
            160,
            160,   // <--6级就是满级，但是允许过充来对抗衰减
            300
        };

        public CD_Illustrious_Dilapidate(CooldownProperty property, AKAbility ability) : base(property, ability)
        {
            Init();
        }

        //需要多少sp才能到达index级充能
        static List<int> cumulativeSpAtCharge = null;

        int cumulativeSp = 0;

        //充能趋于的极限
        const int chargeLimit = 1;
        //sp的渐变极限
        static int SP_Limit => cumulativeSpAtCharge[chargeLimit];

        public override void Tick(int amt)
        {
            if (charge == 1 && SP == 0) return;
            tick += amt;

            int seconds = tick / 60;

            if (seconds == 0) return;

            tick -= seconds * 60;

            //每秒减少1%
            if (cumulativeSp > SP_Limit)
            {
                cumulativeSp = (int)Math.Round((double)cumulativeSp * Math.Pow(0.99, seconds));
            }
            else   //每秒自动充5%,直到充满一层
            {
                cumulativeSp += (int)(0.05 * seconds * cumulativeSpAtCharge[1]);
                if (cumulativeSp > cumulativeSpAtCharge[1]) cumulativeSp = cumulativeSpAtCharge[1];
            }
            RefreshChargeAndSP();
        }

        public override float CooldownPercent()
        {
            return SP / spToNextCharge[charge];
        }

        //有特殊的cd机制 每层需求充能不一样
        public override void RefreshChargeAndSP()
        {
            //极值
            if (cumulativeSp <= 0)
            {
                cumulativeSp = 0;
                //SP = 0;
                charge = 0;
            }
            else if (cumulativeSp >= cumulativeSpAtCharge[MaxCharge])   //已经达到了满级。但可能有过充 - 这是一般cd不允许的
            {
                cumulativeSp = Math.Min(cumulativeSp, cumulativeSpAtCharge.GetLast());  //不允许连过充都溢出
                charge = MaxCharge;
                //SP = cumulativeSp - cumulativeSpAtCharge[MaxCharge];
            }

            charge = AKA_Algorithm.quickSearch(cumulativeSpAtCharge.ToArray(), 0, cumulativeSpAtCharge.Count - 1, cumulativeSp, 3);
            SP = cumulativeSp - cumulativeSpAtCharge[charge];  //超过当前等级的sp
        }

        private void Init()
        {
            if (cumulativeSpAtCharge != null) return;

            cumulativeSpAtCharge = new List<int>(spToNextCharge.Count + 1);
            cumulativeSpAtCharge.Add(0);
            for (int i = 0; i < spToNextCharge.Count; ++i)
            {
                cumulativeSpAtCharge.Add(cumulativeSpAtCharge[i] + spToNextCharge[i]);  //注意，加在cumulativeSpAtCharge[j]，j = i+1
            }
        }

        //会直接用掉所有充能
        public override void CostCharge(int cost)
        {
            cumulativeSp = 0;
            SP = 0;
            charge = 0;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tick, "tick");
            Scribe_Values.Look(ref cumulativeSp, "cumuSP");
        }

        public override void Notify_PawnStricken(ref DamageInfo dinfo)
        {
            int amt = (int)dinfo.Amount;
            if (amt <= 0) amt = 1;
            SP += amt;
            RefreshChargeAndSP();
        }
    }
}
