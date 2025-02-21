using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_SpawnThing : AbilityEffectBase
    {
        public ThingDef summoned;

        public ThingDef stuff = null;

        public bool setFaction = false;

        float radius = 0;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            var thing = ThingMaker.MakeThing(summoned, stuff);
            if (setFaction) thing.SetFactionDirect(caster.CasterPawn.Faction);
            GenPlace.TryPlaceThing(thing, target.Cell + offset, caster.CasterPawn.Map, ThingPlaceMode.Near);

            return base.DoEffect(caster, target);
        }
        protected IntVec3 offset
        {
            get
            {
                if (radius < 1) return IntVec3.Zero;
                int maxExclusive = GenRadial.NumCellsInRadius(radius);
                int num = Rand.Range(0, maxExclusive);
                return GenRadial.RadialPattern[num];
            }
        }
    }
}
