using AKA_Ability.Cooldown;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace AKA_Ability.Gizmos
{
    public abstract class Gizmo_AbilityCast_Base : Command
    {
        public AKAbility_Base parent;

        private Cooldown_Regen Cooldown => parent.cooldown;

        public Gizmo_AbilityCast_Base(AKAbility_Base parent)
        {
            this.parent = parent;
            defaultDescPostfix = GetInspectStringExtra();
        }

        public virtual string GetInspectStringExtra()
        {
            StringBuilder sb = new();
            sb.AppendLine();

            string cooldown = Cooldown?.GetExplanation();
            if (!string.IsNullOrEmpty(cooldown))
            {
                sb.AppendLine(cooldown);
            }

            var conditions = parent.def.castConditions.Select(cc => cc.GetExplanation())
                .Where(explanation => !string.IsNullOrEmpty(explanation)).ToList();
            if (conditions.Any())
            {
                sb.AppendLine($"{"SkillRequirements".Translate()}: ".Colorize(ColorLibrary.Yellow));
                foreach (var condition in conditions)
                {
                    sb.AppendLine(condition);
                }
            }
            return sb.ToString();
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
            else
            {
                GUI.color = IconDrawColor.SaturationChanged(0f);
            }
            if (parms.lowLight)
            {
                GUI.color = GUI.color.ToTransparent(0.6f);
            }
            //if (!parent.CastableNow()) GUI.color = new Color(0.3f, 0.59f, 0.11f, 1);
            Widgets.DrawTextureFitted(rect, badTex, this.iconDrawScale * 0.85f, this.iconProportions, this.iconTexCoords, this.iconAngle, buttonMat);
            //充能
            Widgets.Label(rect, this.Cooldown.Charge + "/" + this.Cooldown.MaxCharge);
            //技能冷却
            GUI.DrawTexture(new Rect(rect.x, rect.y + rect.height, rect.width, rect.height * Cooldown.CooldownPercent() * -1), ContentFinder<Texture2D>.Get("UI/Abilities/White"));
            GUI.color = Color.white;
        }

        public override void ProcessInput(Event ev)
        {
            base.ProcessInput(ev);
        }

        public override void GizmoUpdateOnMouseover()
        {
            base.GizmoUpdateOnMouseover();
            parent.DrawAbilityRadiusRing();
        }

    }
}
