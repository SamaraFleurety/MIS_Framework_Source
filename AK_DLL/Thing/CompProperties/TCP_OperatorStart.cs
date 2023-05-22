using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AK_DLL
{
    public class TCP_OperatorStart : CompProperties
    {
        public OperatorDef designatedOperator = null;

        public TCP_OperatorStart()
        {
            this.compClass = typeof(TC_OperatorStart);
        }
    }
}
