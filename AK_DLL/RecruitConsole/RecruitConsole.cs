using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;
using Verse.AI;

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

            //手动注册干员
            if (AK_ModSettings.allowManualRegister)
            {
                foreach (OperatorDocument docu in GC_OperatorDocumentation.opDocArchive.Values)
                {
                    OperatorDef opdef = docu.operatorDef;
                    if (selPawn.Name.ToString().Equals(new NameTriple(opdef.name, opdef.nickname, opdef.surname).ToString()))
                    {
                        yield return new FloatMenuOption("AK_ManualRegOp".Translate(opdef.nickname), delegate
                        {
                            OperatorDocument doc1 = docu;
                            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("AK_ManualRegConfirm".Translate(opdef.nickname), delegate
                            {
                                doc1.ManualRegister(selPawn);
                            }));
                        });
                    }
                }
            }

            foreach (FloatMenuOption extraOptions in GetExtendedFloatMenuOptions(selPawn)) yield return extraOptions;

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
            if (CompRefuelable.Fuel < 0.1)
            {
                yield return new FloatMenuOption("AK_NoTicket".Translate(), null, MenuOptionPriority.Default, null, null, 0f, null, null, true, 0);
                yield break;
            }

            foreach (FloatMenuOption option in GetFloatMenuRecruitOptions(selPawn))
            {
                yield return option;
            }
        }

        public virtual IEnumerable<FloatMenuOption> GetFloatMenuRecruitOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption("AK_CanReach".Translate(),
                delegate
                {
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AKDefOf.AK_Job_UseRecruitConsole, this));
                }
                );
            yield return new FloatMenuOption("AK_RecruitContinuous".Translate(), delegate
            {
                selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AKDefOf.AK_Job_UseRecruitConsole, this, this));  //用target B做标记，要是不为null就是连续招募模式
            });
        }

        public virtual IEnumerable<FloatMenuOption> GetExtendedFloatMenuOptions(Pawn selPawn)
        {
            yield break;
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