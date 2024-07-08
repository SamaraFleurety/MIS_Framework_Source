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
        public override void DoEffect_Pawn(Pawn user, Thing target, bool delayed)
        {
            Pawn parent_mother;
            Map map = Find.CurrentMap;
            List<Thing> thingsToSend = new List<Thing>();

            if (target == null || !(target is Pawn p))
            {
                return;
            }
            //翻译xml文件写上:我要把你的马马抓走！
            string translatedMessage = TranslatorFormattedStringExtensions.Translate("AKA_Successful_CaughtMother");
            MoteMaker.ThrowText(user.PositionHeld.ToVector3(), user.MapHeld, translatedMessage, 5f);
            Messages.Message(p.Name + "的妈妈  被抓来了!", MessageTypeDefOf.NeutralEvent);
            parent_mother = p.GetMother();
            IntVec3 dropCenter = DropCellFinder.TryFindSafeLandingSpotCloseToColony(map, ThingDefOf.DropPodIncoming.Size, map.ParentFaction);
            if (parent_mother is null)
            {
                PawnKindDef pawnkind = p.kindDef;
                //Faction FactionOfPawn = p.Faction;
                parent_mother = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnkind, (Faction)null, PawnGenerationContext.NonPlayer, tile: -1, allowDowned: true, canGeneratePawnRelations: true, fixedGender: Gender.Female));
                p.SetMother(parent_mother);
                thingsToSend.Add(parent_mother);
                DropPodUtility.DropThingsNear(dropCenter, map, thingsToSend, 110, canInstaDropDuringInit: false, leaveSlag: false, canRoofPunch: false, forbid: false);
            }
            else
            {
                parent_mother.TeleportPawn(dropCenter);
            }
            parent_mother.guest.SetGuestStatus(user.Faction, guestStatus: GuestStatus.Prisoner);
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
                    Messages.Message("不能抓走" + pawn.Name + "的妈妈，她已经死了!", MessageTypeDefOf.NeutralEvent);
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
