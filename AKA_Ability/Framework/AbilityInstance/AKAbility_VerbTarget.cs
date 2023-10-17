using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_VerbTarget : AKAbility
    {
        public AKAbility_VerbTarget() : base()
        {
        }

        public AKAbility_VerbTarget(AKAbilityDef def) : base(def)
        {
        }

        protected override void InitializeGizmo()
        {
            cachedGizmo = new Command_AKAbility
            {
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon,
                verb = new Verb_AbilityTargeting
                {
                    verbProps = def.verb,
                    caster = CasterPawn,
                    verbTracker = new VerbTracker(CasterPawn),
                },
                ability = this
            };
        }
    }
}
