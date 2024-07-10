using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AE_CatchMother : AbilityEffectBase
    {
        public int goodwillchange = 30;
        public override void DoEffect_Pawn(Pawn user, Thing target, bool delayed)
        {
            Pawn parent_mother;
            Map map = Find.CurrentMap;
            IntVec3 dropCenter = DropCellFinder.TryFindSafeLandingSpotCloseToColony(map, ThingDefOf.DropPodIncoming.Size, map.ParentFaction);

            if (target == null || !(target is Pawn p))
            {
                return;
            }
            string translatedMessage = TranslatorFormattedStringExtensions.Translate("AKA_Successful_CaughtMother");
            MoteMaker.ThrowText(user.PositionHeld.ToVector3(), user.MapHeld, translatedMessage, 5f);
            string translatedMessage1 = TranslatorFormattedStringExtensions.Translate("AKA_Message_CaughtMother", p.Label);
            Messages.Message(translatedMessage1, MessageTypeDefOf.NeutralEvent);

            parent_mother = p.GetMother();
            if (parent_mother is null)
            {
                PawnKindDef pawnkind = p.kindDef;
                XenotypeDef xenotype = null;
                if (ModLister.BiotechInstalled)
                {
                    xenotype = p.genes.Xenotype;
                }
                parent_mother = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnkind, (Faction)null, PawnGenerationContext.NonPlayer, tile: -1, allowDowned: true, canGeneratePawnRelations: true, fixedGender: Gender.Female, forcedXenotype: xenotype));
                p.SetMother(parent_mother);
            }
            if (parent_mother.Map == map)
            {
                parent_mother.TeleportPawn(dropCenter);
            }
            else
            {
                List<Thing> thingsToSend = new List<Thing>();
                thingsToSend.Add(parent_mother);
                DropPodUtility.DropThingsNear(dropCenter, map, thingsToSend, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false, forbid: false);
            }
            parent_mother.guest.SetGuestStatus(user.Faction, guestStatus: GuestStatus.Prisoner);
            if (parent_mother.Faction != Faction.OfPlayer && parent_mother.Faction != null)
            {
                parent_mother.Faction.TryAffectGoodwillWith(Faction.OfPlayer, goodwillchange);
            }
            CameraJumper.TryJump(new GlobalTargetInfo(dropCenter, map));
        }
    }
    public static class AE_CatchTool
    {
        internal static void TeleportPawn(this Pawn pawn, IntVec3 pos = default)
        {
            if (pawn == null)
            {
                return;
            }
            pos = (pos == default) ? UI.MouseCell() : pos;
            if (pos.InBounds(Find.CurrentMap))
            {
                if (!pawn.Spawned || !pawn.Position.InBounds(Find.CurrentMap) || (pawn.Dead && (pawn.Corpse == null || pawn.Corpse.Position.InBounds(Find.CurrentMap))))
                {
                    pawn.SpawnPawn(pos);
                }
                if (pawn.Dead)
                {
                    pawn.Corpse.Position = pos;
                    string translatedMessage2 = TranslatorFormattedStringExtensions.Translate("AKA_Failured_CatchMother", pawn.Label);
                    Messages.Message(translatedMessage2, MessageTypeDefOf.NeutralEvent);
                    return;
                }
                pawn.Position = pos;
                pawn.Notify_Teleported();
                pawn.Position = pos;
                pawn.Notify_Teleported();
            }
        }
        internal static void SpawnPawn(this Pawn newPawn, IntVec3 pos = default)
        {
            if (newPawn == null)
            {
                return;
            }
            try
            {
                if (newPawn.Dead && newPawn.Corpse != null && newPawn.Corpse.Spawned)
                {
                    newPawn.Corpse.DeSpawn();
                }
                if (newPawn.Spawned)
                {
                    newPawn.DeSpawn();
                }
                if (pos == default)
                {
                    pos = UI.MouseCell();
                    pos.x -= 5;
                }
                if (!pos.InBounds(Find.CurrentMap))
                {
                    pos = Find.CurrentMap.AllCells.RandomElement();
                }
                if (newPawn.Faction.IsZombie())
                {
                    return;
                }
                else
                {
                    GenSpawn.Spawn(newPawn, pos, Find.CurrentMap, Rot4.South);
                }
                if (newPawn.Faction == Faction.OfPlayer)
                {
                    newPawn.playerSettings.hostilityResponse = HostilityResponseMode.Attack;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + "\n" + ex.StackTrace);
            }
        }
        //微乎其微的ZombieLand兼容
        internal static bool IsZombie(this Faction f)
        {
            return !f.IsNullOrEmpty() && f.def.defName == "Zombies";
        }
        internal static bool IsNullOrEmpty(this Faction f)
        {
            return f == null || f.def == null;
        }
    }
}
