using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Apparels
{
    public class ApparelNameRule_Shield : ApparelNameRule_Apparel
    {
        public override string Label(Apparel_Shipgirl ap)
        {
            return ap.Ext.shieldInfo.label;
        }

        public override string Description(Apparel_Shipgirl ap)
        {
            return ap.Ext.shieldInfo.description;
        }
    }
}
