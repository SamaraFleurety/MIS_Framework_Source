using System;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;
using Unity;

namespace AKA_Ability
{
    public class Command_AKAbility : Command_VerbTarget
    {
        public AKAbility ability;

        private CDandCharge Cooldown => ability.cooldown;

        private TargetMode TargetMode => ability.def.targetMode;

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
            return 1.0f - (float)this.Cooldown.CD / (float)this.Cooldown.maxCD;
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
        /*public OperatorAbilityDef abilityDef;
        public Pawn pawn;
        public override Color IconDrawColor
        {
            get
            {
                return this.defaultIconColor;
            }
        }

        public AKAbility parent;

        //public HC_Ability parentHC;
        public bool Toggled
        {
            get { return this.parentHC.autoCast; }
            set { this.parentHC.autoCast = value; }
        }
        public CDandCharge CDs
        {
            get { return this.parentHC.CDandCharges; }
        }

        //public bool needTarget = true;
        public TargetMode targetMode
        {
            get { return this.abilityDef.targetMode; }
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
            Widgets.Label(rect, this.CDs.charge + "/" + this.CDs.maxCharge);
            //冷却
            GUI.DrawTexture(new Rect(rect.x, rect.y, width(rect.width), rect.height), ContentFinder<Texture2D>.Get("UI/Abilities/White"));
            GUI.color = Color.white;
            //开关技能 右上角的勾叉
            if (this.targetMode == TargetMode.AutoEnemy)
            {
                GUI.DrawTexture(new Rect(rect.x + rect.width - 24f, rect.y, 24f, 24f), this.Toggled ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
            }
        }
        public override bool GroupsWith(Gizmo other)
        {
            return false;
        }
        //按下按钮后，在这个方法执行目标输入
        //fixme:改为akability的子类
        public override void ProcessInput(Event ev)
        {
            switch (this.targetMode)
            {
                case (TargetMode.Single): 
                    base.ProcessInput(ev);
                    break;
                case TargetMode.Self:
                    this.verb.TryStartCastOn(new LocalTargetInfo(this.pawn), new LocalTargetInfo(this.pawn));
                    break;
                case TargetMode.AutoEnemy:
                    this.parentHC.autoCast = !this.parentHC.autoCast;
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
    /*


    }*/
}
