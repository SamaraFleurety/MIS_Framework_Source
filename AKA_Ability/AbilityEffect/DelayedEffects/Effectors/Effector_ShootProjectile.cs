using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.DelayedEffects
{
    public class Effector_ShootProjectile : DelayedEffectorBase
    {
        //直接写 估计泛用不了
        ThingDef Projectile => effectDef.projectile;
        //VerbProperties verbProperties;

        //好像没使用
        //Verb verb = null;

        public Effector_ShootProjectile(DelayedEffectDef effectDef, Pawn casterPawn, LocalTargetInfo targetInfo) : base(effectDef, casterPawn, targetInfo)
        {
        }

        /*Verb Verb
        {
            get
            {
                if (verb == null)
                {
                    verb = (Verb_Shoot)Activator.CreateInstance(verbProperties.verbClass);
                    verb.verbProps = verbProperties;
                    verb.caster = casterPawn;
                    verb.verbTracker = new VerbTracker(casterPawn);
                }
                return verb;
            }
        }*/

        //射出一发子弹
        public override void DoEffect()
        {
            //Log.Message($"[ak] do effect at type {this.GetType()}");
            ShootLine shootLine = new ShootLine(casterPawn.Position, targetInfo.Cell);

            Projectile proj = (Projectile)GenSpawn.Spawn(Projectile, shootLine.Source, casterPawn.Map, WipeMode.Vanish);
            proj.Launch(casterPawn, casterPawn.DrawPos, targetInfo, targetInfo, ProjectileHitFlags.All, false, null, null);
        }
    }
}
