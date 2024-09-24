using AKA_Ability.Gizmos;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_Auto : AKAbility
    {
        public bool AutoCast = false;

        //这是自动瞄准的目标
        protected virtual LocalTargetInfo Target => CasterPawn.TargetCurrentlyAimingAt;

        public AKAbility_Auto(AbilityTracker tracker) : base(tracker)
        {
        }
        public AKAbility_Auto(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        public override void Tick()
        {
            base.Tick();
            if (AutoCast && cooldown.charge >= 1 && Find.TickManager.TicksGame % 180 == 0)
            {
                if (!CasterPawn.Drafted || Target == null) return;
                TryCastShot(Target);
            }

        }

        protected override void InitializeGizmo()
        {
            cachedGizmo = new Gizmo_AbilityCast_Toggle
            {
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon,
                parent = this,
            };

            /*cachedGizmo = new Command_AKAbility
            {
                Action = delegate ()
                {
                    AutoCast = !AutoCast;
                },
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon,
                ability = this,
                verb = new Verb_AbilityTargeting()  //假的 不会使用 给VEF的MVCF功能留的兼容
            };*/
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AutoCast, "auto");
        }
    }
}
