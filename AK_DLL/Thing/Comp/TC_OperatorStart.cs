using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class TC_OperatorStart : ThingComp
    {
        public TCP_OperatorStart Props => (TCP_OperatorStart)this.props;

        List<Pawn> Colonists = new List<Pawn>();
        public override void CompTick()
        {
        }

        public override void CompTickLong()
        {
            CompTick();
        }

        public override void CompTickRare()
        {
            CompTick();
        }
    }
}
