using System;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
using Unity;

namespace AK_DLL
{
    public class Command_Abilities : Command_VerbTarget
    {
        public override Color IconDrawColor 
        {
            get 
            {
                return this.defaultIconColor;
            }
        }
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
            Widgets.Label(rect, this.charge + "/" + this.maxCharge);
            GUI.DrawTexture(new Rect(rect.x, rect.y,width(rect.width),rect.height), ContentFinder<Texture2D>.Get("UI/Abilities/White"));
            GUI.color = Color.white;
        }

        public override bool GroupsWith(Gizmo other)
        {
            return false;
        }

        public override void ProcessInput(Event ev)
        {
            if (this.needTarget)
            {
                base.ProcessInput(ev);
            }
            else 
            {
                foreach (AbilityEffectBase abilityEffectBase in ability.compEffectList) 
                {
                    abilityEffectBase.DoEffect_IntVec(pawn.Position, pawn.Map);
                }
            }
        }

        private float width(float rect_heiget) 
        {
            float percentage = (float)CD/(float)maxCD;
            return rect_heiget * percentage;
        }

        public int CD;
        public int maxCD;
        public AbilityType abilityType;
        public bool needTarget = true;
        public OperatorAbilityDef ability;
        public Pawn pawn;
        public int charge;
        public int maxCharge;
    }
}
