using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.TickCondition
{
    public class TiC_ColonistOnly : TickCondion_Base
    {
        public TiC_ColonistOnly(AbilityTracker tracker) : base(tracker)
        {
        }

        public override bool TickableNow()
        {
            return tracker.owner != null && tracker.owner.IsColonist;
        }
    }
}
