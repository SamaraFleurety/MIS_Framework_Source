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
                float heal = Props.healPoint;
                for (int i = 0; i < parent.pawn.health.hediffSet.hediffs.Count; ++i)
                {
                    Hediff injury = parent.pawn.health.hediffSet.hediffs[i];
                    if (injury is Hediff_Injury)
                    {
                        if (heal > this.Props.amountOfHealOnce)
                        {
                            injury.Heal(this.Props.amountOfHealOnce);
                            heal -= this.Props.amountOfHealOnce;
                        }
                        else
                        {
                            injury.Heal(heal);
                            //heal -= heal;
                            break;
                        }
                    }
                }
                /*foreach (Hediff i in parent.pawn.health.hediffSet.GetHediffsTendable().ToList())
                {
                    if (!(i is Hediff_Injury injury)) continue;
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
                }*/
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref age, "age");
        }

        public int age;
    }
}
