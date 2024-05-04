using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    /*public class AbilityEffect_Trap : AbilityEffectBase
    {
        public override void DoEffect_IntVec(IntVec3 target,Map map,Pawn caster)
        {
            if (target.GetTerrain(map).passability == Traversability.Impassable || !target.InBounds(map) || target.GetFirstBuilding(map) != null)
            {
                Messages.Message("AK_CanntUseAbility_ThereIsThingInTarget".Translate(), MessageTypeDefOf.CautionInput);
                return;
            };
            Trap trap = ThingMaker.MakeThing(this.trap) as Trap;
            trap.duration = this.duration;
            GenSpawn.Spawn(trap,target,map);
        }
        public int duration;
        public ThingDef trap;
    }*/
}