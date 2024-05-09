using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class TCP_TeleportTowerInferior : CompProperties
    {
        public float radius = 10;
        public TCP_TeleportTowerInferior()
        {
            compClass = typeof(TC_TeleportTowerInferior);
        }
    }
    //小传送塔 只能往大传送塔传送
    public class TC_TeleportTowerInferior : ThingComp
    {
        private TCP_TeleportTowerInferior Props => props as TCP_TeleportTowerInferior;
        protected virtual float Radius => Props.radius + 1;

        private static HashSet<TC_TeleportTowerSuperior> AllTowers => GC_AKManager.superiorRecruitTowers;
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            float distance = selPawn.Position.DistanceTo(parent.Position);
            if (distance > Radius)
            {
                yield return new FloatMenuOption("AK_TeleportTower_OutofRange".Translate(), null);
                yield break;
            }

            List<TC_TeleportTowerSuperior> towers = AllTowers.ToList();

            //传送，从自己传送到目标点，可以如蜜传如蜜。只能往大传送塔传
            for (int i = 0; i < towers.Count; ++i)
            {
                //不知道为啥这玩意经常不会被正确移除
                if (towers[i] == null || towers[i].parent == null || towers[i].parent.Destroyed || towers[i].parent.Map == null)
                {
                    AllTowers.Remove(towers[i]);
                    continue;
                }
                TC_TeleportTowerSuperior j = towers[i];
                yield return new FloatMenuOption("AK_TeleportToTower".Translate() + j.Alias, delegate ()
                {
                    //selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(AKDefOf.AK_Job_UseTeleportTower, this.parent, j.parent));
                    selPawn.DeSpawn(DestroyMode.QuestLogic);
                    GenSpawn.Spawn(selPawn, j.parent.InteractionCell, j.parent.Map);
                    CameraJumper.TryJump(new GlobalTargetInfo(j.parent.Position, j.parent.Map));
                });
            }
        }
        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            GenDraw.DrawRadiusRing(parent.Position, Radius, Color.white, (IntVec3 c) => !c.Fogged(parent.Map));
        }
    }
}
