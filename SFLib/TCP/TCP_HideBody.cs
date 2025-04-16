using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace SFLib
{
    public class TCP_HideBody : CompProperties
    {
        public bool hideBody = false;
        public bool hideHead = false;
        public TCP_HideBody()
        {
            compClass = typeof(TC_HideBody);
        }
    }

    public class TC_HideBody : ThingComp
    {
        TCP_HideBody Props => props as TCP_HideBody;
        public override void Notify_Equipped(Pawn pawn)
        {
            if (Props.hideBody && !Patch_TCPHideBody.registeredPawns.Contains(pawn)) Patch_TCPHideBody.registeredPawns.Add(pawn);
            if (Props.hideHead && !Patch_TCPHideHead.registeredPawns.Contains(pawn)) Patch_TCPHideHead.registeredPawns.Add(pawn);
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            if (Patch_TCPHideBody.registeredPawns.Contains(pawn)) Patch_TCPHideBody.registeredPawns.Remove(pawn);
            if (Patch_TCPHideHead.registeredPawns.Contains(pawn)) Patch_TCPHideHead.registeredPawns.Remove(pawn);
        }

        public override void PostExposeData()
        {
            if (parent is Apparel apparel)
            {
                Pawn pawn = apparel.Wearer;
                if (pawn != null) Notify_Equipped(pawn);
            }
        }
    }
}
