using AK_DLL;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace LMA_Lib
{
    //最后还是觉得写个新类 兼容性更好
    public class RecruitConsole_Azur : RecruitConsole
    {
        private const int SingleRecruitCount = 1;
        private const int TenRecruitCount = 10;

        public override IEnumerable<FloatMenuOption> GetExtendedFloatMenuOptions(Pawn selPawn)
        {
            if (AzurDefOf.LMA_Rander_Operators?.root == null)
            {
                Log.Warning("[LMA] 舰娘卡池LMA_Rander_Operators未配置，无法抽卡");
                yield break;
            }

            if (CompRefuelable.Fuel >= SingleRecruitCount - 0.01f)
            {
                yield return new FloatMenuOption("LMA_Invest_6480".Translate(), delegate
                {
                    DrawOperators(selPawn, SingleRecruitCount);
                });
            }
            if (CompRefuelable.Fuel >= TenRecruitCount - 0.01f)
            {
                yield return new FloatMenuOption("LMA_Invest_All".Translate(), delegate
                {
                    DrawOperators(selPawn, TenRecruitCount);
                });
            }
        }

        private void DrawOperators(Pawn selPawn, int count)
        {
            CompRefuelable.ConsumeFuel(count);
            for (int index = 0; index < count; index++)
            {
                foreach (object ignored in AzurDefOf.LMA_Rander_Operators.root.TryIssueGachaResult(InteractionCell, Map, selPawn, 0f))
                {
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