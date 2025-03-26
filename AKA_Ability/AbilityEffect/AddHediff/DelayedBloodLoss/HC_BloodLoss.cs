using Verse;

namespace AKA_Ability
{
    public class HC_BloodLoss : HediffComp
    {
        #region 属性
        private HCP_BloodLoss exactProps = new HCP_BloodLoss();
        private int tick = 5000;
        public HCP_BloodLoss Props
        {
            get { return (HCP_BloodLoss)base.props; }
        }

        public int Intervel
        {
            get { return this.exactProps.interval; }
            set { this.exactProps.interval = value; }
        }

        public float Amount
        {
            get { return this.exactProps.lossAmount; }
            set { this.exactProps.lossAmount = value; }
        }
        public int Duration
        {
            get { return this.exactProps.duration; }
            set { this.exactProps.duration = value; }
        }

        public int Delay
        {
            get { return this.exactProps.delay; }
            set { this.exactProps.delay = value; }
        }
        #endregion

        #region 规范化组件
        public override void CompPostMake()
        {
            base.CompPostMake();
            this.exactProps = this.Props;
        }

        public override string CompLabelInBracketsExtra
        {
            get { return $"持续流血,剩余{this.Duration}"; }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.tick, "tick");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.exactProps = this.Props;
            }
        }
        #endregion

        public override void CompPostTick(ref float severityAdjustment)
        {
            tick++;
            if (tick >= this.Intervel)
            {
                tick = 0;
                if (this.Delay > 0) this.Delay--;
                else
                {
                    this.Duration--;
                    HealthUtility.AdjustSeverity(base.Pawn, DefDatabase<HediffDef>.GetNamed("BloodLoss"), this.Amount);
                }
                if (this.Duration <= 0) this.parent.Severity -= 10;
            }
        }
    }
}
