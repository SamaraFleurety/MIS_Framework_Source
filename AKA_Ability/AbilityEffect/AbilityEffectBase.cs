using RimWorld.Planet;
using Verse;

namespace AKA_Ability
{
    //既是property，也是效果器
    public abstract class AbilityEffectBase
    {
        //无视目标参数 永远指定自己为目标
        public bool forceTargetSelf = false;

        public virtual bool DoEffect(AKAbility_Base caster, GlobalTargetInfo globalTargetInfo = default(GlobalTargetInfo), LocalTargetInfo localTargetInfo = default(LocalTargetInfo))
        {
            if (forceTargetSelf)
            {
                localTargetInfo = new LocalTargetInfo(caster.CasterPawn);
            }

            bool globalSuccess = DoEffect(caster, globalTargetInfo);
            bool localSuccess = DoEffect(caster, localTargetInfo);

            return globalSuccess && localSuccess;
        }

        //返回是否成功执行
        protected virtual bool DoEffect(AKAbility_Base caster, LocalTargetInfo target) { return true; }

        protected virtual bool DoEffect(AKAbility_Base caster, GlobalTargetInfo target) { return true; }

        /*public virtual void DoEffect_All(Pawn caster, LocalTargetInfo targetInfo, bool delayed = false)
        {
            Pawn target = targetInfo.Pawn;
            if (target != null) DoEffect_Pawn(caster, target, delayed);
            if (targetInfo.Cell.InBounds(caster.Map)) DoEffect_IntVec(targetInfo.Cell, caster.Map, delayed, caster);
        }

        public virtual void DoEffect_All(Pawn caster, Thing target, IntVec3? cell, Map map, bool delayed = false)
        {
            if (target != null) DoEffect_Pawn(caster, target, delayed);
            if (cell is IntVec3 vec && vec.InBounds(map)) DoEffect_IntVec(vec, map, delayed, caster);
        }

        public virtual void DoEffect_Pawn(Pawn user, Thing target, bool delayed)
        {

        }
        public virtual void DoEffect_IntVec(IntVec3 target, Map map, bool delayed, Pawn caster = null)
        {

        }*/
    }
}
