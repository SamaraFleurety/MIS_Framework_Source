using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class HediffCompProperties_Bandage : HediffCompProperties
    {
        public HediffCompProperties_Bandage() 
        {
            this.compClass = typeof(HediffComp_Bandage);
        }
        public int tickOfBandageOnce;
        public int bandageCount = 1;
    }
}
