using AKA_Ability.CastConditioner;
using AKA_Ability.Cooldown;
using AKA_Ability.Range;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AKA_Ability
{
    public abstract class AKAbility : IExposable, ILoadReferenceable
    {
        public int ID = -1;
        //不能是空
        public AbilityTracker container;

        public Cooldown_Regen cooldown;

        public AKAbilityDef def;

        protected Command cachedGizmo = null;

        private RangeWorker_Base rangeWorker = null;

        int JudgeInterval => def.castConditionJudgeInterval;

        bool cachedCastableCondition = false;
        public virtual float Range
        {
            get
            {
                if (def.rangeWorker == null) return def.range;
                rangeWorker ??= (RangeWorker_Base)Activator.CreateInstance(def.rangeWorker, this);
                return rangeWorker.Range();
            }
        }

        //仅读档时要用这个 -- 任何时候缺乏def会报错
        public AKAbility(AbilityTracker tracker) /*: this()*/
        {
            this.container = tracker;
        }

        public AKAbility(AKAbilityDef def, AbilityTracker tracker) : this(tracker)
        {
            if (ID == -1) ID = Find.UniqueIDsManager.GetNextAbilityID();
            this.def = def;
            cooldown = (Cooldown_Regen)Activator.CreateInstance(def.cooldownProperty.cooldownClass, def.cooldownProperty, this);
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
            string failReason = "";
            bool castableNow = CastableNow(ref failReason);

            cachedGizmo.Disabled = false;
            if (!castableNow) cachedGizmo.Disable(failReason.Translate());
        }

        //判定是否能发动技能
        public virtual bool CastableNow(ref string failReason)
        {
            if (Find.TickManager.TicksGame % JudgeInterval == 0)
            {
                cachedCastableCondition = true;
                foreach (CastConditioner_Base i in def.castConditions)
                {
                    if (!i.Castable(this))
                    {
                        cachedCastableCondition = false;
                        failReason = i.failReason;
                        break;
                    }
                }
            }
            return cachedCastableCondition;
        }

        protected abstract void InitializeGizmo();

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Deep.Look(ref cooldown, "CD", def.cooldownProperty, this);
            Scribe_Values.Look(ref ID, "id", -1);
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
            float range = Range;
            if (range <= 0) return;
            GenDraw.DrawRadiusRing(CasterPawn.Position, range, Color.white);
        }

        public virtual void SpawnSetup()
        {
            cooldown.SpawnSetup();
        }

        public virtual void PostDespawn()
        {
            cooldown.PostDespawn();
        }

        public virtual void Notify_OwnerStricken(ref DamageInfo dinfo)
        {
            cooldown.Notify_PawnStricken(ref dinfo);
        }

        //没做
        public virtual void Notify_OwnerHitTarget(ref DamageInfo dinfo)
        {
            cooldown.Notify_PawnHitTarget(ref dinfo);
        }

        public string GetUniqueLoadID()
        {
            return "AKAbility" + ID;
        }
    }
}
