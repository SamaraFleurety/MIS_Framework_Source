using AK_DLL;
using AK_DLL.Gacha;
using AK_DLL.Rewards;
using AKR_Random;
using RimWorld;
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
        public const int SilverExchangeRate = 1280;
        //GC更新间隔
        private const int UPDATE_INTERVAL = 10;

        //往招募台存的白银，可以按比例兑换成招募券
        public int storedSilver;
        //最大Up干员数,UI就5个槽位
        public const int MaxUpOperators = 5;
        //Up概率
        public static float UpProbability = 0.1f;

        private int monthlyPeriodStartTick;

        private List<OperatorDef> monthlyUpOperators = new();
        private float nextMonthlyUpdateTime;
        private bool monthlyUpInitialized;

        public static GC_AzurManager Instance => Current.Game?.GetComponent<GC_AzurManager>();

        public int AvaliableCube => storedSilver / SilverExchangeRate;

        public GC_AzurManager(Game game)
        {
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            InitializeUpPool(Find.TickManager.TicksGame);
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            InitializeUpPool(Find.TickManager.TicksGame);
        }

        public override void GameComponentUpdate()
        {
            base.GameComponentUpdate();
            if (!monthlyUpInitialized) return;

            if (Time.realtimeSinceStartup < nextMonthlyUpdateTime) return;
            nextMonthlyUpdateTime = Time.realtimeSinceStartup + UPDATE_INTERVAL;

            int currentTick = Find.TickManager.TicksGame;
            int currentPeriodStartTick = GachaGenerator.GetPeriodStartTick(currentTick);
            if (currentPeriodStartTick != monthlyPeriodStartTick)
            {
                RefreshUpPool(AzurDefOf.LMA_Rander_Operators, currentTick);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref storedSilver, "Ag");
            Scribe_Values.Look(ref monthlyPeriodStartTick, "UpPeriodStartTick");
            Scribe_Collections.Look(ref monthlyUpOperators, "UpOperators", LookMode.Def);

            if (Scribe.mode == LoadSaveMode.PostLoadInit && monthlyUpOperators == null)
            {
                monthlyUpOperators = new List<OperatorDef>();
            }
        }

        public IReadOnlyList<OperatorDef> GetUpOperators(RandomizerDef pool)
        {
            RefreshUpPool(pool, Find.TickManager.TicksGame);
            return monthlyUpOperators;
        }

        public int GetMonthlyUpRemainingTicks()
        {
            int currentTick = Find.TickManager.TicksGame;
            int periodEndTick = GachaGenerator.GetPeriodStartTick(currentTick) + GenDate.TicksPerQuadrum;
            return Mathf.Max(0, periodEndTick - currentTick);
        }

        private void InitializeUpPool(int currentTick)
        {
            RefreshUpPool(AzurDefOf.LMA_Rander_Operators, currentTick);
            nextMonthlyUpdateTime = Time.realtimeSinceStartup + UPDATE_INTERVAL;
            monthlyUpInitialized = true;
        }

        private void RefreshUpPool(RandomizerDef source, int currentTick)
        {
            if (source == null || source.root is not RandomizerNode_Reward_Operator operatorNode)
            {
                throw new InvalidOperationException("[LMA] 无效的卡池节点。");
            }

            //无视了def里的权重配置 未来搞什么轻型重型池子要注意
            List<Rewards_Operator> rewards = operatorNode.Candidates.OfType<Rewards_Operator>().ToList();
            foreach (Rewards_Operator reward in rewards)
            {
                reward.weight = 1;
            }

            List<OperatorDef> candidates = rewards.Select(reward => reward.operatorDef).Distinct().OrderBy(operatorDef => operatorDef.defName, StringComparer.Ordinal).ToList();
            int periodStartTick = GachaGenerator.GetPeriodStartTick(currentTick);
            if (!IsUpPoolValid(periodStartTick, candidates))
            {
                int maximumCount = Mathf.Min(MaxUpOperators, candidates.Count);
                Random random = new(GachaGenerator.SeedMonthly(currentTick));
                int upCount = random.Next(1, maximumCount + 1);
                monthlyUpOperators = GachaGenerator.RequestPerQuadrum(candidates, upCount, currentTick);
                monthlyPeriodStartTick = periodStartTick;
            }

            ApplyWeights(operatorNode, rewards, monthlyUpOperators, UpProbability);
        }

        //当前池子是否过期
        private bool IsUpPoolValid(int periodStartTick, IReadOnlyCollection<OperatorDef> candidates)
        {
            if (monthlyPeriodStartTick != periodStartTick) return false;

            if (monthlyUpOperators == null || monthlyUpOperators.Count < 1 || monthlyUpOperators.Count > Mathf.Min(MaxUpOperators, candidates.Count))
            {
                return false;
            }

            HashSet<OperatorDef> candidateSet = new(candidates);
            HashSet<OperatorDef> upOperatorSet = new();
            return monthlyUpOperators.All(operatorDef => operatorDef != null && candidateSet.Contains(operatorDef) && upOperatorSet.Add(operatorDef));
        }

        private static void ApplyWeights(RandomizerNode_Reward_Operator operatorNode, IReadOnlyCollection<Rewards_Operator> rewards, IReadOnlyCollection<OperatorDef> upOperators, float upProbability)
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
