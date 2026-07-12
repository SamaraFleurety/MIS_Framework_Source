using AK_DLL;
using AK_DLL.Gacha;
using AK_DLL.Rewards;
using AKR_Random;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Random = System.Random;

namespace LMA_Lib
{
    public class GC_AzurManager : GameComponent
    {
        //GC更新间隔
        private const int UPDATE_INTERVAL = 10;

        //往招募台存的白银，可以按比例兑换成招募券
        public int storedSilver = 0;
        //最大Up干员数,UI就5个槽位
        public const int MaxUpOperators = 5;
        //Up概率
        public static float UpProbability = 0.1f;

        private long weeklyPeriodStartTicks = 0L;

        private List<OperatorDef> weeklyUpOperators = new();
        private float nextWeeklyUpdateTime;
        private bool weeklyUpInitialized = false;

        public static GC_AzurManager Instance => Current.Game?.GetComponent<GC_AzurManager>();

        public GC_AzurManager(Game game)
        {
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            InitializeUpPool(DateTime.Now);
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            InitializeUpPool(DateTime.Now);
        }

        public override void GameComponentUpdate()
        {
            base.GameComponentUpdate();
            if (!weeklyUpInitialized) return;

            if (Time.realtimeSinceStartup < nextWeeklyUpdateTime) return;
            nextWeeklyUpdateTime = Time.realtimeSinceStartup + UPDATE_INTERVAL;

            DateTime curTime = DateTime.Now;
            DateTime curPeriodStart = GachaGenerator.GetPeriodStart(curTime);
            if (curPeriodStart.Ticks != weeklyPeriodStartTicks)
            {
                RefreshUpPool(AzurDefOf.LMA_Rander_Operators, curTime);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref storedSilver, "Ag", 0);
            Scribe_Values.Look(ref weeklyPeriodStartTicks, "weeklyUpPeriodStartTicks", 0L);
            Scribe_Collections.Look(ref weeklyUpOperators, "weeklyUpOperators", LookMode.Def);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && weeklyUpOperators == null)
            {
                weeklyUpOperators = new List<OperatorDef>();
            }
        }

        public IReadOnlyList<OperatorDef> GetUpOperators(RandomizerDef pool)
        {
            RefreshUpPool(pool, DateTime.Now);
            return weeklyUpOperators;
        }

        public TimeSpan GetWeeklyUpRemainingTime(DateTime currentTime)
        {
            DateTime periodStart = GachaGenerator.GetPeriodStart(currentTime);
            TimeSpan remaining = periodStart.AddDays(7) - currentTime;
            return remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero;
        }

        private void InitializeUpPool(DateTime currentTime)
        {
            RefreshUpPool(AzurDefOf.LMA_Rander_Operators, currentTime);
            nextWeeklyUpdateTime = Time.realtimeSinceStartup + 10f;
            weeklyUpInitialized = true;
        }

        private void RefreshUpPool(RandomizerDef source, DateTime currentTime)
        {
            if (source == null || source.root is not RandomizerNode_Reward_Operator operatorNode)
            {
                throw new Exception("[LMA] 无效的卡池节点");
            }

            List<Rewards_Operator> rewards = operatorNode.Candidates.OfType<Rewards_Operator>().ToList();
            foreach (Rewards_Operator reward in rewards)
            {
                reward.weight = 1;
            }

            List<OperatorDef> candidates = rewards.Select(reward => reward.operatorDef).Distinct().OrderBy(operatorDef => operatorDef.defName, StringComparer.Ordinal).ToList();
            DateTime periodStart = GachaGenerator.GetPeriodStart(currentTime);
            if (!IsUpPoolValid(periodStart, candidates))
            {
                int maximumCount = Mathf.Min(MaxUpOperators, candidates.Count);
                Random random = new(GachaGenerator.SeedWeekly(periodStart));
                int upCount = random.Next(1, maximumCount + 1);
                weeklyUpOperators = GachaGenerator.RequestWeekly(candidates, upCount, periodStart);
                weeklyPeriodStartTicks = periodStart.Ticks;
            }

            ApplyWeeklyWeights(operatorNode, rewards, weeklyUpOperators, UpProbability);
        }

        //当前池子是否过期
        private bool IsUpPoolValid(DateTime periodStart, IReadOnlyCollection<OperatorDef> candidates)
        {
            if (weeklyPeriodStartTicks != periodStart.Ticks) return false;

            if (weeklyUpOperators == null || weeklyUpOperators.Count < 1 || weeklyUpOperators.Count > Mathf.Min(MaxUpOperators, candidates.Count))
            {
                return false;
            }

            HashSet<OperatorDef> candidateSet = new(candidates);
            HashSet<OperatorDef> upOperatorSet = new();
            return weeklyUpOperators.All(operatorDef => operatorDef != null && candidateSet.Contains(operatorDef) && upOperatorSet.Add(operatorDef));
        }

        private static void ApplyWeeklyWeights(RandomizerNode_Reward_Operator operatorNode, IReadOnlyCollection<Rewards_Operator> rewards, IReadOnlyCollection<OperatorDef> upOperators, float upProbability)
        {
            if (upProbability is < 0f or >= 1f)
            {
                throw new ArgumentOutOfRangeException(nameof(upProbability), upProbability, "UP probability must be at least zero and less than one.");
            }

            HashSet<OperatorDef> upOperatorSet = new(upOperators);
            int upWeight = rewards.Count(reward => upOperatorSet.Contains(reward.operatorDef));
            int normalWeight = rewards.Count(reward => !upOperatorSet.Contains(reward.operatorDef));
            float upWeightFactor = CalculateUpWeightFactor(upWeight, normalWeight, upProbability);

            foreach (Rewards_Operator reward in rewards)
            {
                reward.weight = upOperatorSet.Contains(reward.operatorDef) ? Mathf.CeilToInt(upWeightFactor) : 1;
            }

            operatorNode.InvalidateWeightCache();
        }

        //池子大小*up概率=最终up权重
        private static float CalculateUpWeightFactor(float upWeight, float normalWeight, float upProbability)
        {
            if (upWeight <= 0f || normalWeight <= 0f || upProbability <= 0f) return 1f;

            float factor = upProbability * normalWeight / ((1f - upProbability) * upWeight);
            return Mathf.Max(1f, factor);
        }
    }
}
