using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_Toggle : AKAbility
    {
        public bool AutoCast = false;

        private LocalTargetInfo Target => CasterPawn.TargetCurrentlyAimingAt;

        public AKAbility_Toggle() : base()
        {

        }

        public AKAbility_Toggle(AKAbilityDef def) : base(def)
        {
        }

        public override void Tick()
        {
            base.Tick();
            if (AutoCast)
            {
                if (AutoCast && cooldown.charge >= 1 && Find.TickManager.TicksGame % 180 == 0)
                {
                    if (!CasterPawn.Drafted || Target == null) return;
                    //this.ability_Command.verb.TryStartCastOn(new LocalTargetInfo(this.Document.pawn), target); 
                    foreach (AbilityEffectBase compEffect in this.def.compEffectList)
                    {
                        compEffect.DoEffect_All(CasterPawn, CasterPawn.TargetCurrentlyAimingAt);
                        /*compEffect.DoEffect_Pawn(CasterPawn, Target.Pawn);
                        compEffect.DoEffect_IntVec(Target.Cell, CasterPawn.Map, CasterPawn);*/
                    }
                    UseOneCharge();
                }
            }
        }

        protected override void InitializeGizmo()
        {
            cachedGizmo = new Command_AKAbility
            {
                Action = delegate ()
                {
                    AutoCast = !AutoCast;
                },
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon,
                ability = this
            };
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref AutoCast, "auto");
        }
    }
}
