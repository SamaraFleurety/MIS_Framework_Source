using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    //做了一半：能显示了，但释放会有空指针问题
    /*public class HC_DrawAbility : HediffComp
    {
        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            List<Gizmo> gizmos = new List<Gizmo>();
            if ((this.parent as Hediff_Operator).document.apparel != null)
            {
                List<ThingComp> comps = ((this.parent as Hediff_Operator).document.apparel as ThingWithComps).AllComps;
                if (comps != null)
                {
                    foreach (ThingComp i in comps)
                    {
                        if (i is CompAbility) gizmos.AddRange((i as CompAbility).DrawGizmo());
                    }
                }
            }
            return gizmos;
        }
    }*/
}
