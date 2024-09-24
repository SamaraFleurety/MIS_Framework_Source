using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKA_Ability.Gizmos
{
    //需要用targetor手动选择目标的技能
    public class Gizmo_AbilityCast_Targetor : Gizmo_AbilityCast_Base
    {
        private AKAbility_Targetor Parent => parent as AKAbility_Targetor;
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev); 
            Find.DesignatorManager.Deselect();
            //Find.Targeter.BeginTargeting(parent.def.targetParams, parent.TryCastShot, parent.CasterPawn, actionWhenFinished: null);
            Find.Targeter.BeginTargeting(Parent);
        }
    }
}
