using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class HediffComp_Bandage : HediffComp
    {
        public HediffCompProperties_Bandage Props => (HediffCompProperties_Bandage)this.props;
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.age++;
            if (this.age > this.Props.tickOfBandageOnce) 
            {
                this.age = 0;
                List<Hediff_Injury> injuries = this.parent.pawn.health.hediffSet.GetInjuriesTendable().ToList();
                if (!injuries.NullOrEmpty())
                {
                    int count = 0;
                    foreach (Hediff_Injury injury in injuries) 
                    {
                        if (count > this.Props.bandageCount) 
                        {
                            break;
                        }
                        if (injury.Bleeding && injury.TendableNow()) 
                        {
                            count++;
                            injury.Tended(1f,5f);
                        }
                    }
                }
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref this.age,"AK_Bandage_Age");
        }
        public int age = 0;
    }
}
