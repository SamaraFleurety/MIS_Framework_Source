using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;

namespace AK_DLL
{
    [HarmonyPatch(typeof(GameComponent_PawnDuplicator), "CopyAbilities")]
    public class Patch_ObeliskCopyAbility
    {
        [HarmonyPostfix]
        public static void postfix(Pawn pawn, Pawn newPawn)
        {
            List<Ability> abilities = newPawn.abilities.abilities;
            for (int i = abilities.Count - 1; i >= 0; i--)
            {
                Ability ability = abilities[i];
                if (ability != null && ability is VAbility_Operator)
                {
                    if (newPawn.abilities.GetAbility(ability.def) != null) newPawn.abilities.RemoveAbility(ability.def);
                }
            }
        }
    }
}
