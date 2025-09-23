using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_ShootProjectile : AbilityEffectBase
    {
        ThingDef projectile = null;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn == null) return false;

            Pawn casterPawn = caster.CasterPawn;
            ShootLine shootLine = new(casterPawn.Position, target.Cell);

            Projectile proj = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, casterPawn.Map, WipeMode.Vanish);
            proj.Launch(casterPawn, casterPawn.DrawPos, target, target, ProjectileHitFlags.All, false, null, null);
            return base.DoEffect(caster, target);
        }
    }
}
