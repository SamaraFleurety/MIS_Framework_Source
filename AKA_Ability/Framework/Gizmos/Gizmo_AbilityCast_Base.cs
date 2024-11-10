using AKA_Ability.Cooldown;
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
    public abstract class Gizmo_AbilityCast_Base : Command
    {
        public AKAbility_Base parent;

        private Cooldown_Regen Cooldown => parent.cooldown;

        //private TargetMode TargetMode => ability.def.targetMode;

        //private bool Toggled => (ability as AKAbility_Toggle).AutoCast;

        //public Action Action;

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
            Widgets.Label(rect, this.Cooldown.charge + "/" + this.Cooldown.MaxCharge);
            //技能冷却
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height, rect.width, rect.height * Cooldown.CooldownPercent() * -1), ContentFinder<Texture2D>.Get("UI/Abilities/White"));
            GUI.color = Color.white;
        }

        /*protected virtual float CooldownBarFillPercent()
        {
            if (Cooldown.charge == Cooldown.maxCharge) return 0;
            return (float)this.Cooldown.CDCurrent / (float)this.Cooldown.CDPerCharge;
        }*/

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
        }

        public override Color IconDrawColor => defaultIconColor;


        public override void GizmoUpdateOnMouseover()
        {
            //if (TargetMode != TargetMode.VerbSingle) return;
            base.GizmoUpdateOnMouseover();
            parent.DrawAbilityRadiusRing();
        }

    }
}
