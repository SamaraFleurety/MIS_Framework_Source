﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AKA_Ability.AbilityEffect
{
    //方便用来直接释放各种范围特效
    public class AE_DoEffectFX : AbilityEffectBase
    {
        public bool doVisualEffects = true; //是否启用视觉特效
        public bool doFlash = true;
        public bool doSmoke = false;

        public FloatRange radiusRange = new(10f, 10f);
        public float RandomizedRadius => radiusRange.RandomInRange;
        public float heatEnergyPerCell = 0f; //每格所施加的热量能量
        public float screenShakeFactor = 0f; //摇晃系数

        public float fxInteriorCellCountMultiplier = 1f;
        public float fxInteriorCellDistanceMultiplier = 0.7f;

        public EffecterDef fxInteriorEffecter; //内部特效
        public ThingDef fxInteriorMote; //内部mote
        public FleckDef fxInteriorFleck; //内部Fleck
        public ThingDef fxCenterMote; //中心mote
        public FleckDef fxCenterFleck; //中心Fleck
        public EffecterDef fxCenterEffecter; //中心特效

        public List<SoundDef> useSounds = new(); //播放音效

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            //延迟技能已经录入GC但是可能执行的时候技能容器已经带着container一起炸了
            if (caster.CasterPawn == null) return false;

            IntVec3 position = target.Cell;
            Map map = caster.CasterPawn.Map;

            if (heatEnergyPerCell > float.Epsilon)
            {
                GenTemperature.PushHeat(position, map, heatEnergyPerCell * GenRadial.RadialCellsAround(position, RandomizedRadius, true).Count());
            }
            if (doVisualEffects)
            {
                if (doFlash)
                {
                    FleckMaker.Static(position, map, FleckDefOf.ExplosionFlash, RandomizedRadius * 6f);
                }
                if (map == Find.CurrentMap && screenShakeFactor > 0)
                {
                    float magnitude = (position.ToVector3Shifted() - Find.Camera.transform.position).magnitude;
                    Find.CameraDriver.shaker.DoShake(4f * RandomizedRadius * screenShakeFactor / magnitude);
                }
                DoVisualEffectCenter(position, map, RandomizedRadius, doSmoke);
            }
            if (useSounds.Count > 0)
            {
                useSounds.ForEach(sound => PlayFxSound(sound, new TargetInfo(position, map)));
            }
            return base.DoEffect(caster, target);
        }

        protected virtual void DoVisualEffectCenter(IntVec3 position, Map map, float radius, bool doSmoke = false)
        {
            if (doSmoke)
            {
                for (int i = 0; i < 4; i++)
                {
                    FleckMaker.ThrowSmoke(position.ToVector3Shifted() + Gen.RandomHorizontalVector(radius * 0.7f), map, radius * 0.6f);
                }
            }
            if (fxCenterFleck != null)
            {
                FleckMaker.Static(position.ToVector3Shifted(), map, fxCenterFleck);
            }
            else if (fxCenterMote != null)
            {
                MoteMaker.MakeStaticMote(position.ToVector3Shifted(), map, fxCenterMote);
            }
            fxCenterEffecter?.Spawn(position, map, Vector3.zero);
            if (fxInteriorMote == null && fxInteriorFleck == null && fxInteriorEffecter == null)
            {
                return;
            }
            int num = Mathf.RoundToInt((float)Math.PI * radius * radius / 6f * fxInteriorCellCountMultiplier);
            for (int j = 0; j < num; j++)
            {
                Vector3 vector = Gen.RandomHorizontalVector(radius * fxInteriorCellDistanceMultiplier);
                if (fxInteriorEffecter != null)
                {
                    Vector3 vect = position.ToVector3Shifted() + vector;
                    fxInteriorEffecter.Spawn(position, vect.ToIntVec3(), map);
                }
                else if (fxInteriorFleck != null)
                {
                    FleckMaker.ThrowExplosionInterior(position.ToVector3Shifted() + vector, map, fxInteriorFleck);
                }
                else
                {
                    MoteMaker.ThrowExplosionInteriorMote(position.ToVector3Shifted() + vector, map, fxInteriorMote);
                }
            }
        }

        private void PlayFxSound(SoundDef explosionSound, TargetInfo targetInfo)
        {
            if ((!Prefs.DevMode) ? (!explosionSound.NullOrUndefined()) : (explosionSound != null))
            {
                explosionSound.PlayOneShot(targetInfo);
            }
        }
    }
}
