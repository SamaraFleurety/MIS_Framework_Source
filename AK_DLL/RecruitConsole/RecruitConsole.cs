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

            //换装,没写完
            OperatorDocument doc = selPawn.GetDoc();
            if (doc != null && doc.operatorDef.clothSet != null && doc.operatorDef.clothSet.Count > 0)
            {
                yield return new FloatMenuOption("AK_ChangeFashionDefault".Translate(),
                delegate
                {
                    doc.pendingFashion = -1;
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.AK_OperatorChangeFashion, this));
                }
                );
                foreach (KeyValuePair<int, OperatorClothSet> set in doc.operatorDef.clothSet)
                {
                    int j = set.Key;
                    yield return new FloatMenuOption("AK_ChangeFashionTo".Translate() + set.Value.label.Translate(),
                        delegate ()
                        {
                            doc.pendingFashion = j;
                            selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.AK_OperatorChangeFashion, this));
                        });
                }
            }

            //可以招募
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

        public CompPowerTrader compPower;
        public CompRefuelable compRefuelable;
    }
}