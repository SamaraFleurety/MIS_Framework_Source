using AKA_Ability.SharedData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AKA_Ability
{
    //武器用这个要改TickerType为Normal
    //放Thing上面的技能容器。可能是放在衣服上需要穿戴，也可能放在pawn上面可以直接施法
    public class TCP_AKATracker : CompProperties
    {
        [Obsolete]
        public List<AKAbilityDef> abilities = new();

        public AbilityTrackerGenerationProperty trackerGenProp = null;

        //对于多个技能共享数据(例如cd)的，这里面的才是原始数据。不可以把CD_TrackerShared等非真实数据放进来！
        [Obsolete]
        public AbilityTrackerSharedDataProperty sharedDataProperty = null;

        public TCP_AKATracker()
        {
            compClass = typeof(TC_AKATracker);
        }
    }

    public class TC_AKATracker : ThingComp
    {
        public AbilityTracker tracker;
        public TCP_AKATracker Props => props as TCP_AKATracker;
        //弃用字段找个日子全删了
        private List<AKAbilityDef> Abilities => Props.abilities;
        AbilityTrackerSharedDataProperty SharedDataProp => Props.sharedDataProperty ?? Props.trackerGenProp?.sharedDataProperty;

        Apparel Parent => parent as Apparel;
        Pawn EquipmentOwner => (EquipmentSource?.ParentHolder as Pawn_EquipmentTracker)?.pawn;
        Pawn Wearer
        {
            get
            {
                if (Parent == null) return null;
                return Parent.Wearer;
            }
        }
        Pawn CasterPawn
        {
            get
            {
                if (Wearer != null) return Wearer;
                if (parent is Pawn p) return p;
                if (EquipmentOwner != null) return EquipmentOwner;
                return null;
            }
        }

        private CompEquippable compEquippableInt;
        CompEquippable EquipmentSource
        {
            get
            {
                if (compEquippableInt != null) return compEquippableInt;
                compEquippableInt = parent.TryGetComp<CompEquippable>();
                return compEquippableInt;
            }
        }

        //holder字段之前没用 用上了
        public override void PostPostMake()
        {
            base.PostPostMake();

            if (Props.trackerGenProp == null)
            {
                Log.Warning($"[AKA] {parent.def.label} 仍在使用旧版AKA生成参数。");

                //fixme:没做ce兼容
                if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null)
                {
                    return;
                }
                tracker = new AbilityTracker(CasterPawn);
                if (SharedDataProp != null)
                {
                    tracker.sharedData = (AbilityTrackerSharedData_Base)Activator.CreateInstance(SharedDataProp.sharedDataType, tracker, SharedDataProp);
                }
                if (this.Abilities != null && this.Abilities.Count > 0)
                {
                    foreach (AKAbilityDef i in this.Abilities)
                    {
                        tracker.AddAbility(i);
                        //AKAbilityMaker.MakeAKAbility(i, AKATracker);
                    }
                }
            }
            else
            {
                tracker = Props.trackerGenProp.GenerateAbilityTracker(CasterPawn);
                tracker.holder = parent;
            }
        }

        public override void Notify_Equipped(Pawn pawn)
        {
            tracker.owner = pawn;
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            tracker.owner = null;
        }

        public override void CompTick()
        {
            if (CasterPawn == null) return;
            tracker.Tick();
        }

        public override void CompTickLong()
        {
            if (CasterPawn == null) return;
            tracker.Tick();
            return;
        }

        public override void CompTickRare()
        {
            if (CasterPawn == null) return;
            tracker.Tick();
            return;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (CasterPawn == null || CasterPawn.Faction != Faction.OfPlayer) return Enumerable.Empty<Gizmo>();
            return tracker.GetGizmos();
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            if (CasterPawn == null || CasterPawn.Faction != Faction.OfPlayer) return Enumerable.Empty<Gizmo>();
            return tracker.GetGizmos();
        }

        public virtual IEnumerable<Gizmo> CompGetWeaponGizmosExtra()
        {
            if (EquipmentOwner == null || EquipmentOwner.Faction != Faction.OfPlayer) return Enumerable.Empty<Gizmo>();
            return tracker.GetGizmos();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            //Log.Message("expose tcp tracker");
            Scribe_Deep.Look(ref tracker, "AKATracker", CasterPawn);
            if (SharedDataProp != null) tracker.sharedData.props = SharedDataProp;

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (CasterPawn != null) tracker.owner = CasterPawn; //读档时不会Notify_Equipped
            }
        }
    }
}
