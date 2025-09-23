using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    public class AECommand_Window : Window
    {
        #region
        private readonly Map map;
        //private readonly int margin = 10;
        private int width = 750;
        private readonly string OPID;
        public IntVec3 spot = IntVec3.Invalid;
        public override Vector2 InitialSize => new(850f, 400f);
        public bool randomRewardMod = false;
        private List<RewardDef> rewards;
        private readonly List<RewardDef> rewardPool;
        internal RewardDef choosenReward;
        private readonly int rewardNumber = 4;
        private readonly RewardCategory dynamicPhase = RewardCategory.Poor;
        #endregion
        internal AECommand_Window(Map map, IntVec3 dropSpot, string ID, int phaseNumber)
        {
            dynamicPhase = CheckEnumInt(phaseNumber);
            forcePause = true;
            preventSave = true;
            absorbInputAroundWindow = false;
            doCloseX = false;
            doCloseButton = false;
            closeOnClickedOutside = false;
            closeOnCancel = false;
            doWindowBackground = false;
            drawShadow = false;
            this.map = map;
            spot = dropSpot;
            OPID = ID;
            rewardPool = DefDatabase<RewardDef>.AllDefsListForReading.FindAll((RewardDef r) => r.ID == OPID).ToList();
            //不清楚为什么构造函数内使用if语句会全部返回Empty(List)，暂无其他对ID的检查方式
            /*if (OPID is null)
            {
                rewardPool = DefDatabase<RewardDef>.AllDefsListForReading.FindAll((RewardDef r) => r.ID == null).ToList();
            }
            else if (OPID == "All")
            {
                rewardPool = DefDatabase<RewardDef>.AllDefsListForReading.ToList();
            }
            else
            {
                rewardPool = DefDatabase<RewardDef>.AllDefsListForReading.FindAll((RewardDef r) => r.ID == OPID).ToList();
            }*/
        }
        public override void DoWindowContents(Rect inRect)
        {
            if (!rewards.NullOrEmpty())
            {
                float num = 40f;
                for (int i = 0; i < rewards.Count; i++)
                {
                    Rect rect = new Rect(num, 0f, width, inRect.height).Rounded();
                    Widgets.DrawWindowBackground(rect);
                    rewards.ElementAt(i).DrawCard(rect, this);
                    num = rect.xMax;
                }
                return;
            }
            if (!randomRewardMod)
            {
                width /= rewardNumber;
                rewards = new List<RewardDef>();
                for (int j = 0; j < rewardNumber; j++)
                {
                    RewardDef item = rewardPool.FindAll((RewardDef r) => r.category == dynamicPhase).RandomElement();
                    rewards.Add(item);
                    rewardPool.Remove(item);
                }
                return;
            }
            choosenReward = rewardPool.FindAll((RewardDef r) => r.category == dynamicPhase).RandomElement();
            Close();
        }
        public override void PostClose()
        {
            Messages.Message("空投成功", MessageTypeDefOf.NeutralEvent);
            AE_RewardTool.SendSupply(choosenReward, map, dropSpot: spot);
        }
        public RewardCategory CheckEnumInt(int value)
        {
            if (Enum.IsDefined(typeof(RewardCategory), value))
            {
                return (RewardCategory)value;
            }
            else
            {
                Log.Warning($"The value {value} does not correspond to a valid RewardCategory.");
                throw new ArgumentOutOfRangeException(nameof(value), $"The value {value} does not correspond to a valid RewardCategory.");
            }
        }
    }
}
