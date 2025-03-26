using AK_DLL;
using UnityEngine;
using Verse;

namespace LMA_Lib.FCS
{
    //舰装的verb，会被身上的装备强化
    public class Verb_ShipShoot : Verb_Shoot
    {
        public override float EffectiveRange
        {
            get
            {
                if (CasterPawn?.GetDoc() is not ShipDocument doc) return base.EffectiveRange;

                return base.EffectiveRange + doc.ShipFCS.rangeOffset;
            }
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            base.DrawHighlight(target);
            GenDraw.DrawRadiusRing(caster.Position, EffectiveRange, Color.green);
        }
    }
}
