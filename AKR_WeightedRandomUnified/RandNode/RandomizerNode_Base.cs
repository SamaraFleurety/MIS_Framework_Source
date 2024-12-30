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
        //是否从奖池继承权重。
        //如果否的话，本节点被抽中的概率被预先填好，和奖品完全无关。
        //如果是，那就是本节点包含的奖品越多，概率也越大。等于子节点直接参与了本级的抽选，本节点仅相当于一种类似过路节点一样的，无实际意义的节点。
        public bool inheritCandidatesWeight = false;

        //这是*本节点*被随机到权重 权重必须>=1，不然会被随机选到甚至报错。仅在不继承权重时填入才会被使用。
        public int weight = 1;  

        #region xml不能填
        public virtual int Weight
        {
            get
            {
                if (inheritCandidatesWeight)
                {
                    CaculateWeightArray();
                    weight = weightCached[weightCached.Length - 1];
                }

                return weight;
            }
        }

        protected int[] weightCached;

        protected bool arrayCached = false;
        #endregion

        public abstract IEnumerable<IWeightedRandomable> Candidates {get; }


        public abstract IEnumerable<object> TryIssueGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0);

        //返回随机结果-reward的下标
        public virtual int RandRewardIndex()
        {
            this.CaculateWeightArray();
            return Algorithm.WeightArrayRand(weightCached);
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
