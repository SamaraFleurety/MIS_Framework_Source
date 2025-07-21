using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AKM_MusicPlayer
{
    public class TCP_MusicPlayer : CompProperties
    {
        public TCP_MusicPlayer()
        {
            compClass = typeof(TC_MusicPlayer);
        }
    }

    public class TC_MusicPlayer : ThingComp
    {
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            foreach (ArkSongDef i in GC_MusicMuseum.enabledSongs)
            {
                ArkSongDef j = i;
                yield return new FloatMenuOption($"{"playsong".Translate()}: {j.label.Translate()}", j.PlayMe);
            }
        }
    }
}
