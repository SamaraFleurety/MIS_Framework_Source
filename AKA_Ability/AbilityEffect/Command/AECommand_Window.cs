using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    public class AECommand_Window : Window
    {
        private readonly Map map;
        //private readonly int margin = 10;
        private int width = 750;
        private List<RewardDef> rewards;
        private readonly List<RewardDef> rewardPool;
        internal RewardDef choosenReward;
        private readonly int rewardNumber = 4;
        private readonly RewardCategory dynamicPhase = RewardCategory.Poor;
        public bool randomRewardMod = false;
        public IntVec3 spot = IntVec3.Invalid;
        public override Vector2 InitialSize => new Vector2(850f, 500f);

        internal AECommand_Window(Map map, IntVec3 dropSpot, int phaseNumber)
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
            rewardPool = DefDatabase<RewardDef>.AllDefsListForReading.ToList();
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
