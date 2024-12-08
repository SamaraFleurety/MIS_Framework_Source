using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class PawnCapacityModifier_Exposable : PawnCapacityModifier, IExposable
    {
        public void ExposeData()
        {
            Scribe_Defs.Look(ref capacity, "cap");
            Scribe_Values.Look(ref offset, "offset", 0);
            Scribe_Values.Look(ref postFactor, "factor", 1);
        }
    }
    public class HediffStageProperty : IExposable
    {
        //可以换成个Interface 没必要
        public Hediff_DynamicStage parent;

        public Dictionary<StatDef, float> statOffsets;

        public Dictionary<StatDef, float> statFactors;

        public Dictionary<PawnCapacityDef, PawnCapacityModifier_Exposable> capacities;

        public HediffStageProperty(Hediff_DynamicStage parent)
        {
            this.parent = parent;
            statOffsets = new();
            statFactors = new();
            capacities = new();
        }

        //offset是往offset叠加，factor是往postFactor叠乘
        public void TryAddMergeCapMod(PawnCapacityDef def, float offset = 0, float factor = 1)
        {
            capacities.TryGetValue(def, out PawnCapacityModifier_Exposable mod);
            if (mod == null)
            {
                mod = new PawnCapacityModifier_Exposable
                {
                    offset = 0,
                    capacity = def,
                };
                capacities.Add(def, mod);
            }

            mod.offset += offset;
            mod.postFactor *= factor;

            parent.Notify_StageDirty();
        }

        public void TryRemoveCapMod(PawnCapacityDef def)
        {
            if (capacities.ContainsKey(def))
            {
                capacities.Remove(def);
                parent.Notify_StageDirty();
            }
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
                TryRemoveStatModifier(def, isOffset);
            }
            else parent.Notify_StageDirty();
        }

        public void TryRemoveStatModifier(StatDef def, bool isOffset = true)
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
            Scribe_Collections.Look(ref statFactors, "statFactor", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref capacities, "capMod", LookMode.Def, LookMode.Deep);
        }
    }
}
