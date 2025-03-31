using AKA_Ability.DelayedEffects;
using System.Collections.Generic.RedBlack;
using System.Linq;
using Verse;

namespace AKA_Ability
{
    public class GC_DelayedAbilityManager : GameComponent
    {
        public static RedBlackTree<int, DelayedEffectorBase> delayedAbilities = new RedBlackTree<int, DelayedEffectorBase>(); //key:被执行时的tick
        private static int first = 0;

        static int CurrentTick => Find.TickManager.TicksGame;
        public GC_DelayedAbilityManager(Game game)
        {
            delayedAbilities = new RedBlackTree<int, DelayedEffectorBase>();
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (!delayedAbilities.Any())
            {
                return;
            }
            if (first <= 0) first = delayedAbilities.GetMinKey();
            if (CurrentTick >= first)
            {
                delayedAbilities[first].TryDoEffect();
                delayedAbilities.Remove(first);
                if (delayedAbilities.Any()) first = delayedAbilities.GetMinKey();
                else first = -1;
            }
        }

        public static void AddDelayedAbilities(int delay, DelayedEffectorBase delayedAbility)
        {
            int key = CurrentTick + delay;
            while (delayedAbilities.ContainsKey(key)) ++key;
            delayedAbilities.Add(key, delayedAbility);
            if (first <= 0 || first > key) first = key;
        }

        //fixme:没做
        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
