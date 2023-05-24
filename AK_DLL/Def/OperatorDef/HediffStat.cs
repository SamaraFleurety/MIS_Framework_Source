using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class HediffStat
    {
        public HediffDef hediff;
        public BodyPartDef part = null;
        public float serverity = 1f;  //这个是招募时给的状态的严重度，和矿石病随机无关
        //以下是矿石病增加hediff时的加权随机
        public int randWeight = 1;
        public int randWorseMin = 0;
        public int randWorseMax = 1;
        public List<HediffStat> complication;
    }
}
