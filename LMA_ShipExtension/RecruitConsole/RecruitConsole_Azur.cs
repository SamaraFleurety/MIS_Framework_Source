using AK_DLL;
using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace LMA_Lib
{
    //最后还是觉得写个新类 兼容性更好
    public class RecruitConsole_Azur : RecruitConsole
    {
        public const int SingleRecruitCost = 1;

        public override IEnumerable<FloatMenuOption> GetExtendedFloatMenuOptions(Pawn selPawn)
        {
            if (AzurDefOf.LMA_Rander_Operators?.root == null)
            {
                //Log.Warning("[LMA] 舰娘卡池LMA_Rander_Operators未配置，无法抽卡");
                yield break;
            }

            //GC_AzurManager manager = GC_AzurManager.Instance;
            //if (manager.storedSilver >= SingleRecruitCost * GC_AzurManager.SilverExchangeRate)
            yield return new FloatMenuOption("LMA_Invest_6480".Translate(), delegate
            {
                selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AzurDefOf.LMA_Job_UseGachaConsole, this));
            });
            yield return new FloatMenuOption("LMA_Invest_All".Translate(), delegate
            {
                selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AzurDefOf.LMA_Job_UseGachaConsole, this, this));
            });

            List<LocalTargetInfo> silverTargets = new();
            int silverCount = 0;
            List<Thing> silverThings = Map.listerThings.ThingsOfDef(ThingDefOf.Silver);
            for (int index = 0; index < silverThings.Count && silverCount < GC_AzurManager.SilverExchangeRate; index++)
            {
                Thing silver = silverThings[index];
                if (!silver.Spawned || silver.IsForbidden(selPawn) || !selPawn.CanReserveAndReach(silver, PathEndMode.ClosestTouch, Danger.Deadly))
                {
                    continue;
                }

                silverTargets.Add(silver);
                silverCount += silver.stackCount;
            }

            if (silverCount >= GC_AzurManager.SilverExchangeRate)
            {
                yield return new FloatMenuOption("LMA_StoreSilver".Translate(GC_AzurManager.SilverExchangeRate), delegate
                {
                    Job job = JobMaker.MakeJob(AzurDefOf.LMA_Job_StoreSilver, this);
                    job.targetQueueB = silverTargets;
                    selPawn.jobs.TryTakeOrderedJob(job);
                });
            }
        }

        public void DrawOperators(Pawn selPawn, int count)
        {
            GC_AzurManager manager = GC_AzurManager.Instance;

            int silverCost = count * GC_AzurManager.SilverExchangeRate;
            if (manager.storedSilver < silverCost)
            {
                Log.Warning($"[LMA] 抽取 {count} 次需要 {silverCost} 白银，当前仅有 {manager.storedSilver}");
                return;
            }

            manager.storedSilver -= silverCost;
            manager.GetUpOperators(AzurDefOf.LMA_Rander_Operators);
            for (int index = 0; index < count; index++)
            {
                foreach (object result in AzurDefOf.LMA_Rander_Operators.root.TryIssueGachaResult(InteractionCell, Map, selPawn))
                {
                    if (result is Thing t)
                    {
                        Messages.Message("LMA_GachaReport".Translate(t.LabelShort), MessageTypeDefOf.NeutralEvent);
                    }
                }
            }
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuRecruitOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption("AK_CanReach".Translate(), delegate
            {
                selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AzurDefOf.LMA_Job_UseRecruitConsole, this));
            });
            yield return new FloatMenuOption("AK_RecruitContinuous".Translate(), delegate
            {
                selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AzurDefOf.LMA_Job_UseRecruitConsole, this, this));  //用target B做标记，要是不为null就是连续招募模式
            });
        }
    }
}
