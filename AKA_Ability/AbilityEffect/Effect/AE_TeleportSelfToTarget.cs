using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_TeleportSelfToTarget : AbilityEffectBase
    {
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            Pawn casterPawn = caster.CasterPawn;
            bool drafted = casterPawn.Drafted;
            Map map = casterPawn.Map;
            casterPawn.DeSpawn(DestroyMode.QuestLogic);
            GenSpawn.Spawn(casterPawn, target.Cell, map);
            if (drafted) casterPawn.drafter.Drafted = true;
            CameraJumper.TryJump(new GlobalTargetInfo(target.Cell, casterPawn.Map));
            return base.DoEffect(caster, target);
        }

        /*public override void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster = null)
        {
            bool drafted = false;
            if (caster.Drafted) drafted = true;
            caster.DeSpawn(DestroyMode.QuestLogic);
            GenSpawn.Spawn(caster, target, map);
            if (drafted) caster.drafter.Drafted = true;
            CameraJumper.TryJump(new GlobalTargetInfo(target, map));
        }*/
    }
}
