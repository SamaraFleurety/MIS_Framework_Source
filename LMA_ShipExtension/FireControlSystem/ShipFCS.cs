using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LMA_Lib.FCS
{
    public class ShipFCS
    {
        public float rangeOffset = 0;
        public float warmupFactor = 1;
        public float accuracyOffset = 0;

        public ShipFCS()
        {

        }

        public ShipFCS(float rangeOffset, float warmupFactor, float accuracyOffset)
        {
            this.rangeOffset = rangeOffset;
            this.warmupFactor = warmupFactor;
            this.accuracyOffset = accuracyOffset;
        }

        public static ShipFCS operator +(ShipFCS a, ShipFCS b)
        {
            return new ShipFCS(a.rangeOffset + b.rangeOffset, a.warmupFactor * b.warmupFactor, a.accuracyOffset + b.accuracyOffset);
        }
    }
}
