﻿using Verse;

namespace LMA_Lib
{
    public class GC_AzurManager : GameComponent
    {
        //往招募台存的白银，可以按比例兑换成招募券
        public int storedSilver = 0;

        public GC_AzurManager(Game game)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref storedSilver, "Ag", 0);
        }
    }
}
