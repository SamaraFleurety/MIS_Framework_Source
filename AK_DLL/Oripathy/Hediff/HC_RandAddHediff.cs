using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class HCP_RandAddHediff : HediffCompProperties
    {
        public int interval = 1;
        public TimeToTick intervalUnit = TimeToTick.hour;
        public List<HediffStat> hediffStatsSet = new List<HediffStat>();

        public HCP_RandAddHediff()
        {
            this.compClass = typeof(HC_RandAddHediff);
        }
    }
    public class HC_RandAddHediff : HediffComp
    {
        public HCP_RandAddHediff Props => (HCP_RandAddHediff)this.props;

        private int tick = 0;

        private int[] weight;

        private bool arrayCached = false;

        private int arraySum;
        private List<HediffStat> HediffStats
        {
            get { return this.Props.hediffStatsSet; }
        }

        private int Interval
        {
            get { return this.Props.interval * (int)this.Props.intervalUnit; }
        }
        private void CaculateWeightArray()
        {
            this.weight = new int[this.HediffStats.Count];
            this.weight[0] = this.HediffStats[0].randWeight;
            for (int i = 1; i < this.HediffStats.Count; ++i)
            {
                this.weight[i] = this.weight[i - 1] + this.HediffStats[i].randWeight;
            }
            arraySum = this.weight.Last();
            this.arrayCached = true;
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            if (!arrayCached)
            {
                this.CaculateWeightArray();
            }
            ++tick;
            if (tick >= Interval)
            {
                tick = 0;
                HediffStat hediff = HediffStats[AK_Tool.weightArrayRand(weight)];
                Log.Message("Add " + hediff.hediff.defName + " to " + this.Pawn.Name.ToString());
                AbilityEffect_AddHediff.AddHediff(this.Pawn, hediff.hediff, hediff.part, UnityEngine.Random.Range(hediff.randWorseMin, hediff.randWorseMax));
            }
        }
    }
}
