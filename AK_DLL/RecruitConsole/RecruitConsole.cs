using System;
using System.Collections.Generic;
using Verse.AI;
using RimWorld;
using Verse;
using RimWorld.Planet;
using HarmonyLib;
using Verse.Sound;

namespace AK_DLL
{
    public class RecruitConsole : Building
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            this.compPower = this.GetComp<CompPowerTrader>();
            this.compRefuelable = this.GetComp<CompRefuelable>();
            if (compPower != null && !compPower.PowerOn)
            {
                yield return new FloatMenuOption("CannotUseNoPower".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0); yield break;
            }
            if (selPawn.health.Dead || selPawn == null && selPawn.CanReach(this, PathEndMode.Touch, Danger.Deadly))
            {
                yield return new FloatMenuOption("AK_PawnNull".Translate(), null); yield break;
            }
            else
            {
                if (compRefuelable.Fuel >= 0.1)
                {
                    yield return new FloatMenuOption("AK_CanReach".Translate(),
                    delegate
                    {
                        selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.AK_UseRecruitConsole, this));
                    }
                    ); yield break;
                }
                else
                {
                    yield return new FloatMenuOption("AK_NoTicket".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0); yield break;
                }
            }

        }

        //招募干员放进class operatorDef了

        /*public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Collections.Look(ref recruitedOperators, "spawnedOperators", LookMode.Def);
        } */
        public CompPowerTrader compPower;
        public CompRefuelable compRefuelable;
        //public List<OperatorDef> recruitedOperators = new List<OperatorDef>();
    }
}