using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class AKAbility_Toggle : AKAbility
    {
        private Command_Toggle cachedGizmo = null;

        private bool AutoCast = false;

        private LocalTargetInfo Target => CasterPawn.TargetCurrentlyAimingAt;

        public AKAbility_Toggle(OperatorAbilityDef def) : base(def)
        {
        }

        public override void Tick()
        {
            base.Tick();
            if (AutoCast)
            {
                if (AutoCast && CoolDown.charge >= 1 && Find.TickManager.TicksGame % 180 == 0)
                {
                    if (!CasterPawn.Drafted || Target == null) return;
                    //this.ability_Command.verb.TryStartCastOn(new LocalTargetInfo(this.Document.pawn), target); 
                    foreach (AbilityEffectBase compEffect in this.def.compEffectList)
                    {
                        compEffect.DoEffect_Pawn(CasterPawn, Target.Pawn);
                        compEffect.DoEffect_IntVec(Target.Cell, CasterPawn.Map, CasterPawn);
                    }
                    UseOneCharge();
                }
            }
        }

        public override Gizmo GetGizmo()
        {
            if (cachedGizmo == null) InitializeGizmo();
            return cachedGizmo;
        }

        private void InitializeGizmo()
        {
            cachedGizmo = new Command_Toggle
            {
                toggleAction = delegate ()
                {
                    AutoCast = !AutoCast;
                },
                isActive = delegate()
                {
                    return AutoCast;
                },
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon
            };
        }
    }
}
