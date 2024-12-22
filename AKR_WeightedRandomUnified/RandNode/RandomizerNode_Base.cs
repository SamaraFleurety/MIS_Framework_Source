using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKR_Random
{
    public abstract class RandomizerNode_Base : IRandomizer, IWeightedRandomable
    {
        public int weight = 1;  //权重必须>=1，不然会被随机选到甚至报错

        #region xml不能填
        public virtual int Weight => weight;

        protected int[] weightCached;

        protected bool arrayCached = false;
        #endregion

        public abstract IEnumerable<IWeightedRandomable> Candidates {get; }

        public virtual IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null)
        {
            CaculateWeightArray();
            return Enumerable.Empty<Gizmo>();
        }
        //对于节点i，缓存从0到i求权重的和。随机时，从0到全权重和中抽一个x，并搜索>= x的值，并返回被选中的节点的下标。显然必有结果并且权重越大被选中概率越大。
        protected virtual void CaculateWeightArray()
        {
            if (arrayCached) return;
            List<IWeightedRandomable> cand = Candidates.ToList();
            this.weightCached = new int[cand.Count];
            this.weightCached[0] = cand[0].Weight;
            for (int i = 1; i < cand.Count; ++i)
            {
                this.weightCached[i] = this.weightCached[i - 1] + cand[i].Weight;
            }
            //arraySum = this.weight.Last();
            this.arrayCached = true;
        }
    }
}
