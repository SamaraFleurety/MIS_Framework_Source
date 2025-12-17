using AKA_Ability.AbilityEffect;
using AKA_Ability.Gizmos;
using RimWorld;
using System.Linq;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_Targetor : AKAbility_Base, ITargetingSource
    {
        public AKAbility_Targetor(AbilityTracker tracker) : base(tracker)
        {
        }
        public AKAbility_Targetor(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        protected override void InitializeGizmoInnate()
        {
            cachedGizmo = new Gizmo_AbilityCast_Targetor(this)
            {
                parent = this,
                icon = def.Icon,
                defaultLabel = def.label,
                defaultDesc = def.description,
            };
        }

        #region Targetor

        public bool CasterIsPawn => true;

        public bool IsMeleeAttack => false;

        public bool Targetable => true;

        public bool MultiSelect => false;

        public bool HidePawnTooltips => false;

        public Thing Caster => CasterPawn;

        public Verb GetVerb => null;

        public Texture2D UIIcon => null;

        TargetingParameters cachedTargetingParams = null;
        public TargetingParameters targetParams
        {
            get
            {
                if (cachedTargetingParams == null)
                {
                    cachedTargetingParams = def.targetParams;
                    cachedTargetingParams.validator = TargetingValidator;
                }
                return cachedTargetingParams;
            }
        }

        public virtual bool TargetingValidator(TargetInfo info)
        {
            foreach (AbilityEffectBase ae in def.compEffectList)
            {
                if (ae is ITargetingValidator validator)
                {
                    if (!validator.TargetingValidator(info)) return false;
                }
            }
            return true;
        }

        public ITargetingSource DestinationSelector => null;

        public bool CanHitTarget(LocalTargetInfo target)
        {
            if (!target.IsValid) return false;
            if (target.Cell.DistanceTo(CasterPawn.Position) > Range) return false;
            if (def.reqLineofSight && !GenSight.LineOfSight(CasterPawn.Position, target.Cell, CasterPawn.Map)) return false;
            return true;
        }

        public void DrawHighlight(LocalTargetInfo target)
        {
            this.DrawAbilityRadiusRing();

            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);
                DrawAbilityFieldRadiusAroundTarget(target);
            }
        }

        public override void DrawAbilityFieldRadiusAroundTarget(LocalTargetInfo target)
        {
            float num = HighlightFieldRadiusAroundTarget(out bool needLOSToCenter);
            if (!(num > 0.2f) || !CanHitTarget(target)) return;
            if (needLOSToCenter)
            {
                GenExplosion.RenderPredictedAreaOfEffect(target.Cell, num, Color.white);
                return;
            }
            GenDraw.DrawFieldEdges((from x in GenRadial.RadialCellsAround(target.Cell, num, useCenter: true)
                                    where x.InBounds(Find.CurrentMap)
                                    select x).ToList(), Color.white);
        }

        public virtual float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            return FieldRange;
        }

        //不知道为啥GenDraw.DrawTargetHighlight放着只有点下左键那一瞬间才会画
        public void OnGUI(LocalTargetInfo target)
        {
            bool canhit = CanHitTarget(target);

            Texture2D icon = canhit ? def.Icon : TexCommand.CannotShoot;

            GenUI.DrawMouseAttachment(icon);

            Widgets.MouseAttachedLabel(def.label);
        }

        //这是确认目标后执行功能
        public virtual void OrderForceTarget(LocalTargetInfo target)
        {
            TryCastShot(target);
        }

        public virtual bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return CanHitTarget(target);
        }
        #endregion
    }
}
