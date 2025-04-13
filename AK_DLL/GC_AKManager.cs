using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    public class GC_AKManager : GameComponent
    {
        public static HashSet<TC_TeleportTowerSuperior> superiorRecruitTowers = new HashSet<TC_TeleportTowerSuperior>();

        public static GC_AKManager instance = null;
        public GC_AKManager(Game game)
        {
            instance = this;
            superiorRecruitTowers = new HashSet<TC_TeleportTowerSuperior>();
            PatchWindowOnGUI.lastSpineInstance = null;
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
