using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    internal static class AE_RewardTool
    {
        public static int Setdelaytime = 142;
        //空投舱打开延迟时间
        public static int Delaytimer
        {
            get { return Setdelaytime; }
            set { Setdelaytime = value; }
        }
        public static Dictionary<RewardCategory, List<RewardDef>> rewardsPerCat = new Dictionary<RewardCategory, List<RewardDef>>
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
        public static void FindAllDefsListForReading(string OPID)
        {
            List<RewardDef> allDefsListForReading = DefDatabase<RewardDef>.AllDefsListForReading.FindAll((RewardDef r) => r.ID == OPID);
            for (int i = 0; i < allDefsListForReading.Count; i++)
            {
                RewardDef rewardDef = allDefsListForReading[i];
                rewardsPerCat[rewardDef.category].Add(rewardDef);
            }
        }
        public static void SendSupply(RewardDef reward, Map map, IntVec3 dropSpot)
        {
            if (reward.sendRewardOf > RewardCategory.Poor)
            {
                RewardDef rewardDef = rewardsPerCat[reward.sendRewardOf].RandomElement();
                Messages.Message("AKA.RandRewardOutcome".Translate(rewardDef.LabelCap), MessageTypeDefOf.NeutralEvent);
                SendSupply(rewardDef, map, dropSpot);
                return;
            }
            if (reward.incidentDef != null)
            {
                Find.Storyteller.incidentQueue.Add(reward.incidentDef, Find.TickManager.TicksGame, new IncidentParms
                {
                    target = map
                });
            }
            if (reward.massHeal)
            {
                HealAllPawns(map.mapPawns.FreeColonistsSpawned);
                HealAllPawns(map.mapPawns.SlavesAndPrisonersOfColonySpawned);
            }
            if (reward.unlockXResearch > 0)
            {
                for (int i = 0; i < reward.unlockXResearch; i++)
                {
                    List<ResearchProjectDef> list = DefDatabase<ResearchProjectDef>.AllDefsListForReading.FindAll((ResearchProjectDef x) => x.CanStartNow);
                    if (!list.NullOrEmpty())
                    {
                        ResearchProjectDef researchProjectDef = list.RandomElement();
                        Find.ResearchManager.FinishProject(researchProjectDef);
                        Messages.Message("AKA.ResearchUnlocked".Translate(researchProjectDef.LabelCap), MessageTypeDefOf.NeutralEvent);
                    }
                }
            }
            List<Thing> thingsToSend = new List<Thing>();
            GenerateRandomItems(reward, ref thingsToSend);
            GenerateItems(reward, ref thingsToSend);
            if (thingsToSend.Count > 0)
            {
                if (map == null) { map = Find.CurrentMap; }
                // 生成空投舱的位置必须是空的且无障碍
                IntVec3 dropCenter = ((dropSpot != IntVec3.Invalid) ? dropSpot : DropCellFinder.TryFindSafeLandingSpotCloseToColony(map, ThingDefOf.DropPodIncoming.Size, map.ParentFaction));
                DropPodUtility.DropThingsNear(dropCenter, map, thingsToSend, Delaytimer, canInstaDropDuringInit: false, leaveSlag: true, canRoofPunch: false, forbid: false);
                //Log.Message("空投 成功");
            }
        }
        private static void GenerateItems(RewardDef reward, ref List<Thing> thingsToSend)
        {
            if (reward.items.NullOrEmpty()) return;
            for (int i = 0; i < reward.items.Count; i++)
            {
                ItemReward itemReward = reward.items[i];
                int num = itemReward.count;
                ThingDef thing = itemReward.thing;
                while (num > 0)
                {
                    Thing thingpack = ((thing.CostStuffCount <= 0) ? ThingMaker.MakeThing(thing) : ThingMaker.MakeThing(thing, GenStuff.RandomStuffFor(thing)));
                    thingpack.TryGetComp<CompQuality>()?.SetQuality(((int)itemReward.quality > 0) ? itemReward.quality : QualityUtility.GenerateQualityRandomEqualChance(), ArtGenerationContext.Outsider);
                    num -= (thingpack.stackCount = Math.Min(num, thing.stackLimit));
                    if (thingpack.def.minifiedDef != null)
                    {
                        thingpack = thingpack.MakeMinified();
                    }
                    thingsToSend.Add(thingpack);
                }
            }
        }
        private static void GenerateRandomItems(RewardDef reward, ref List<Thing> thingsToSend)
        {
            if (reward.randomItems.NullOrEmpty())
            {
                return;
            }
            for (int i = 0; i < reward.randomItems.Count; i++)
            {
                RandItemReward item = reward.randomItems[i];
                List<ThingDef> list = (item.randomFrom.NullOrEmpty() ? DefDatabase<ThingDef>.AllDefsListForReading.FindAll((ThingDef t) => item.thingCategories.Any((ThingCategoryDef c) => t.IsWithinCategory(c)) && t.tradeability != 0 && !t.destroyOnDrop && t.BaseMarketValue > 0f) : item.randomFrom);
                if (!item.excludeThingCategories.NullOrEmpty())
                {
                    list.RemoveAll((ThingDef t) => item.excludeThingCategories.Any((ThingCategoryDef c) => t.IsWithinCategory(c)));
                }
                int num = item.count;
                while (num > 0)
                {
                    ThingDef thingDef = list.RandomElement();
                    Thing thing = ((thingDef.CostStuffCount <= 0) ? ThingMaker.MakeThing(thingDef) : ThingMaker.MakeThing(thingDef, GenStuff.RandomStuffFor(thingDef)));
                    thing.TryGetComp<CompQuality>()?.SetQuality(((int)item.quality > 0) ? item.quality : QualityUtility.GenerateQualityRandomEqualChance(), ArtGenerationContext.Outsider);
                    num = (thingDef.isTechHediff ? (num - 1) : (num - (thing.stackCount = Math.Min(num, thing.def.stackLimit))));
                    if (thing.def.minifiedDef != null)
                    {
                        thing = thing.MakeMinified();
                    }
                    thingsToSend.Add(thing);
                }
            }
        }
        private static void HealAllPawns(List<Pawn> pawns)
        {
            for (int i = 0; i < pawns.Count; i++)
            {
                HealEverything(pawns[i]);
            }
        }
        private static void HealEverything(Pawn p)
        {
            if (p.health == null || p.health.hediffSet == null || p.health.hediffSet.hediffs.NullOrEmpty())
            {
                return;
            }
            List<Hediff> list = p.health.hediffSet.hediffs.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is Hediff_Injury hediff)
                {
                    p.health.RemoveHediff(hediff);
                }
                else if (list[i] is Hediff_MissingPart hediff_MissingPart && hediff_MissingPart.Part.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore) && (hediff_MissingPart.Part.parent == null || p.health.hediffSet.GetNotMissingParts().Contains(hediff_MissingPart.Part.parent)))
                {
                    p.health.RestorePart(hediff_MissingPart.Part);
                }
            }
        }
    }
}
