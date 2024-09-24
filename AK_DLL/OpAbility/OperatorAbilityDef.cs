using System;
using RimWorld;
using System.Linq;
using System.Text;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace AK_DLL
{/*
    /// <summary>
    ///技能def。招募时，会在Recruit_Ability方法内，每个def都往Hediff_Operator（干员ID Hediff）上新增一个HC_Ability来保存技能cd/技能选择等数据。
    ///调用：HC_Ability会产生Gizmo（即按钮，亦称Command）。点击gizmo后，触发gizmo的ProcessInput方法，决定目标选择。决定目标后，会调用verb_ability的方法，依次执行本def中compEffectList的效果。
    /// </summary>
    public class OperatorAbilityDef : Def
    {
        public string icon;
        public int CD = 1;
        public TimeToTick CDUnit = TimeToTick.day;
        public AbilityType abilityType;
        public int? range;
        public VerbProperties verb;
        public VerbProperties verb_Reclaim;
        public string iconReclaim;
        public string reclaimLabel;
        public string reclaimDesc;
        public PawnKindDef canReclaimPawn;
        public SoundDef useSound;
        public SFXType typeSFX = SFXType.tact;

        public bool isSectorAbility = false;
        public float sectorRadius;
        public float minAngle;
        public float maxAngle;

        public List<AbilityEffectBase> compEffectList;
        public bool needCD;
        //public bool needTarget;
        public TargetMode targetMode = TargetMode.Self;

        public List<HediffDef> selfHediff;
        public float debuffSeverity;

        public int maxCharge = 1;
        public bool displayOnUndraft = false;
        public bool grouped = false;

        public Texture2D Icon => ContentFinder<Texture2D>.Get(icon);
    }*/
}