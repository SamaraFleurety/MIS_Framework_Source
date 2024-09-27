using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //对附近敌人造成伤害并治疗自身
    public class AME_Illustrious_RiseFall : AbilityMassEffectBase
    {
        DamageDef damageDef;
        const float DAMAGE_HEAL_PER_PAWN = 20;
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            AME_Worker<Pawn> worker = new AME_Worker<Pawn>(this, doEffect_SingleTarget: delegate (AKAbility ab, Pawn victim)
            {
                if (victim.Destroyed) return;
                DamageInfo absDmgInfo = new DamageInfo(damageDef, DAMAGE_HEAL_PER_PAWN, 999, instigator: ab.CasterPawn, instigatorGuilty: false);
                //float absDmgAmt = 20;
                //absDmgInfo.SetAmount(absDmgAmt);
                //absDmgInfo.Def = damageDef;
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

                AbilityEffect_Heal.Heal(caster.CasterPawn, DAMAGE_HEAL_PER_PAWN);

            }, allPossibleTargetsInCell: delegate (AKAbility ab, IntVec3 cell)
            {
                List<Pawn> res = new List<Pawn>();
                foreach (Thing t in cell.GetThingList(ab.CasterPawn.Map))
                {
                    if (t is Pawn p && !p.Dead &&!p.Destroyed) res.Add(p);
                }
                return res;
            });
            worker.DoEffect_AllTargets(caster, target);
            return true;
        }
    }
}