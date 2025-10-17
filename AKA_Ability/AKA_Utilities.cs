using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    [StaticConstructorOnStartup]
    public static class AKA_Utilities
    {
        public static Dictionary<Pawn, HashSet<AKAbility_Base>> pawn_NotifyStricken = new();

        //攻击恢复sp 
        public static Dictionary<Pawn, HashSet<AKAbility_Base>> pawn_NotifyHitTarget = new();

        //写个泛型也行 估计这辈子就这两套，算了吧
        public static void RegisterAsStricken(Pawn owner, AKAbility_Base ab)
        {
            //Log.Message($"reg {ab.def.label} as stricken cd");
            if (!pawn_NotifyStricken.ContainsKey(owner)) pawn_NotifyStricken.Add(owner, new HashSet<AKAbility_Base>());

            if (!pawn_NotifyStricken[owner].Contains(ab)) pawn_NotifyStricken[owner].Add(ab);
        }

        public static void DeregisterAsStricken(Pawn owner, AKAbility_Base ab)
        {
            if (!pawn_NotifyStricken.ContainsKey(owner)) return;

            if (!pawn_NotifyStricken[owner].Contains(ab)) return;

            pawn_NotifyStricken[owner].Remove(ab);
        }

        public static void RegisterAsHitman(Pawn owner, AKAbility_Base ab)
        {
            if (!pawn_NotifyHitTarget.ContainsKey(owner)) pawn_NotifyHitTarget.Add(owner, new HashSet<AKAbility_Base>());

            if (!pawn_NotifyHitTarget[owner].Contains(ab)) pawn_NotifyHitTarget[owner].Add(ab);
        }

        public static void DeregisterAsHitman(Pawn owner, AKAbility_Base ab)
        {
            if (!pawn_NotifyHitTarget.ContainsKey(owner)) return;

            if (!pawn_NotifyHitTarget[owner].Contains(ab)) return;

            pawn_NotifyHitTarget[owner].Remove(ab);
        }

    }

}
