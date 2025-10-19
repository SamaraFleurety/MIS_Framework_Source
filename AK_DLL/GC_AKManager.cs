using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    public class GC_AkManager : GameComponent
    {
        public static HashSet<TC_TeleportTowerSuperior> SuperiorRecruitTowers = new();

        public static GC_AkManager Instance;

        public GC_AkManager(Game game)
        {
            Instance = this;
            SuperiorRecruitTowers = new HashSet<TC_TeleportTowerSuperior>();
            Patch_WindowOnGUI.LastSpineInstance = null;
        }

        public override void ExposeData()
        {
            //Scribe_Collections.Look(ref superiorRecruitTowers, "portals", LookMode.Reference);
        }

        public override void FinalizeInit()
        {

        }
    }
}
