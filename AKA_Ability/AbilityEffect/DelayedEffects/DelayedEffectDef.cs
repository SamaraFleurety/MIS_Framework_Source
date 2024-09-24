using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.DelayedEffects
{
    public class DelayedEffectDef : Def
    {
        public Type delayedEffector;

        public ThingDef projectile;
    }
}
