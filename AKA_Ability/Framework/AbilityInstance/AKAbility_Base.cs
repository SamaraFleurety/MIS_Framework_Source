using AKA_Ability.CastConditioner;
using AKA_Ability.Cooldown;
using AKA_Ability.Range;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AKA_Ability
{
    public abstract class AKAbility_Base : IExposable, ILoadReferenceable
    {
        public int ID = -1;
        //不能是空
        public AbilityTracker container;

        public Cooldown_Regen cooldown;

        public AKAbilityDef def;

        protected Command cachedGizmo = null;

        private RangeWorker_Base rangeWorker = null;

        protected int JudgeInterval => def.castConditionJudgeInterval;

        protected bool cachedCastableCondition = false;

        protected string lastFailReason = "";
        public virtual float Range
        {
            get
            {
                if (def.rangeWorker == null) return def.range;
                rangeWorker ??= (RangeWorker_Base)Activator.CreateInstance(def.rangeWorker, this);
                return rangeWorker.Range();
            }
        }

        public virtual float FieldRange => def.field;

        //仅读档时要用这个 -- 任何时候缺乏def会报错
        public AKAbility_Base(AbilityTracker tracker) /*: this()*/
        {
            this.container = tracker;
        }

        public AKAbility_Base(AKAbilityDef def, AbilityTracker tracker) : this(tracker)
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

        public virtual IEnumerable<Command> GetGizmos()
        {
            if (!CasterPawn.Drafted && !def.displayGizmoUndraft) yield break;
            if (cachedGizmo == null) InitializeGizmoInnate();
            UpdateGizmoInnate();

            /*foreach (ExtraGizmoDrawer_Base EGD in def.extraGizmos)
            {
                EGD.UpdateExtraGizmo();
                yield return EGD.GetExtraGizmo();
            }*/
            yield return cachedGizmo;
        }

        protected virtual void UpdateGizmoInnate()
        {
            //string failReason = "";
            bool castableNow = CastableNow();

            if (!castableNow)
            {
                cachedGizmo.Disable(lastFailReason.Translate());
            }
            else cachedGizmo.Disabled = false;
        }

        //判定是否能发动技能
        public virtual bool CastableNow()
        {
            if (Find.TickManager.TicksGame % JudgeInterval == 0)
            {
                cachedCastableCondition = true;
                foreach (CastConditioner_Base i in def.castConditions)
                {
                    if (!i.Castable(this))
                    {
                        cachedCastableCondition = false;
                        lastFailReason = i.failReason;
                        break;
                    }
                }
            }
            return cachedCastableCondition;
        }

        protected abstract void InitializeGizmoInnate();

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
            cachedCastableCondition = false;
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

        //画技能的作用范围
        public virtual void DrawAbilityFieldRadiusAroundTarget(LocalTargetInfo target) 
        {
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
