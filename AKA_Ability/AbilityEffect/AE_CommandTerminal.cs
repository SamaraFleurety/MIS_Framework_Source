using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using System.Net.NetworkInformation;

namespace AKA_Ability
{
    public class AE_CommandTerminal : AbilityEffectBase
    {
        public int DelayTime = 142;
        public override void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster)
        {
            if (caster == null) return;
            CallSupplyDropPod(target, map);
        }
        public static void CallSupplyDropPod(IntVec3 target, Map map, Pawn caster = null)
        {
            AKA_RewardDef reward = new AKA_RewardDef();
            List<Thing> thingsToSend = new List<Thing>();
            GenerateItems(reward, ref thingsToSend);

            if (map == null) map = Find.CurrentMap;
            string translatedMessage = TranslatorFormattedStringExtensions.Translate("AK_SuccessfulCallSupplyDropPod");
            MoteMaker.ThrowText(caster.PositionHeld.ToVector3(), caster.MapHeld, translatedMessage, 2f);
            // 生成空投舱的位置必须是空的且无障碍
            IntVec3 dropCenter = (target != IntVec3.Invalid) ? target : DropCellFinder.TryFindSafeLandingSpotCloseToColony(map, ThingDefOf.DropPodIncoming.Size, map.ParentFaction);
            DropPodUtility.DropThingsNear(dropCenter, map, thingsToSend, 142, canInstaDropDuringInit: false, false, canRoofPunch: true, forbid: false);
        }
        private static void GenerateItems(AKA_RewardDef reward, ref List<Thing> thingsToSend)
        {
            if (reward.items.NullOrEmpty()) return;

        }
    }
}