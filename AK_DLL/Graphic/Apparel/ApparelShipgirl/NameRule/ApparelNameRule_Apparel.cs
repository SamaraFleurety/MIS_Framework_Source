using AK_DLL.SharedSingleton;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Apparels
{
    //因为没有衣服def了所以衣服def写ext里面了
    public class ApparelNameRule_Apparel : ISharedSingleton
    {
        public virtual string Label(Apparel_Shipgirl ap)
        {
            return ap.Ext.apparelInfo.label;
        }

        public virtual string Description(Apparel_Shipgirl ap)
        {
            return ap.Ext.apparelInfo.description;
        }
    }
}
