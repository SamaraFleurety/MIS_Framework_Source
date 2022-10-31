using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;
using System.Reflection;

namespace AK_DLL
{
    /*[HarmonyPatch(typeof(Thing), "TakeDamage",new Type[] { typeof(DamageInfo)})]
    public class Patch_IncreasePower
    {
        [HarmonyPrefix]
        public static bool prefix(DamageInfo dinfo) 
        {
            if (dinfo.Instigator is Pawn pawn) 
            {
                foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
                {
                    if (hediff.TryGetComp<HediffComp_IncreaseAttackPower>() is HediffComp_IncreaseAttackPower comp)
                    {
                        dinfo.SetAmount(dinfo.Amount * comp.Props.damageMultiple);
                    }
                }
            }
            return true;
        }
    }*/
}