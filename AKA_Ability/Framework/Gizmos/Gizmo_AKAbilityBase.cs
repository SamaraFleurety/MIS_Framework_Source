using System;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    /*[Obsolete]
    public class Command_AKAbility : Command_VerbTarget
    {
        public AKAbility ability;

        private CoolDown Cooldown => ability.cooldown;

        //private TargetMode TargetMode => ability.def.targetMode;

        private bool Toggled => (ability as AKAbility_Toggle).AutoCast;

        public Action Action;

        public override void DrawIcon(Rect rect, Material buttonMat, GizmoRenderParms parms)
        {
            Texture2D badTex = this.icon as Texture2D;
            if (badTex == null)
            {
                badTex = BaseContent.BadTex;
            }
            rect.position += new Vector2(this.iconOffset.x * rect.size.x, this.iconOffset.y * rect.size.y);
            if (!this.disabled || parms.lowLight)
            {
                GUI.color = this.IconDrawColor;
            }
            if (parms.lowLight)
            {
                GUI.color = GUI.color.ToTransparent(0.6f);
            }
            Widgets.DrawTextureFitted(rect, badTex, this.iconDrawScale * 0.85f, this.iconProportions, this.iconTexCoords, this.iconAngle, null);
            //充能
            Widgets.Label(rect, this.Cooldown.charge + "/" + this.Cooldown.maxCharge);
            //冷却
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height, rect.width, rect.height * CooldownPercent() * -1), ContentFinder<Texture2D>.Get("UI/Abilities/White"));
            GUI.color = Color.white;
            //开关技能 右上角的勾叉
            if (this.TargetMode == TargetMode.AutoEnemy)
            {
                GUI.DrawTexture(new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f), this.Toggled ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
            }
        }
        private float CooldownPercent()
        {
            if (Cooldown.charge == Cooldown.maxCharge) return 0;
            return 1.0f - (float)this.Cooldown.CDCurrent / (float)this.Cooldown.CDPerCharge;
        }

        public override void ProcessInput(Event ev)
        {
            switch (this.TargetMode)
            {
                case (TargetMode.VerbSingle):
                    base.ProcessInput(ev);
                    break;
                //不需要Verb瞄准，直接结算目标
                case TargetMode.Self:
                case TargetMode.AutoEnemy:
                    Action();
                    break;
                default:
                    break;
            }
        }

        public override Color IconDrawColor
        {
            get
            {
                return this.defaultIconColor;
            }
        }

        public override void GizmoUpdateOnMouseover()
        {
            if (TargetMode != TargetMode.VerbSingle) return;
            base.GizmoUpdateOnMouseover();
        }
    }*/
}
