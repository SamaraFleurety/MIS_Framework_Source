using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKS_Shield
{
    public class TC_ShipgirlShield : TC_GenericShield
    {
        public override bool ShouldDisplay
        {
            get
            {
                Pawn wearer = Wearer;
                return wearer is { Spawned: true, Destroyed: false, Dead: false, Downed: false } && wearer.Drafted;
            }
        }
    }
}
