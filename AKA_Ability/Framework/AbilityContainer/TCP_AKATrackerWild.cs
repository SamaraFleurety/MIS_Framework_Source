using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    //设计之初技能默认必然绑在人身上，这是开了个野蛮后门使其可以放在建筑之类和人无关的地方
    //不是真的清楚此框架原理就不要乱用
    public class TCP_AKATrackerWild : TCP_AKATracker
    {
        public TCP_AKATrackerWild()
        {
            compClass = typeof(TC_AKATrackerWild);
        }
    }

    public class TC_AKATrackerWild : TC_AKATracker
    {
        public override void CompTick()
        {
            tracker.Tick();
        }

        public override void CompTickLong()
        {
            tracker.Tick();
        }

        public override void CompTickRare()
        {
            tracker.Tick();
        }

        public override void CompTickInterval(int delta)
        {
            tracker.Tick();
        }
    }
}
