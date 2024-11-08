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
    //按下按钮后仅开关技能，不涉及目标判定 技能会在合适的时候自动释放
    public class Gizmo_AbilityCast_Toggle : Gizmo_AbilityCast_Base
    {
        AKAbility_Auto Ability => parent as AKAbility_Auto;

        //是否允许控制技能开关，false的话按按钮并没有用，仅显示状态
        public bool allowToggle = true;

        public override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
        {
            base.DrawIcon(rect, buttonMat, parms);
            //开关技能 右上角的勾叉
            GUI.DrawTexture(new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f), Ability.AutoCast ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
        }

        public override void ProcessInput(Event ev)
        {
            if (!allowToggle) return;
            base.ProcessInput(ev);
            Ability.AutoCast = !Ability.AutoCast;
        }
    }
}
