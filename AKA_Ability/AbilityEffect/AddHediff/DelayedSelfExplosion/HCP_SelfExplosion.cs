﻿using System;
using Verse;

namespace AKA_Ability
{
    [Obsolete]
    public class HCP_SelfExplosion : HediffCompProperties
    {
        public int afterTicks = 1;
        public float radius = 2.2f;
        public int damage = 10;

        public HCP_SelfExplosion()
        {
            this.compClass = typeof(HC_SelfExplosion);
        }
    }
}
