using System;
using System.Collections.Generic;
using Verse;

namespace AKR_Random.Rewards
{
    //被选中时，直接给予本节点所有奖励。本节点是终点，不会再继续开连锁
    public abstract class RewardSet_Base : IWeightedRandomable
    {
        public int weight = 1;
        public virtual int Weight => weight;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cell">刷奖品的中心节点</param>
        /// <param name="map">刷奖品的地图</param>
        /// <param name="gambler">谁抽的</param>
        /// <param name="point">抽奖点数。默认并不使用</param>
        /// <returns></returns>
        public abstract IEnumerable<object> GenerateGachaResult(IntVec3 cell = default, Map map = null, Pawn gambler = null, float point = 0);

        #region 动态生成reward节点
        //动态生成reward节点时，判断奖励入参是否合法。 
        //抽奖时不使用
        //reward节点不一定允许动态生成
        public virtual bool RewardValidator(object reward)
        {
            Log.Error($"[AKR] {this.GetType()} 不允许动态生成");
            return false;
        }
        //设置奖励
        public virtual void SetReward(object reward)
        {
            return;
        }
        public static T GenerateRewardSet<T>(object reward, int weight = 1) where T : RewardSet_Base
        {
            T rewardNode = (T)Activator.CreateInstance(typeof(T));

            if (rewardNode == null || !rewardNode.RewardValidator(reward))
            {
                Log.Error($"[AKR] 生成{typeof(T)}时出错: 无效节点类型或者无效奖励");
                return null;
            }

            rewardNode.weight = weight;
            rewardNode.SetReward(reward);
            return rewardNode;
        }
        #endregion
    }
}
