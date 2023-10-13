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
        protected override void InitializeGizmo()
        {
            cachedGizmo = new Command_VerbTarget
            {
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon,
                verb = new Verb_AbilityTargeting
                {
                    verbProps = def.verb,
                    caster = CasterPawn,
                    verbTracker = new VerbTracker(CasterPawn)
                }
            };
        }
    }
}
