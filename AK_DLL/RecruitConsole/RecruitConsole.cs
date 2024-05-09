using System;
using System.Collections.Generic;
using Verse.AI;
using RimWorld;
using Verse;
using RimWorld.Planet;
using HarmonyLib;
using Verse.Sound;
using UnityEngine;

namespace AK_DLL
{
    public class RecruitConsole : Building
    {
        public CompPowerTrader CompPower => GetComp<CompPowerTrader>();
        public CompRefuelable CompRefuelable => GetComp<CompRefuelable>();

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            //this.compPower = this.GetComp<CompPowerTrader>();
            //this.compRefuelable = this.GetComp<CompRefuelable>();
            //不可以使用
            if (CompPower != null && !CompPower.PowerOn)
            {
                yield return new FloatMenuOption("CannotUseNoPower".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0); yield break;
            }
            if (selPawn.health.Dead || selPawn == null && selPawn.CanReach(this, PathEndMode.Touch, Danger.Deadly))
            {
                yield return new FloatMenuOption("AK_PawnNull".Translate(), null); yield break;
            }

            //换装 右键直接出选项，没有ui
            OperatorDocument doc = selPawn.GetDoc();
            if (doc != null && !doc.operatorDef.clothSets.NullOrEmpty())
            {
                yield return new FloatMenuOption("AK_ChangeFashionDefault".Translate(),
                delegate
                {
                    //doc.pendingFashion = -1;
                    doc.pendingFashionDef = null;
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AKDefOf.AK_Job_OperatorChangeFashion, this));
                }
                );
                /*foreach (KeyValuePair<int, OperatorClothSetDef> set in doc.operatorDef.clothSet)
                {
                    int j = set.Key;
                    yield return new FloatMenuOption("AK_ChangeFashionTo".Translate() + set.Value.label.Translate(),
                        delegate ()
                        {
                            doc.pendingFashion = j;
                            selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AKDefOf.AK_Job_OperatorChangeFashion, this));
                        });
                }*/
                foreach (OperatorFashionSetDef set in doc.operatorDef.clothSets)
                {
                    yield return new FloatMenuOption("AK_ChangeFashionTo".Translate() + set.label.Translate(),
                        delegate
                        {
                            doc.pendingFashionDef = set;
                            selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AKDefOf.AK_Job_OperatorChangeFashion, this));
                        });

                }
            }

            foreach (FloatMenuOption i in base.GetFloatMenuOptions(selPawn))
            {
                if (i != null) yield return i;
            }

            //可以招募
            if (CompRefuelable.Fuel >= 0.1)
            {
                yield return new FloatMenuOption("AK_CanReach".Translate(),
                delegate
                {
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AKDefOf.AK_Job_UseRecruitConsole, this));
                }
                ); yield break;
            }
            else
            {
                yield return new FloatMenuOption("AK_NoTicket".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0); yield break;
            }
        }


        public override void ExposeData()
        {
            base.ExposeData();
        }

        public override string Label
        {
            get
            {
                TC_TeleportTowerSuperior comp = GetComp<TC_TeleportTowerSuperior>();
                if (comp != null) return comp.Alias;
                return base.Label;
            }
        }
    }
}