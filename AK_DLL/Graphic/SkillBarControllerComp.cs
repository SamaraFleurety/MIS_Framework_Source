using AKA_Ability;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public class SkillBarControllerComp : ThingComp
    {
        private Pawn pawn;
        public AKAbility AKATracker;
        public VAbility_Operator operatorID;
        public bool noSkill = true;
        private float SkillPercent;
        Vector3 BottomMargin = Vector3.back * 1.075f;
        private static readonly Vector2 BarSize = new Vector2(1.5f, 0.075f);
        private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(160, 170, 60, 200));
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.75f));
        private float CooldownPercent(AKAbility a)
        {
            if (a.cooldown.charge == a.cooldown.maxCharge) return 1;
            return 1f - (float)a.cooldown.CD / (float)a.cooldown.maxCD;
        }
        public override void PostDraw()
        {
            base.PostDraw();
            if (AK_ModSettings.displayBarModel)
            {
                pawn = parent as Pawn;
                if (!pawn.Drafted)
                {
                    return;
                }
                operatorID = pawn.abilities?.abilities.Find((Ability a) => a.def == AKDefOf.AK_VAbility_Operator) as VAbility_Operator;
                if (operatorID != null) 
                {
                    AKATracker = operatorID?.AKATracker?.innateAbilities.Find((AKAbility a) => !a.def.grouped);
                    if (AKATracker == null)
                    {
                        AKATracker = operatorID?.AKATracker?.groupedAbilities.Find((AKAbility a) => a.def.grouped);
                    }
                }
                if (AKATracker != null)
                {
                    GenDraw.FillableBarRequest fbr = default;
                    fbr.center = pawn.DrawPos + (Vector3.up * 5f) + BottomMargin;
                    fbr.size = BarSize;
                    //填充比例
                    SkillPercent = CooldownPercent(AKATracker);
                    fbr.fillPercent = (SkillPercent < 0f) ? 0f : SkillPercent;
                    //填充部分的颜色
                    fbr.filledMat = BarFilledMat;
                    fbr.unfilledMat = BarUnfilledMat;
                    //间距
                    fbr.margin = 0.001f;
                    fbr.rotation = Rot4.North;
                    GenDraw.DrawFillableBar(fbr);
                }
                else 
                {
                    return;
                }
            }
        }
    }
}
