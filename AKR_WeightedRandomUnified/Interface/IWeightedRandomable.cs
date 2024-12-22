using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKR_Random
{
    //有权重的单位 可能是node，也可能是最终奖品
    public interface IWeightedRandomable
    {
        public int Weight { get; }
    }
}
