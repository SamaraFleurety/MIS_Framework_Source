﻿using System;
using RimWorld;
using System.Linq;
using System.Text;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace AKA_Ability
{
    /// <summary>
    ///技能def。招募时，会在Recruit_Ability方法内，每个def都往Hediff_Operator（干员ID Hediff）上新增一个HC_Ability来保存技能cd/技能选择等数据。
    ///调用：HC_Ability会产生Gizmo（即按钮，亦称Command）。点击gizmo后，触发gizmo的ProcessInput方法，决定目标选择。决定目标后，会调用verb_ability的方法，依次执行本def中compEffectList的效果。
    /// </summary>
    public class AKAbilityDef : Def
    {
        public Type abilityClass;
        public string icon;
        public int CD = 1;
        public TimeToTick CDUnit = TimeToTick.day;
        public int maxCharge = 1;
        public int? range;
        public VerbProperties verb = null;
        public SoundDef useSound = null;
        public SFXType typeSFX = SFXType.tact;

        //public bool isSectorAbility = false;
        //public float sectorRadius;
        //public float minAngle;
        //public float maxAngle;

        public List<AbilityEffectBase> compEffectList;
        public bool needCD;
        //public bool needTarget;
        public TargetMode targetMode = TargetMode.Self;

        //public Type gizmoClass = null;
        public bool displayGizmoUndraft = false;
        public bool grouped = false;

        public Texture2D Icon => ContentFinder<Texture2D>.Get(icon);

        public bool allowPostEffect = true; //有些技能会导致pawn despawn，然后又有诸如刷脏污和声音之类的后效

        public void CastEffects(Pawn caster, IntVec3? cell, Thing target = null, Map map = null)
        {
            if (caster == null) Log.Error("AKA casting skill by null caster");
            if (true)
            {
                foreach(AbilityEffectBase i in compEffectList)
                {
                    i.DoEffect_All(caster, target, cell, map);
                }
            }
        }
    }
}