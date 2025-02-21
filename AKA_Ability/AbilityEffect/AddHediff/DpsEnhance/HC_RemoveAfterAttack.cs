using Verse;

namespace AKA_Ability
{
    public class HC_RemoveAfterAttack : HediffComp
    {
        bool schedueRemoval = false;
        public override void Notify_PawnUsedVerb(Verb verb, LocalTargetInfo target)
        {
            schedueRemoval = true;
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if (Pawn.CurrentEffectiveVerb.WarmingUp)
            {
                (Pawn.stances.curStance as Stance_Warmup).ticksLeft = 0;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (schedueRemoval) parent.pawn.health.RemoveHediff(parent);
        }
    }
}
