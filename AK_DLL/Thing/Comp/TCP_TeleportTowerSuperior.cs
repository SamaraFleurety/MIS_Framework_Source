using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    public class TCP_TeleportTowerSuperior : TCP_TeleportTowerInferior
    {
        public TCP_TeleportTowerSuperior()
        {
            compClass = typeof(TC_TeleportTowerSuperior);
        }
    }

    public class TC_TeleportTowerSuperior : TC_TeleportTowerInferior
    {
        public string alias = null;

        public string Alias
        {
            get
            {
                if (alias != null) return alias;
                return parent.def.label.Translate();
            }
        }

        //private Command_Action cachedChangeAliasGizmo = null;

        private static HashSet<TC_TeleportTowerSuperior> AllTowers => GC_AKManager.superiorRecruitTowers;

        private Command_Action CachedChangeAliasGizmo
        {
            get
            {
                return new Command_Action
                {
                    icon = TypeDef.iconTeleTowerChangeName,
                    defaultDesc = "AK_ChangeRecruitTowerAliasDesc".Translate(),
                    defaultLabel = "AK_ChangeRecruitTowerAliasLabel".Translate(),
                    action = delegate ()
                    {
                        Find.WindowStack.Add(new Dialog_Input(delegate (string alias)
                        {
                            this.alias = alias;
                        }, delegate (string alias)
                        {
                            return true;
                        }, this.alias));
                    }
                };
                //return cachedChangeAliasGizmo;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return CachedChangeAliasGizmo;
        }

        //往管理器注册 执行传送
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (!AllTowers.Contains(this))
            {
                GC_AKManager.superiorRecruitTowers.Add(this);
            }
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (AllTowers.Contains(this)) AllTowers.Remove(this);
            base.PostDestroy(mode, previousMap);
        }

        public override void PostDeSpawn(Map map, DestroyMode mode = DestroyMode.Vanish)
        {
            if (AllTowers.Contains(this)) AllTowers.Remove(this);
            base.PostDeSpawn(map);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref alias, "alias");
        }
    }
}
