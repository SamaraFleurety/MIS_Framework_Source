using AKA_Ability.Gizmos;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_Targetor : AKAbility, ITargetingSource
    {
        public AKAbility_Targetor(AbilityTracker tracker) : base(tracker)
        {
        }
        public AKAbility_Targetor(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        protected override void InitializeGizmo()
        {
            cachedGizmo = new Gizmo_AbilityCast_Targetor
            {
                defaultLabel = def.label,
                defaultDesc = def.description,
                icon = def.Icon,
                parent = this
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

        public TargetingParameters targetParams => def.targetParams;

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
            }
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
        public void OrderForceTarget(LocalTargetInfo target)
        {
            TryCastShot(target);
        }

        public bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return CanHitTarget(target);
        }
        #endregion
    }
}
