using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class OperatorWeapon : ThingWithComps
    {
        /*public override void Tick()
        {
            base.Tick();
            if (operator_Pawn == null || operator_Pawn.Dead || !operator_Pawn.Spawned)
            {
                if (this.GetComps<CompOperatorWeapon>() is CompOperatorWeapon compOperatorWeapon)
                {
                    if (Operator_Recruited.RecruitedOperators.Contains(compOperatorWeapon.operatorDef))
                    {
                        Operator_Recruited.RecruitedOperators.Remove(compOperatorWeapon.operatorDef);
                    }
                }
                this.Destroy();
            }
            else 
            {
                operator_Pawn.equipment.AddEquipment(this);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref this.operator_Pawn, "operator_Pawn");
            //Log.Warning($"!!!{operator_Pawn.Name}!!!");
        }
        public Pawn operator_Pawn;*/
    }
}
