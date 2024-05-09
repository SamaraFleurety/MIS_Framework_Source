using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class HediffComp_Heal : HediffComp
    {
        public HediffCompProperties_Heal Props => (HediffCompProperties_Heal)this.props;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            age++;
            if (age > this.Props.healOncePer) 
            {
                float heal = (float)this.Props.healPoint;
                foreach (Hediff_Injury injury in parent.pawn.health.hediffSet.GetInjuriesTendable().ToList())
                {
                    if (heal > this.Props.amountOfHealOnce)
                    {
                        injury.Heal(this.Props.amountOfHealOnce);
                        heal  -= this.Props.amountOfHealOnce;
                    }
                    else
                    {
                        injury.Heal(heal);
                        heal -= heal;
                        break;
                    }
                }
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref age,"age");
        }

        public int age = 0;
    }
}
