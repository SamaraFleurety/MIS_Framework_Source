using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace AKA_Ability
{
    /*public class Verb_Reclaim : Verb_CastBase
    {
        protected override bool TryCastShot()
        {
            Pawn casterPawn = this.CasterPawn;
            ThingWithComps apparel = base.EquipmentSource;
            //CompAbility compAbility = apparel.TryGetComp<CompAbility>();
            Pawn target = this.currentTarget.Pawn as Pawn;
            if (casterPawn == null || casterPawn.Dead || apparel == null || target == null|| target.Dead||target.kindDef != pawn||((Summoned)target).summoner_Pawm != casterPawn)
            {
                Messages.Message("AK_CanntReclaim".Translate(),MessageTypeDefOf.CautionInput);
                return false;
            }
            else 
            {
                target.Destroy();
                //compAbility.SummonedDead();
                return true;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref pawn,"pawn");
        }
        public PawnKindDef pawn;
    }*/
}
