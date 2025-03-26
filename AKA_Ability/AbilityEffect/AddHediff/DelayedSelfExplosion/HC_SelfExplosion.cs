using RimWorld;
using System;
using Verse;

namespace AKA_Ability
{
    [Obsolete]
    class HC_SelfExplosion : HediffComp
    {
        #region 属性 

        private HCP_SelfExplosion exactProps = new HCP_SelfExplosion();
        public HCP_SelfExplosion Props
        {
            get { return (HCP_SelfExplosion)base.props; }
        }
        public int AfterTick
        {
            get
            {
                return this.exactProps.afterTicks;
            }
            set
            {
                this.exactProps.afterTicks = value;
            }
        }

        public int Damage
        {
            get
            {
                return this.exactProps.damage;
            }
            set
            {
                this.exactProps.damage = value;
            }
        }

        public float Radius
        {
            get
            {
                return this.exactProps.radius;
            }
            set
            {
                this.exactProps.radius = value;
            }
        }
        #endregion


        public override void CompPostMake()
        {
            base.CompPostMake();
            this.exactProps = Props;
        }
        public override string CompLabelInBracketsExtra
        {
            get
            {
                return ($"{base.Pawn}将会在{this.AfterTick}后产生一个爆炸!");
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.exactProps = this.Props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            this.AfterTick--;
            if (this.AfterTick <= 0)
            {
                if (this.Pawn.Map != null)
                    GenExplosion.DoExplosion(this.Pawn.Position, this.Pawn.Map, this.Radius, DamageDefOf.Flame, null, this.Damage, -1f, null, null, null, null, null, 0f, 1, null, true, null, 0f, 1, 0f, false, null, null, null, true, 1f, 0f, true, null, 1f);
                HealthUtility.AdjustSeverity(Pawn, base.Def, -10f);
            }
        }
    }
}
