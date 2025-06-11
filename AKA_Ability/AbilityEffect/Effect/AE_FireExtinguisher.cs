using Verse;

namespace AKA_Ability
{
    public class AE_Extinguisher : AbilityEffectBase
    {
        public DamageDef DmgType;
        public int dmgAmount = -1;
        public float dmgPenetration = -1f;
        public float explosiveRadius;
        //生成泡沫之类的后效Thing
        public ThingDef postExplosionSpawnThingDef = null;
        public float postExplosionSpawnChance = 0;
        public int postExplosionSpawnThingCount = 0;
        public SoundDef explosionSound = null;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn == null) return false;

            IntVec3 targetCell = target.Cell;
            Pawn casterPawn = caster.CasterPawn;

            if (/*targetCell == null ||*/ casterPawn == null || casterPawn.Map == null || !targetCell.InBounds(casterPawn.Map))
            {
                return false;
            }
            GenExplosion.DoExplosion(targetCell, casterPawn.Map, explosiveRadius, DmgType, null, dmgAmount, dmgPenetration, explosionSound, null, null, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, null, null, 255, applyDamageToExplosionCellsNeighbors: true, null, 0f, 0, 0f, false, null, null, null, true, 1f, 0f, true, null, screenShakeFactor: 1f);

            return base.DoEffect(caster, target);
        }

    }
}
