using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    //每一层的加成固定，但是可以整数叠加
    //见过小日子写的类似功能hediff，真的用原版方式挨着写了几十个stage，太逆天了
    public class Hediff_DynamicStage_Stacks : Hediff_DynamicStage
    {
        //public HediffStageProperty stageProp_SingleStack;  //单层的加成
        //单层的加成就是父类的stageProperty

        public int stacks = 1; //层数
        public Hediff_DynamicStage_Stacks() : base()
        {
            //stageProp_SingleStack = new HediffStageProperty(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref stacks, "stacks", 1);
        }

        public override string Label => base.Label + $"x {stacks}层";

        protected override void RefreshStage()
        {
            if (cachedStage != null) return;

            RefreshStageProperty();

            cachedStage = new HediffStage
            {
                statOffsets = new(),
                statFactors = new()
            };

            foreach (StatDef stat in stageProperty.statOffsets.Keys)
            {
                cachedStage.statOffsets.Add(new StatModifier
                {
                    stat = stat,
                    value = stageProperty.statOffsets[stat] * stacks
                });
            }

            foreach (StatDef stat in stageProperty.statFactors.Keys)
            {
                cachedStage.statFactors.Add(new StatModifier
                {
                    stat = stat,
                    value = stageProperty.statFactors[stat] * stacks    
                });
            }

            foreach (PawnCapacityModifier modifier in stageProperty.capacities.Values)
            {
                PawnCapacityModifier modifierWithStacks = new PawnCapacityModifier
                {
                    capacity = modifier.capacity,
                    offset = modifier.offset * stacks,
                    postFactor = (modifier.postFactor - 1) * stacks + 1,
                    statFactorMod = modifier.statFactorMod,
                    setMaxCurveOverride = modifier.setMaxCurveOverride,
                    setMaxCurveEvaluateStat = modifier.setMaxCurveEvaluateStat
                    //不包含setmax
                };
                cachedStage.capMods.Add(modifierWithStacks);
            }

            cachedStage.regeneration = stageProperty.regeneration;

            foreach (var pair in stageProperty.miscFieldsValue)
            {
                var field = typeof(HediffStage).GetField(pair.Key);
                if (field != null)
                {
                    if (field.FieldType == typeof(bool))
                    {
                        bool flag = pair.Value != 0 ? true : false;  //bool转int，非0为true，0是false
                        field.SetValue(cachedStage, flag);
                    }
                    field.SetValue(cachedStage, pair.Value * stacks);
                }
                else
                {
                    Log.Error($"[AK] {this.Label} 的stage无名为{pair.Key}的字段");
                }
            }

            //刷新
            pawn.health.hediffSet.DirtyCache();
            pawn.health.Notify_HediffChanged(this);
        }
    }
}
