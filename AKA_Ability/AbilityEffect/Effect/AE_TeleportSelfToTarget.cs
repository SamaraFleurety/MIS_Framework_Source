using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AKA_Ability
{
    public class AE_TeleportSelfToTarget : AbilityEffectBase
    {
        public override void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster = null)
        {
            bool drafted = false;
            if (caster.Drafted) drafted = true;
            caster.DeSpawn(DestroyMode.QuestLogic);
            GenSpawn.Spawn(caster, target, map);
            if (drafted) caster.drafter.Drafted = true;
            CameraJumper.TryJump(new GlobalTargetInfo(target, map));
        }
    }
}
