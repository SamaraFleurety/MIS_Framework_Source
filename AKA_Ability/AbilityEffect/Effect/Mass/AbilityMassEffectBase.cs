using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //指定一个中心，对范围内所有有效目标造成效果
    public abstract class AbilityMassEffectBase : AbilityEffectBase
    {
        public float radius = 8;  //作用范围 和射程没关系

        /*public Type workerInnerType;

        //写不进xml，<>不读
        public Type workerType = typeof(AME_Worker<>);

        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            Type trueWorkerType = workerType.MakeGenericType(workerInnerType);

            AME_Worker<object> worker = (AME_Worker<object>)Activator.CreateInstance(trueWorkerType, this);

            worker.DoEffect_AllTargets(caster, target);
            return true;
        }*/
        public static IEnumerable<Pawn> AllPawnAliveInCell(AKAbility ab, IntVec3 cell)
        {
            //List<Pawn> res = new List<Pawn>();
            foreach (Thing t in cell.GetThingList(ab.CasterPawn.Map))
            {
                if (t is Pawn p && !p.Dead && !p.Destroyed) yield return p;
            }
            //return res;
        }
    }

    //啥比泰南的xml解析器不读泛型
    public class AME_Worker<T>
    {
        public AbilityMassEffectBase prop;

        public AME_Worker(AbilityMassEffectBase prop, Action<AKAbility, T> doEffect_SingleTarget, Func<AKAbility, IntVec3, IEnumerable<T>> allPossibleTargetsInCell)
        {
            this.prop = prop;
            //this.targetValidator = targetValidator;
            this.doEffect_SingleTarget = doEffect_SingleTarget;
            this.validatedThingsAtCell = allPossibleTargetsInCell;
        }

        //private Predicate<AKAbility, T> targetValidator;

        //对单个有效目标的效果
        private Action<AKAbility, T> doEffect_SingleTarget;


        //基本就是全部效果 不直接写在上面的doeffect是对以后对有效目标有后效预留
        public virtual List<T> DoEffect_AllTargets(AKAbility caster, LocalTargetInfo target)
        {
            List<IntVec3> affectedCells = GenRadial.RadialCellsAround(target.Cell, prop.radius, true).ToList();
            List<T> affectedTargets = new List<T>();

            foreach (IntVec3 cell in affectedCells)
            {
                /*IEnumerable<T> localTargets = ;
                if (localTargets == null || localTargets.Count() == 0) continue;*/
                
                foreach (T t in validatedThingsAtCell(caster, cell))
                {
                    doEffect_SingleTarget(caster, t);
                }
            }

            return affectedTargets;
        }

        //给定一格 返回有效目标
        /*protected virtual IEnumerable<T> ValidatedTargetsSingleCell(AKAbility caster, IntVec3 cell)
        {
            foreach (T possibleTarget in validatedThingsAtCell(caster,cell))
            {
                if (targetValidator(caster, possibleTarget)) yield return possibleTarget; 
            }
        }*/

        //每格中所有可能目标
        private Func<AKAbility, IntVec3, IEnumerable<T>> validatedThingsAtCell;
    }
}