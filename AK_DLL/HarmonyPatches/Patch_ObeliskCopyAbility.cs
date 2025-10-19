using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    [HarmonyPatch(typeof(GameComponent_PawnDuplicator), "CopyAbilities")]
    public class Patch_ObeliskCopyAbility
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, Pawn newPawn)
        {
            List<Ability> abilities = newPawn.abilities.abilities;
            for (int i = abilities.Count - 1; i >= 0; i--)
            {
                Ability ability = abilities[i];
                if (ability is VAbility_Operator)
                {
                    if (newPawn.abilities.GetAbility(ability.def) != null) newPawn.abilities.RemoveAbility(ability.def);
                }
            }
        }
    }
}
