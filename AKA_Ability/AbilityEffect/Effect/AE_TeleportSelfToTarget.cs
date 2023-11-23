using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AKA_Ability
{
    public class AE_TeleportSelfToTarget : AbilityEffectBase
    {
        public override void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster = null)
        {
            caster.DeSpawn(DestroyMode.QuestLogic);
            GenSpawn.Spawn(caster, target, map);
            CameraJumper.TryJump(new GlobalTargetInfo(target, map));
        }
    }
}
