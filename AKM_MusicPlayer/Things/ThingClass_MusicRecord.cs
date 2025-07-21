using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace AKM_MusicPlayer
{
    public class ThingClass_MusicRecord : ThingWithComps
    {
        public ArkSongDef recordedSong;

        public const int XPGain = 500;

        public ThingClass_MusicRecord()
        {
            this.recordedSong = DefDatabase<ArkSongDef>.GetRandom();
        }

        public ThingClass_MusicRecord(ArkSongDef recordedSong)
        {
            this.recordedSong = recordedSong;
        }

        private string StatusString => recordedSong.Enabled ? "akm_actived".Translate() : "akm_inactived".Translate();

        public override string Label
        {
            get
            {
                return $"{base.def.label.Translate()}: {recordedSong.label.Translate()} ({StatusString})";
            }
        }

        public override string LabelCap => Label;
        public override string LabelNoCount => Label;
        public override string LabelCapNoCount => Label;
        public override string LabelShort => Label;
        public override string LabelShortCap => Label;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref recordedSong, "song");
        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption i in base.GetFloatMenuOptions(selPawn)) yield return i;

            //让小人记录这张唱片
            Thing t = FindMusicPlayer(selPawn);
            if (t != null)
            {
                yield return new FloatMenuOption("insertThisMusicRecord".Translate(), delegate ()
                {
                    selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AKMDefOf.AKM_Job_InsertMusicRecord, this, t));
                });
            }
        }

        public Thing FindMusicPlayer(Pawn p)
        {
            List<Building> candidate = Map.listerBuildings.allBuildingsColonist;

            foreach (Building i in candidate)
            {
                if (i.GetComp<TC_MusicPlayer>() != null && ReservationUtility.CanReserveAndReach(p, i, PathEndMode.InteractionCell, Danger.Deadly)) return i;
            }

            return null;
        }
    }
}
