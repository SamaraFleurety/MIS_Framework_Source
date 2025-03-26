using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_ShootProjectile : AbilityEffectBase
    {
        ThingDef projectile;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            Pawn casterPawn = caster.CasterPawn;
            ShootLine shootLine = new ShootLine(casterPawn.Position, target.Cell);

            Projectile proj = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, casterPawn.Map, WipeMode.Vanish);
            proj.Launch(casterPawn, casterPawn.DrawPos, target, target, ProjectileHitFlags.All, false, null, null);
            return base.DoEffect(caster, target);
        }
    }
}
