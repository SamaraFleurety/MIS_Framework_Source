using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class HediffCompProperties_Regrow : HediffCompProperties
    {
        public int healInterval = 60;

        public float healAmount = 1f;

        public HediffCompProperties_Regrow()
        {
            this.compClass = typeof(HediffComp_Regrow);
        }
    }
}
