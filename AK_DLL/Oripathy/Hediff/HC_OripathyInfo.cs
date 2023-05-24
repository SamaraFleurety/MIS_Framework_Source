using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;


namespace AK_DLL
{

    public class HCP_OripathyInfo : HediffCompProperties
    {
        public HCP_OripathyInfo()
        {
            this.compClass = typeof(HC_OripathyInfo);
        }
    }

    public class HC_OripathyInfo : HediffComp
    {
        public override string CompLabelInBracketsExtra
        {
            get
            {
                //fixme:翻译
                string info = "no circlet";

                if (Pawn.apparel == null)
                {
                    return info;
                }

                if (Pawn.apparel.WornApparel.Any(apparel => apparel.GetComp<TC_OripathyMonitor>() != null))
                {
                    info = "some oripathy info";
                }
                return info;
            }
        }

    }
}
