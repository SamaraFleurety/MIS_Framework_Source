using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    class HC_SelfExplosion : HediffComp
    {
        public HCP_SelfExplosion Props
        {
            get { return (HCP_SelfExplosion)base.props; }
        }

        private HCP_SelfExplosion exactProps = new HCP_SelfExplosion();

        public int AfterTick
        {
            get
            {
                return exactProps.afterTicks;
            }
            set
            {
                exactProps.afterTicks = value;
            }
        }

        public float Damage
        {
            get
            {
                return exactProps.damage;
            }
            set
            {
                exactProps.damage = value;
            }
        }

        public float Radius { 
            get
            {
                return exactProps.radius;
            }
            set
            {
                exactProps.radius = value;
            }
        }

        private int remainingTick = 100;

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.exactProps = Props;
        }
        public override string CompLabelInBracketsExtra
        {
            get
            {
                return ($"{base.Pawn}将会在{this.remainingTick}后产生一个爆炸!");
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.remainingTick, "rTick");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.exactProps = this.Props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            this.remainingTick--;
            if (this.remainingTick <= 0)
            {
                //fixme:doexplosion
                HealthUtility.AdjustSeverity(Pawn, base.Def, -100f);
            }
        }
    }
}
