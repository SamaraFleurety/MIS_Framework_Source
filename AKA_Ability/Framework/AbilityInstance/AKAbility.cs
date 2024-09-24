using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AKA_Ability
{
    public abstract class AKAbility : IExposable
    {
        //不能是空
        public AbilityTracker container;

        public CoolDown cooldown = new CoolDown();

        public AKAbilityDef def;

        protected Command cachedGizmo = null;

        /*public AKAbility()
        {
        }*/

        //存档时要用这个
        public AKAbility(AbilityTracker tracker) /*: this()*/
        {
            this.container = tracker;
        }

        public AKAbility(AKAbilityDef def, AbilityTracker tracker) : this(tracker)
        {
            this.def = def;
            cooldown = new CoolDown(def.maxCharge, def.CDPerCharge);
        }

        public Pawn CasterPawn => container.owner;

        public virtual void Tick()
        {
            cooldown.Tick(1);
        }

        public virtual Command GetGizmo()
        {
            if (!CasterPawn.Drafted && !def.displayGizmoUndraft) return null;
            if (cachedGizmo == null) InitializeGizmo();
            UpdateGizmo();
            return cachedGizmo;
        }

        protected virtual void UpdateGizmo()
        {
            cachedGizmo.Disabled = false;
            if (cooldown.charge == 0) cachedGizmo.Disable("AK_ChargeIsZero".Translate());
        }

        protected abstract void InitializeGizmo();

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Deep.Look(ref cooldown, "CD");
        }

        public virtual void TryCastShot(LocalTargetInfo target)
        {
            if (target == null)
            {
                Log.Error($"[AK] 释放技能{def.label} 时目标非法");
                return;
            }
            foreach (AbilityEffectBase effect in def.compEffectList)
            {
                bool success = effect.DoEffect(this, localTargetInfo: target);
                if (def.allowInterrupt && !success) return;
            }
            PlayAbilitySound();
            container.Notify_AbilityCasted(this);
        }

        public virtual void PlayAbilitySound()
        {
            foreach (SoundDef sound in def.useSounds)
            {
                sound.PlayOneShotOnCamera();
            }
        }

        //画技能范围
        public virtual void DrawAbilityRadiusRing()
        {
            float range = def.range;
            if (range <= 0) return;
            GenDraw.DrawRadiusRing(CasterPawn.Position, range, Color.white);
        }
    }
}
