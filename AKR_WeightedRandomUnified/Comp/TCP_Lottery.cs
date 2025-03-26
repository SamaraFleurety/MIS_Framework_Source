using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AKR_Random
{
    //附在thing上面的抽奖comp，需要手动抽
    public class TCP_Lottery : CompProperties
    {
        public bool showFloatMenu = false;
        public bool showGizmo = false;

        public string label = "gacha";
        public string description = "desc";
        public string iconPath = BaseContent.BadTexPath;

        public RandomizerNode_Base root;
        public TCP_Lottery()
        {
            compClass = typeof(TC_Lottery);
        }
    }

    public class TC_Lottery : ThingComp
    {
        TCP_Lottery Prop => props as TCP_Lottery;

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (!Prop.showFloatMenu) yield break;
            yield return new FloatMenuOption(Prop.label, delegate
            {
                Gacha(parent.InteractionCell, parent.Map).Count();
            });
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (!Prop.showGizmo && !Prefs.DevMode) yield break;
            yield return new Command_Action()
            {
                defaultLabel = Prop.label,
                defaultDesc = Prop.description,
                icon = ContentFinder<Texture2D>.Get(Prop.iconPath),
                action = () =>
                {
                    Gacha(parent.InteractionCell, parent.Map).Count();
                }
            };
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            return this.CompGetGizmosExtra();
        }

        public virtual IEnumerable<object> Gacha(IntVec3 cell, Map map, Pawn gambler = null, float point = 0)
        {
            return Prop.root.TryIssueGachaResult(cell, map, gambler, point);
        }
    }
}
