using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKS_Shield
{
    //父类不是Apparel，直接放在pawn上面的护盾
    public class TC_GenericShieldDirect : TC_GenericShield
    {
        public override Pawn Wearer => parent as Pawn;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in GetGizmos()) yield return gizmo;
        }
    }
}
