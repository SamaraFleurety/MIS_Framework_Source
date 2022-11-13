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

        public CompAbility parent;
        public bool Toggled
        {
            get { return this.parent.autoCast; }
            set { this.parent.autoCast = value; }
        }
        public CDandCharge CDs
        {
            get { return this.parent.CDandCharges; }
        }

        //public bool needTarget = true;
        public TargetMode targetMode
        {
            get { return this.abilityDef.targetMode; }
        }
        public OperatorAbilityDef abilityDef;
        public Pawn pawn;

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
            Widgets.Label(rect, this.CDs.charge + "/" + this.CDs.maxCharge);
            GUI.DrawTexture(new Rect(rect.x, rect.y, width(rect.width), rect.height), ContentFinder<Texture2D>.Get("UI/Abilities/White"));
            GUI.color = Color.white;
            if (this.targetMode == TargetMode.AutoEnemy)
            {
                GUI.DrawTexture(new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f), this.Toggled ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
            }
        }
        public override bool GroupsWith(Gizmo other)
        {
            return false;
        }

        public override void ProcessInput(Event ev)
        {
            /*Log.Message("needTarget: "+ (this.needTarget).ToString()); 
            Log.Message("verb: " + (this.verb == null).ToString());
            Log.Message("pawn: " + (this.pawn == null).ToString());
            Log.Message("pos: " + (this.pawn.Position == null).ToString());*/
            switch (this.targetMode)
            {
                case (TargetMode.Single): 
                    base.ProcessInput(ev);
                    break;
                case TargetMode.Self:
                    this.verb.TryStartCastOn(new LocalTargetInfo(this.pawn), new LocalTargetInfo(this.pawn));
                    break;
                case TargetMode.AutoEnemy:
                    this.parent.autoCast = !this.parent.autoCast;
                    break;
                default:
                    break;
            }
            /*if (this.needTarget)
            {
                base.ProcessInput(ev);
            }
            else
            {
                Log.Message("verbDef:" + (this.ability == null).ToString());
                Log.Message("comp: " + (this.ability.compEffectList != null).ToString());
                Log.Message("cnt: " + this.ability.compEffectList.Count().ToString());
                if (this.ability.compEffectList != null && this.ability.compEffectList.Count() > 0)
                {
                    
                    /*foreach (AbilityEffectBase abilityEffectBase in this.ability.compEffectList)
                    {
                        abilityEffectBase.DoEffect_IntVec(this.pawn.Position, this.pawn.Map);
                    }
                }
            }*/
        }

        private float width(float rect_heiget)
        {
            float percentage = (float)this.CDs.CD / (float)this.CDs.maxCD;
            return rect_heiget * percentage;
        }

    }
}
