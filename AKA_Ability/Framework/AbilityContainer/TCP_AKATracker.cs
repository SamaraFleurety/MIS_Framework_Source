using AKA_Ability.SharedData;
using AKA_Ability.TickCondition;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AKA_Ability
{
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

        private TCP_AKATracker Props => props as TCP_AKATracker;
        private List<AKAbilityDef> Abilities => Props.abilities;

        Apparel Parent => parent as Apparel;

        AbilityTrackerSharedDataProperty SharedDataProp => Props.sharedDataProperty;

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
                else if (parent is Pawn p) return p;
                else return null;
            }
        }

        private CompEquippable compEquippableInt;
        private Verb verbInt;

        CompEquippable EquipmentSource
        {
            get
            {
                if (compEquippableInt != null)
                {
                    return compEquippableInt;
                }
                compEquippableInt = parent.TryGetComp<CompEquippable>();
                if (compEquippableInt == null)
                {
                    return null;
                }
                return compEquippableInt;
            }
        }

        Verb Verb
        {
            get
            {
                verbInt ??= EquipmentSource.PrimaryVerb;
                return verbInt;
            }
        }

        Pawn WeaponOwner => Verb.CasterPawn;

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
                return;
            }

            tracker = Props.trackerGenProp.GenerateAbilityTracker(CasterPawn);
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
            return;
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
            if (WeaponOwner == null || WeaponOwner.Faction != Faction.OfPlayer) return Enumerable.Empty<Gizmo>();
            return tracker.GetGizmos();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            //Log.Message("expose tcp tracker");
            Scribe_Deep.Look(ref tracker, "AKATracker", CasterPawn);
            if (SharedDataProp != null) tracker.sharedData.props = SharedDataProp;
        }
    }
}
