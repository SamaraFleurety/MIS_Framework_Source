using AK_DLL;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace LMA_Lib
{
    //最后还是觉得写个新类 兼容性更好
    public class RecruitConsole_Azur : RecruitConsole
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuRecruitOptions(Pawn selPawn)
        {
            yield return new FloatMenuOption("AK_CanReach".Translate(),
                 delegate
                 {
                     selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AzurDefOf.LMA_Job_UseRecruitConsole, this));
                 }
                 );
            yield return new FloatMenuOption("AK_RecruitContinuous".Translate(), delegate
            {
                selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AzurDefOf.LMA_Job_UseRecruitConsole, this, this));  //用target B做标记，要是不为null就是连续招募模式
            });
        }

    }
}
