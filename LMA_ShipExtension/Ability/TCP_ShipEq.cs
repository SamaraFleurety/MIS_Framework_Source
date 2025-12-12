using AK_DLL;
using AKA_Ability;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LMA_Lib.Ability
{
    //舰娘舰装的开火效果是aka伪装
    //原理是穿上装备就往干员的abilitytracker里塞技能，脱下就删掉
    public class TCP_ShipEq : CompProperties
    {
        public List<AKAbilityDef> abilities = new();
        public TCP_ShipEq()
        {
            compClass = typeof(TC_ShipEq);
        }
    }

    public class TC_ShipEq : ThingComp
    {
        TCP_ShipEq Props => (TCP_ShipEq)props;

        Pawn lastEquippedShipgirl = null;

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);

            Notify_Unequipped(lastEquippedShipgirl);


            if (pawn.GetDoc() is not OperatorDocument doc || doc.parentContainer.AK_Tracker is not LMA_AbilityTracker tracker) return;
            for (int i = 0; i < Props.abilities.Count; i++)
            {
                AKAbilityDef def = Props.abilities[i];
                tracker.AddAbility(def, parent.thingIDNumber * 100 + i); //我不相信技能能有100个
            }

            lastEquippedShipgirl = pawn;
        }

        public override void Notify_Unequipped(Pawn pawn)
        {
            if (pawn == null) return;
            base.Notify_Unequipped(pawn);
            if (pawn.GetDoc() is not OperatorDocument doc || doc.parentContainer.AK_Tracker is not LMA_AbilityTracker tracker) return;
            for (int i = 0; i < Props.abilities.Count; i++)
            {
                tracker.RemoveAbility(parent.thingIDNumber * 100 + i); //我不相信技能能有100个
            }

            lastEquippedShipgirl = null;
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            base.PostDestroy(mode, previousMap);
            Notify_Unequipped(lastEquippedShipgirl);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref lastEquippedShipgirl, "lastEquippedShipgirl");
        }
    }
}
