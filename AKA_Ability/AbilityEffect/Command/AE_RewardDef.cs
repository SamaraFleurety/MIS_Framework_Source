using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    public class RewardDef : Def
    {
        public string texPath;
        public string ID;
        public RewardCategory category;
        public RewardCategory sendRewardOf = RewardCategory.Poor;
        public List<ItemReward> items;
        public List<RandItemReward> randomItems;
        public IncidentDef incidentDef;
        public int unlockXResearch = 0;
        public bool massHeal = false;
        private Texture2D rewardIcon;
        public Texture2D RewardIcon
        {
            get
            {
                if (rewardIcon is null)
                {
                    rewardIcon = ((texPath == null) ? null : ContentFinder<Texture2D>.Get(texPath, reportFailure: false));
                    if (rewardIcon is null)
                    {
                        rewardIcon = BaseContent.BadTex;
                    }
                }
                return rewardIcon;
            }
        }
        public string Category
        {
            get
            {
                if (category == RewardCategory.Poor)
                    return "AKA.Poor".Translate();
                else if (category == RewardCategory.Normal)
                    return "AKA.Normal".Translate();
                else if (category == RewardCategory.Good)
                    return "AKA.Good".Translate();
                else if (category == RewardCategory.Excellent)
                    return "AKA.Excellent".Translate();
                else if (category == RewardCategory.Legendary)
                    return "AKA.Legendary".Translate();
                else
                    throw new InvalidOperationException("Unexpected RewardCategory value.");
            }
        }
        //递归功能不可用可还行？
        /*public string Category => category switch
        {
            RewardCategory.Poor => "AKA.Poor".Translate(),
            RewardCategory.Normal => "AKA.Normal".Translate(),
            RewardCategory.Good => "AKA.Good".Translate(),
            RewardCategory.Excellent => "AKA.Excellent".Translate(),
            RewardCategory.Legendary => "AKA.Legendary".Translate(),
        };*/
        public void DrawCard(Rect rect, AECommand_Window window)
        {
            Rect rect2 = new Rect(rect.x, rect.y, rect.width, rect.width);
            GUI.DrawTexture(rect2.ContractedBy(25f), RewardIcon);
            TextAnchor anchor = Text.Anchor;
            Text.Anchor = TextAnchor.UpperCenter;
            Text.Font = GameFont.Small;
            Rect rect3 = new Rect(rect.x, rect2.yMax + 5f, rect.width, 20f);
            Widgets.Label(rect3, label);
            Text.Font = GameFont.Tiny;
            Rect rect4 = new Rect(rect.x, rect3.yMax + 10f, rect.width, 20f);
            Widgets.Label(rect4, "AKA.Reward".Translate(Category));
            Rect rect5 = new Rect(rect.x, rect4.yMax + 5f, rect.width, 70f);
            Widgets.Label(rect5.ContractedBy(5f), description);
            Rect rect6 = new Rect(rect.x, rect5.yMax - 15f, rect.width, 45f);
            Rect rect7 = rect6.ContractedBy(5f);
            if (Widgets.ButtonText(rect7, "AKA.SelectReward".Translate()))
            {
                window.choosenReward = this;
                window.Close();
            }
            Text.Font = GameFont.Small;
            Text.Anchor = anchor;
        }
    }
    public class ItemReward
    {
        public ThingDef thing;

        public QualityCategory quality;

        public int count;
    }
    public class RandItemReward
    {
        public List<ThingCategoryDef> thingCategories;

        public List<ThingCategoryDef> excludeThingCategories;

        public QualityCategory quality;

        public int count;

        public List<ThingDef> randomFrom;
    }
    public enum RewardCategory
    {
        Poor,
        Normal,
        Good,
        Excellent,
        Legendary
    }
}
