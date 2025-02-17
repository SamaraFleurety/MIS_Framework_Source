using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;
using static UnityEngine.GraphicsBuffer;

namespace AKA_Ability
{
    public class Projectile_Custom : Bullet
    {
        protected ModExtension_Projectile extension => def.GetModExtension<ModExtension_Projectile>();

        public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
        {
            base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
            LaunchEffect(usedTarget);
        }

        public virtual void LaunchEffect(LocalTargetInfo target)
        {
            if (extension?.launchEffecter != null)
            {
                Effecter effecter = extension.launchEffecter.Spawn();
                effecter.scale = extension.muzzleScale;
                effecter.offset = Vector3.forward.RotatedBy((target.Cell.ToVector3Shifted() - DrawPos).Yto0().AngleFlat()) * extension.muzzleOffset;
                effecter.Trigger(launcher, target.ToTargetInfo(launcher.Map));
            }
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (!blockedByShield && !def.projectile.soundImpact.NullOrUndefined())
            {
                def.projectile.soundImpact.PlayOneShot(SoundInfo.InMap(this));
            }
            if (extension?.hitEffecter != null)
            {
                var hitPosV3 = hitThing == null ? ExactPosition : hitThing.DrawPos;

                Effecter effecter = extension.hitEffecter.Spawn();
                effecter.scale = extension.hitEffecterScale;
                effecter.offset = hitPosV3 - hitPosV3.ToIntVec3().ToVector3Shifted();
                effecter.Trigger(new TargetInfo(hitPosV3.ToIntVec3(), Map), launcher);
            }
            base.Impact(hitThing, blockedByShield);
        }
    }
    public class ModExtension_Projectile : DefModExtension
    {
        public EffecterDef launchEffecter;
        public float muzzleOffset = 0.7f;
        public float muzzleScale = 1;

        public EffecterDef hitEffecter;
        public float hitEffecterScale = 1;
    }
}
