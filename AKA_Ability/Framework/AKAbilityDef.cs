using AKA_Ability.CastConditioner;
using AKA_Ability.Cooldown;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    /// <summary>
    /// 本框架使用手册：
    /// 需要先生成一个Tracker，然后往tracker里面通过本def，每个技能生成一个ability实例。
    /// ability实例会自动处理冷却等
    /// </summary>
    public class AKAbilityDef : Def
    {
        //cd
        public CooldownProperty cooldownProperty;

        //图像
        public string icon;
        public bool displayGizmoUndraft = false;  //不征召时是否显示按钮

        public bool grouped = false;        //技能分组 分组的技能同时只准带一个
        //public List<>

        public Type abilityClass;           //技能class 目标选择逻辑和gizmo都在里面

        public List<AbilityEffectBase> compEffectList;
        public bool allowInterrupt = false;   //是否允许技能效果执行时中断 如果是true 那会跟游戏王一样依次执行ae效果直到返回第一个false为止

        //目标选择
        public TargetingParameters targetParams = TargetingParameters.ForPawns();
        public float range = 0;
        public float field = 0;                 //技能范围
        public Type rangeWorker = null;         //可变射程 这个比上面的float优先
        public bool reqLineofSight = false;     //是否需要视线

        //释放技能后会播放的音效
        public List<SoundDef> useSounds = new List<SoundDef>();

        //每10tick判定一次技能是否可用
        public int castConditionJudgeInterval = 10;
        //要满足所有条件才可以释放
        public List<CastConditioner_Base> castConditions = new();

        public int sortOrder = 999; //自动排序，值越小越前

        public List<Type> inertiaConditions = new();
        #region 非xml可填参数
        public Texture2D Icon => ContentFinder<Texture2D>.Get(icon);
        #endregion
    }
}