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

        public override void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster)
        {
            if (target == null || caster == null)
            {
                return;
            }
            GenExplosion.DoExplosion(target, caster.Map, explosiveRadius, DmgType, null, dmgAmount, dmgPenetration, explosionSound, null, null, null, postExplosionSpawnThingDef, postExplosionSpawnChance, postExplosionSpawnThingCount, null, applyDamageToExplosionCellsNeighbors: true, null, 0f, 0, 0f, false, null, null, null, true, 1f, 0f, true, null, screenShakeFactor: 1f);
        }
    }
}
