using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class HediffStageProperty : IExposable
    {
        //可以换成个Interface 没必要
        public Hediff_DynamicStage parent;

        public Dictionary<StatDef, float> statOffsets;

        public Dictionary<StatDef, float> statFactors;

        public HediffStageProperty(Hediff_DynamicStage parent)
        {
            this.parent = parent;
        }

        //最后那个bool可能不合理 不过应该没啥扩展需求，随便吧
        public void TryAddMergeStatModifier(StatDef def, float amount, bool isOffset = true)
        {
            Dictionary<StatDef, float> dic = statFactors;
            if (isOffset) dic = statOffsets;

            if (dic.ContainsKey(def)) dic[def] += amount;
            else dic.Add(def, amount);

            if (dic[def] == 0)
            {
                TryRemoveStatModifier(def, amount, isOffset);
            }
            else parent.Notify_StageDirty();
        }

        public void TryRemoveStatModifier(StatDef def, float amount, bool isOffset = true)
        {
            Dictionary<StatDef, float> dic = statFactors;
            if (isOffset) dic = statOffsets;

            if (!dic.ContainsKey(def)) return;
            else dic.Remove(def);

            parent.Notify_StageDirty();
        }

        public virtual void ExposeData()
        {
            Scribe_Collections.Look(ref statOffsets, "statOffset", LookMode.Def, LookMode.Value);
        }
    }
}
