using AKA_Ability.AbilityEffect;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_Summon : AKAbility_Targetor
    {
        public HashSet<Thing> summoneds = new();

        private AE_SummonBase effector = null;

        public AE_SummonBase Effector
        {
            get
            {
                if (effector == null)
                {
                    foreach (AbilityEffectBase ae in def.compEffectList)
                    {
                        if (ae is AE_SummonBase aes)
                        {
                            effector = aes;
                            break;
                        }
                    }
                }
                return effector;
            }
        }

        public AKAbility_Summon(AbilityTracker tracker) : base(tracker)
        {
        }
        public AKAbility_Summon(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref summoneds, "summons", LookMode.Reference);
        }
    }
}
