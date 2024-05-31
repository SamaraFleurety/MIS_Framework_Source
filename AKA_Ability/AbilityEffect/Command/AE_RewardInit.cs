using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    [StaticConstructorOnStartup]
    public static class AE_RewardInit
    {
        public static Dictionary<RewardCategory, List<RewardDef>> rewardsPerCat;
        static AE_RewardInit()
        {
            rewardsPerCat = new Dictionary<RewardCategory, List<RewardDef>>
            {
                {
                    RewardCategory.Poor,
                    new List<RewardDef>()
                },
                {
                    RewardCategory.Normal,
                    new List<RewardDef>()
                },
                {
                    RewardCategory.Good,
                    new List<RewardDef>()
                },
                {
                    RewardCategory.Excellent,
                    new List<RewardDef>()
                },
                {
                    RewardCategory.Legendary,
                    new List<RewardDef>()
                }
            };
            List<RewardDef> allDefsListForReading = DefDatabase<RewardDef>.AllDefsListForReading;
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                RewardDef rewardDef = allDefsListForReading[i];
                rewardsPerCat[rewardDef.category].Add(rewardDef);
            }
        }
    }
}
