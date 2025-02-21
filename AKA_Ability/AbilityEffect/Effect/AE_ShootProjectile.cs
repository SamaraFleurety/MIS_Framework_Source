using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class AE_ShootProjectileCircularError : AbilityEffectBase
    {
        ThingDef projectile;

        float missRadius = 0;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            Pawn casterPawn = caster.CasterPawn;
            ShootLine shootLine = new ShootLine(casterPawn.Position, target.Cell);

            Projectile proj = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, casterPawn.Map, WipeMode.Vanish);
            proj.Launch(casterPawn, casterPawn.DrawPos, GetForcedMissTarget(target), target, ProjectileHitFlags.All, false, null, null);
            return base.DoEffect(caster, target);
        }
        protected IntVec3 GetForcedMissTarget(LocalTargetInfo target)
        {
            int maxExclusive = GenRadial.NumCellsInRadius(missRadius);
            int num = Rand.Range(0, maxExclusive);
            return target.Cell + GenRadial.RadialPattern[num];
        }
    }
}
