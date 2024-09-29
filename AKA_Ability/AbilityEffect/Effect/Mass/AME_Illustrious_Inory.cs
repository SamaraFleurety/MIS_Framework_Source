using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //黑光辉交织的祈愿
    //光辉以自身为圆心吸取范围内敌人的移动速度和伤害，自身获得一定量加成，并获得生命吸取。范围内队友会承受一定效果并且不提供加成。
    public class AME_Illustrious_Inory : AbilityMassEffectBase
    {
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            int enemyCnt = 0;
            AME_Worker<Pawn> worker = new AME_Worker<Pawn>(this, doEffect_SingleTarget: delegate (AKAbility ab, Pawn victim)
            {
                /*if (victim.Destroyed) return;
                if (victim.Faction != null && victim.Faction.HostileTo(Faction.OfPlayer))
                {
                    ++enemyCnt;
                }
                DamageInfo absDmgInfo = new DamageInfo(damageDef, DAMAGE_HEAL_PER_PAWN, 999, instigator: ab.CasterPawn, instigatorGuilty: false);
                if (victim.def.useHitPoints)
                {
                    victim.HitPoints -= (int)DAMAGE_HEAL_PER_PAWN;
                    if (victim.HitPoints <= 0)
                    {
                        victim.HitPoints = 0;
                        victim.Kill(absDmgInfo);
                    }
                }
                else absDmgInfo.Def.Worker.Apply(absDmgInfo, victim);

                AbilityEffect_Heal.Heal(caster.CasterPawn, DAMAGE_HEAL_PER_PAWN);*/

            }, AllPawnAliveInCell /*allPossibleTargetsInCell: delegate (AKAbility ab, IntVec3 cell)
            {
                List<Pawn> res = new List<Pawn>();
                foreach (Thing t in cell.GetThingList(ab.CasterPawn.Map))
                {
                    if (t is Pawn p && !p.Dead &&!p.Destroyed) res.Add(p);
                }
                return res;
            }*/);
            worker.DoEffect_AllTargets(caster, target);
            return true;
        }
    }
}
