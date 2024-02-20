using RimWorld;
using System;
using System.Collections.Generic;
using System.Collections.Generic.RedBlack;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class GC_DelayedAbilityManager : GameComponent
    {
        public static ulong tick = 0;
        public static RedBlackTree<ulong, DelayedAbility> delayedAbilities = new RedBlackTree<ulong, DelayedAbility>(); //key:被执行时的tick
        private static ulong first = 0;
        public GC_DelayedAbilityManager(Game game)
        {
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (!delayedAbilities.Any())
            {
                tick = 0;
                return;
            }
            ++tick;
            if (tick >= first)
            {
                delayedAbilities[first].DoEffect_Delayed();
                delayedAbilities.Remove(first);
                if (delayedAbilities.Any()) first = delayedAbilities.GetMinKey();
            }
        }

        public static void AddDelayedAbilities(uint delay, DelayedAbility delayedAbility)
        {
            ulong key = tick + delay;
            while (delayedAbilities.ContainsKey(key)) ++key;
            delayedAbilities.Add(key, delayedAbility);
            if (first == 0 || first > key) first = key;
        }
    }
}
