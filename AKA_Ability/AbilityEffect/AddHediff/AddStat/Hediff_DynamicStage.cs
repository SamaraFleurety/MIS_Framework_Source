using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class Hediff_DynamicStage : HediffWithComps
    {
        public HediffStageProperty stageProperty;

        protected HediffStage cachedStage = null;

        public Hediff_DynamicStage()
        {
            stageProperty = new HediffStageProperty(this);
        }

        public override HediffStage CurStage
        {
            get
            {
                RefreshStage();
                return cachedStage;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref stageProperty, "stage", this);
        }

        public void Notify_StageDirty()
        {
            cachedStage = null;
        }

        protected virtual void RefreshStage()
        {
            if (cachedStage != null) return;
            cachedStage = new HediffStage();
            cachedStage.statOffsets = new();
            cachedStage.statFactors = new();

            foreach (StatDef stat in stageProperty.statOffsets.Keys)
            {
                cachedStage.statOffsets.Add(new StatModifier
                {
                    stat = stat,
                    value = stageProperty.statOffsets[stat]
                });
            }

            foreach (StatDef stat in stageProperty.statFactors.Keys)
            {
                cachedStage.statFactors.Add(new StatModifier
                {
                    stat = stat,
                    value = stageProperty.statFactors[stat]
                });
            }

            foreach (PawnCapacityModifier modifier in stageProperty.capacities.Values)
            {
                cachedStage.capMods.Add(modifier);
            }
        }

        public void ForceRefreshStage()
        {
            cachedStage = null;
            RefreshStage();
        }
    }
}
