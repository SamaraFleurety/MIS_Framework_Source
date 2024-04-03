using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AE_ShootProjectile : AbilityEffectBase
    {
        public VerbProperties verb;
        private Verb_Shoot v = null;

        public override void DoEffect_Pawn(Pawn user, Thing target, bool delayed)
        {
            Log.Message($"delayed? {delayed}");
            if (v == null)
            {
                v = (Verb_Shoot)Activator.CreateInstance(verb.verbClass);
                v.verbProps = verb;
                v.caster = user;
                v.verbTracker = new VerbTracker(user);
            }
            //直接硬写的强命 看不懂原版的命中系统
            ShootLine shootLine = new ShootLine(user.Position, user.TargetCurrentlyAimingAt.Cell);

            Projectile proj = (Projectile)GenSpawn.Spawn(verb.defaultProjectile, shootLine.Source, user.Map, WipeMode.Vanish);
            proj.Launch(user, user.DrawPos, user.TargetCurrentlyAimingAt, user.TargetCurrentlyAimingAt, ProjectileHitFlags.All, false, null, null);

            if (!delayed)
            {
                Log.Message("add delayed");
                int delayBase = verb.ticksBetweenBurstShots;
                if (delayBase <= 0) delayBase = 1;
                uint delay = (uint)delayBase;
                int burstCnt = verb.burstShotCount - 1;
                DelayedAbility delayedAbility = new DelayedAbility(user, null, target, null, target.Map, this);
                while (burstCnt > 1)
                {
                    GC_DelayedAbilityManager.AddDelayedAbilities(delay, delayedAbility);
                    burstCnt -= 1;
                    delay += (uint)delayBase;
                }
            }

            //v.TryStartCastOn(new LocalTargetInfo(user), new LocalTargetInfo(target), false, true, false, false);
        }

    }
}
