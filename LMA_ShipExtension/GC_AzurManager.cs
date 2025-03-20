using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LMA_Lib
{
    public class GC_AzurManager : GameComponent
    {
        //往招募台存的白银，可以按比例兑换成招募券
        public int storedSilver = 0;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref storedSilver, "Ag", 0);
        }
    }
}
