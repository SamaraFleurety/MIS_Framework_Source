using Verse;

namespace AKS_Shield.Extension
{
    public abstract class TC_ShieldExtension_Base : ThingComp
    {
        private TC_GenericShield compShield = null;

        public TC_GenericShield CompShield
        {
            get
            {
                compShield ??= parent.TryGetComp<TC_GenericShield>();
                return compShield;
            }
        }

        protected bool HasEnergyLeft => CompShield.Energy > 0;

        protected bool ShouldDraw => CompShield.ShouldDisplay;

        protected Pawn Wearer => CompShield.Wearer;

        protected static int TickNow => Find.TickManager.TicksGame;

        public override void CompTick()
        {
            Tick(1);
        }

        public virtual void Tick(int amt) { }
    }
}
