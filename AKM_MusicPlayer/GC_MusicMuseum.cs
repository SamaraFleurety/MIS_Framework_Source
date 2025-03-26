using System.Collections.Generic;
using Verse;

namespace AKM_MusicPlayer
{
    public class GC_MusicMuseum : GameComponent
    {
        public static HashSet<ArkSongDef> enabledSongs = new HashSet<ArkSongDef>();
        public static HashSet<ArkSongDef> potentialSongs = new HashSet<ArkSongDef>();

        public GC_MusicMuseum(Game game)
        {
        }

        public override void StartedNewGame()
        {
            base.StartedNewGame();
            enabledSongs.Clear();
            InitializePotentialSongs();
        }

        //成功激活返回true，已经有了返回false
        public static bool EnableSong(ArkSongDef def)
        {
            if (enabledSongs.Contains(def)) return false;
            enabledSongs.Add(def);
            potentialSongs.Remove(def);
            return true;
        }

        private void InitializePotentialSongs()
        {
            potentialSongs.Clear();
            foreach (ArkSongDef i in DefDatabase<ArkSongDef>.AllDefs)
            {
                potentialSongs.Add(i);
            }
        }

        public override void LoadedGame()
        {
            base.LoadedGame();
            InitializePotentialSongs();
            foreach(ArkSongDef i in enabledSongs)
            {
                potentialSongs.Remove(i);
            }
        }

        public override void ExposeData()
        {
            Scribe_Collections.Look(ref enabledSongs, "songs", LookMode.Def);
        }
    }

}
