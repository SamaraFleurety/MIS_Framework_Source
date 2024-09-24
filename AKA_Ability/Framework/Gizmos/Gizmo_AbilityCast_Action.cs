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
    //按下按钮就能直接(自动)确定目标的技能
    public class Gizmo_AbilityCast_Action : Gizmo_AbilityCast_Base
    {
        public Action action;
        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
            action();
        }
    }
}
