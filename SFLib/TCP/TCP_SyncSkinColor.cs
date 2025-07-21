using RimWorld;
using Verse;

namespace SFLib
{
    public class TCP_SyncSkinColor : CompProperties
    {
        public TCP_SyncSkinColor()
        {
            compClass = typeof(TC_SyncSkinColor);
        }
    }

    public class TC_SyncSkinColor : ThingComp
    {
        private const int INTERVAL = 500;
        private int tick = 999;

        private Apparel GetApparel => parent as Apparel;
        private Pawn Wearer => GetApparel.Wearer;
        public override void CompTick()
        {
            Tick(1);
        }

        public override void CompTickLong()
        {
            Tick(999);
        }

        private void Tick(int amt)
        {
            tick += amt;
            if (tick < INTERVAL) return;
            tick = 0;
            if (GetApparel == null) parent.AllComps.Remove(this);
            if (Wearer == null || Wearer.story == null) return;
            this.parent.DrawColor = Wearer.story.SkinColor;
            if (Wearer.Map != null) this.parent.DirtyMapMesh(Wearer.Map);
        }
    }
}
