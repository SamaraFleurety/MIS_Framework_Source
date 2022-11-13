using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class AE_ShootProjectile : AbilityEffectBase
    {
        public VerbProperties verb;
        private Verb_Shoot v = null;

        public override void DoEffect_Pawn(Pawn user, Thing target)
        {
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

            //v.TryStartCastOn(new LocalTargetInfo(user), new LocalTargetInfo(target), false, true, false, false);
        }

    }
}
