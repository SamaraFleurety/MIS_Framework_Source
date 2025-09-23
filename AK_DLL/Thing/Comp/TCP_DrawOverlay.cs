using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class TCP_DrawOverlay : CompProperties
    {
        public GraphicData overlay;
        public Vector3 offset = new(0, 0, 0);
        public TCP_DrawOverlay()
        {
            compClass = typeof(TC_DrawOverlay);
        }
    }

    public class TC_DrawOverlay : ThingComp
    {
        TCP_DrawOverlay Props => props as TCP_DrawOverlay;

        //Graphic graphic = null;
        Graphic OverlayGraphic
        {
            get
            {
                return Props.overlay.GraphicColoredFor(parent);
            }
        }

        public override void PostDraw()
        {
            Vector3 loc = parent.DrawPos + Altitudes.AltIncVect + Props.offset;
            loc.y += 5f;
            OverlayGraphic.Draw(loc, parent.Rotation, parent);
        }
    }
}
