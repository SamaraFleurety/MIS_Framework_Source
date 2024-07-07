using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect.Effect
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

            parent_mother = p.GetMother();
            if (parent_mother is null)
            {
                PawnKindDef pawnkind = p.kindDef;
                Faction FactionOfPawn = p.Faction;

                parent_mother = PawnGenerator.GeneratePawn(new PawnGenerationRequest(pawnkind, FactionOfPawn, PawnGenerationContext.NonPlayer, tile: -1, allowDowned: true, canGeneratePawnRelations: true, fixedGender: Gender.Female));
                p.SetMother(parent_mother);
            }
            parent_mother.guest.SetGuestStatus(user.Faction, guestStatus: GuestStatus.Prisoner);
            thingsToSend.Add(parent_mother);
            IntVec3 dropCenter = DropCellFinder.TryFindSafeLandingSpotCloseToColony(map, ThingDefOf.DropPodIncoming.Size, map.ParentFaction);
            DropPodUtility.DropThingsNear(dropCenter, map, thingsToSend, 110, canInstaDropDuringInit: false, leaveSlag: true, canRoofPunch: false, forbid: false);
        }
    }
}
