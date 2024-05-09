using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class GC_AKManager : GameComponent
    {
        public static HashSet<TC_TeleportTowerSuperior> superiorRecruitTowers = new HashSet<TC_TeleportTowerSuperior>();

        public GC_AKManager(Game game)
        {
        }

        public override void ExposeData()
        {
            //Scribe_Collections.Look(ref superiorRecruitTowers, "portals", LookMode.Reference);
        }
    }
}
