using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.Range
{
    //严格来讲这堆光辉的技能可能该单开dll 但要不是光辉不会有这些时髦系统 评价是吃水不忘挖井人
    public abstract class RangeWorker_Base
    {
        public AKAbility parent;

        protected RangeWorker_Base(AKAbility parent)
        {
            this.parent = parent;
        }

        public abstract float Range();
    }
}
