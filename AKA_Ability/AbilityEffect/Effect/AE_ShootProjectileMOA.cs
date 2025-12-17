using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //主要舰炮用 齐射子弹 散布服从正太分布
    public class AE_ShootProjectileMOA : AE_ShootProjectile
    {
        //在10格时，子弹落点偏差的最大直径。实际直径会在这个圆里面做正太分布
        //类似MOA精度，即20格时误差会翻倍，5格时会减半
        public float deviationDiameter = 2;

        //连射次数 是同一帧内会射出多少发子弹，中间不会有延迟
        public int burst = 1;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            if (caster.CasterPawn is not Pawn casterPawn || !casterPawn.Spawned || casterPawn.Map is not Map map) return false;

            ShootLine shootLine = new(casterPawn.Position, ProjectileDestinationFinetuned(casterPawn.Position, target.Cell, map));

            Projectile proj = (Projectile)GenSpawn.Spawn(projectile, shootLine.Source, casterPawn.Map, WipeMode.Vanish);
            proj.Launch(casterPawn, casterPawn.DrawPos, target, target, ProjectileHitFlags.All, false, null, null);
            return base.DoEffect(caster, target);
        }

        Vector3 ProjectileDestinationMOA(Vector3 origin, Vector3 destination)
        {
            float range = (destination - origin).magnitude;

            return AKA_Algorithm.GenerateNormalDistributionPointsAboutCircle(destination, range / 10f * deviationDiameter);
        }

        IntVec3 ProjectileDestinationFinetuned(IntVec3 origin, IntVec3 destination, Map map)
        {
            Vector3 des = ProjectileDestinationMOA(origin.ToVector3(), destination.ToVector3());

            des.x = Mathf.RoundToInt(des.x);
            des.z = Mathf.RoundToInt(des.z);

            des.x = Mathf.Clamp(des.x, 0, map.Size.x);
            des.z = Mathf.Clamp(des.x, 0, map.Size.z);

            return des.ToIntVec3();
        }
    }
}
