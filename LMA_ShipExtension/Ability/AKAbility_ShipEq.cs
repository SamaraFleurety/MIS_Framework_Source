using AKA_Ability;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LMA_Lib.Ability
{
    //有点像Targetor + AutoCast
    //fixme 逻辑好像不对
    public class AKAbility_ShipEq : AKAbility_Targetor
    {
        LMA_AbilityTracker Container => (LMA_AbilityTracker)container;
        protected virtual LocalTargetInfo Target
        {
            get
            {
                if (Container.shipEqTargets.TryGetValue(ID, out LocalTargetInfo target) && target != null)
                {
                    return target;
                }
                if (Container.shipEqTargets.TryGetValue(-1, out LocalTargetInfo altTarget) && altTarget != null)
                {
                    return altTarget;
                }
                return null;
            }
        }
        public AKAbility_ShipEq(AbilityTracker tracker) : base(tracker)
        {
        }

        public AKAbility_ShipEq(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        public override void Tick()
        {
            base.Tick();

            if (Find.TickManager.TicksGame % AKA_Ability.TypeDef.AUTO_ABILITY_TICK_INTERVAL == 0 && CastableNow())
            {
                if (Target == null || !Target.IsValid) return;
                TryCastShot(Target);
            }
        }
    }
}
