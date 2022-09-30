using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class Summoned : Pawn
    {

        public override void Tick()
        {
            base.Tick();
            if (summoner_Pawm.Dead||!summoner_Pawm.Spawned||summoner_Pawm == null) 
            {
                this.Destroy();
            }
        }

        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {
            base.Kill(dinfo, exactCulprit);
            summoner.TryGetComp<CompAbility>().SummonedDead();
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            summoner.TryGetComp<CompAbility>().SummonedDead();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref summoner, "summoner");
            Scribe_References.Look(ref summoner_Pawm, "summoner_Pawm");
        }

        public Thing summoner;
        public Pawn summoner_Pawm;
    }
}
