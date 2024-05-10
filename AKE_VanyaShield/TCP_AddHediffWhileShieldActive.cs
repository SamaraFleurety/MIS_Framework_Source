using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AKA_Ability;

namespace AKE_VanyaShield
{
    public class TCP_AddHediffWhileShieldActive : CompProperties
    {
        public HediffDef hediff;
        public TCP_AddHediffWhileShieldActive()
        {
            compClass = typeof(TC_AddHediffWhileShieldActive);
        }
    }

    public class TC_AddHediffWhileShieldActive : ThingComp
    {
        TCP_AddHediffWhileShieldActive Props => props as TCP_AddHediffWhileShieldActive;
        int Tick => Find.TickManager.TicksGame;
        int INTERVAL = (int)((int)TimeToTick.hour * 0.5);

        Pawn Wearer => ((Apparel)parent).Wearer;
        public override void CompTick()
        {
            if (Tick % INTERVAL == 0)
            {
                TC_VanyaShieldExtension compExt = parent.TryGetComp<TC_VanyaShieldExtension>();
                if (compExt == null || Wearer == null) return;
                if (Wearer.drafter.Drafted && compExt.shouldDrawNow)
                {
                    AbilityEffect_AddHediff.AddHediff(Wearer, Props.hediff, severity: 10);
                }
                else
                {
                    AbilityEffect_AddHediff.AddHediff(Wearer, Props.hediff, severity: -10);
                }
            }
        }

    }
}
