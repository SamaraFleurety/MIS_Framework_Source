using AK_DLL;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKR_Random
{
    public class TCP_LotteryAuto : TCP_Lottery
    {
        public int interval = 1;
        public TimeToTick intervalUnit = TimeToTick.day;

        public int repeat = 1;  //每次到点抽多少次
        public TCP_LotteryAuto()
        {
            compClass = typeof(TC_LotteryAuto);
        }
    }

    public class TC_LotteryAuto : TC_Lottery
    {
        TCP_LotteryAuto Prop => props as TCP_LotteryAuto;

        protected float tick = 0;

        public int Interval => Prop.interval * (int)Prop.intervalUnit;

        public override void CompTick()
        {
            Tick(1);
        }

        public override void CompTickLong()
        {
            Tick(TimeToTickDirect.tickLong);
        }

        public override void CompTickRare()
        {
            Tick(TimeToTickDirect.tickRare);
        }
        public virtual void Tick(int amt)
        {
            if (!ShouldTick()) return;
            tick += amt;
            if (tick >= Interval)
            {
                tick = 0;
                Gacha(parent.InteractionCell, parent.Map).Count();
            }
        }

        public virtual bool ShouldTick()
        {
            return true;
        }
    }
}
