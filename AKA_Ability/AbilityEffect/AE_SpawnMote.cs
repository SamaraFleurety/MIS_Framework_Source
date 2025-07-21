using RimWorld;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_SpawnMote : AbilityEffectBase
    {
        ThingDef moteDef = null;
        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn == null) return false;

            /*Mote mote = (Mote)ThingMaker.MakeThing(moteDef);
            //mote.Attach(caster.CasterPawn);
            GenSpawn.Spawn(mote, target.Cell, caster.CasterPawn.Map);
            Log.Message($"spawning mote at {target.Cell.x}, {target.Cell.z}; {caster.CasterPawn.Map}");*/

            MoteMaker.MakeStaticMote(target.Cell, caster.CasterPawn.Map, moteDef);
            return base.DoEffect(caster, target);
        }
    }
}
