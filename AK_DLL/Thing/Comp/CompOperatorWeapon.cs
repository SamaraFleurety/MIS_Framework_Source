using System;
using Verse;
using System.Linq;
using System.Text;
using RimWorld;
using System.IO;

namespace AK_DLL
{
    public class CompOperatorWeapon : ThingComp
    {
        /*public override void PostPostMake()
        {
            base.PostPostMake();
            if (!Operator_Recruited.RecruitedOperators.Contains(operatorDef))
            {
                Operator_Recruited.RecruitedOperators.Add(operatorDef);
            }
        }
        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            if (Operator_Recruited.RecruitedOperators.Contains(operatorDef))
            {
                Operator_Recruited.RecruitedOperators.Remove(operatorDef);
            }
        }
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref operatorDef, "operatorDef");
            if (!Operator_Recruited.RecruitedOperators.Contains(operatorDef))
            {
                Operator_Recruited.RecruitedOperators.Add(operatorDef);
            }
        }
        public OperatorDef operatorDef;*/
    }
}