using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    /*public class AbilityEffect_Summon : AbilityEffectBase
    {
        public override void DoEffect_IntVec(IntVec3 target,Map map, Pawn caster)
        {
            if (target.GetTerrain(map).passability == Traversability.Impassable || !target.InBounds(map) || target.GetFirstBuilding(map) != null)
            {
                Messages.Message("AK_CanntUseAbility_ThereIsThingInTarget".Translate(), MessageTypeDefOf.CautionInput);
                return;
            };
            //parent.TryGetComp<CompAbility>().Summon();
            PawnKindDef pawnKind = this.summoned;
            Pawn pawn = PawnGenerator.GeneratePawn(pawnKind,Find.FactionManager.OfPlayer);
            Summoned summoned = pawn as Summoned;
            summoned.summoner = parent;
            summoned.summoner_Pawm = ((Apparel)parent).Wearer;
            GenSpawn.Spawn(summoned, target,map);
        }
        public Thing parent;
        public PawnKindDef summoned;
    }*/
}
