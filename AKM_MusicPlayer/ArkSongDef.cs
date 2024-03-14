using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKM_MusicPlayer
{
    //用这个啥额外字段都没有的def是因为不想检索原版song
    public class ArkSongDef : SongDef
    {
        public bool Enabled
        {
            get
            {
                if (GC_MusicMuseum.enabledSongs.Contains(this)) return true;
                return false;
            }
        }

        public void PlayMe()
        {
            Find.MusicManagerPlay.ForcePlaySong(this, false);
        }
    }
}
