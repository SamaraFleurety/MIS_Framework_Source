using System;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;

namespace AK_DLL
{
    public class CompAbilityEffect_Push : AbilityEffectBase //暂时废除，待我学成归来
    {
        //public override void DoEffect_Pawn(Pawn user, Thing target)
        //{
        //    base.DoEffect_Pawn(user, target);
        //    float angle = user.Position.ToVector3().AngleToFlat(target.Position.ToVector3());
        //    Rot4? rot = null;
        //    switch (angle)
        //    {
        //        case 90f:rot = Rot4.South;break;
        //        case -90f: rot = Rot4.North;break;
        //        case 0f:rot = Rot4.East;break;
        //        case -180f:rot = Rot4.West;break;
        //    }
        //    if (rot != null)
        //    {
        //        Push_RightAngle((Rot4)rot,(Pawn)target);
        //    }
        //    else 
        //    {
        //        Push_NonRightAngle(angle);
        //    }
        //}

        //private void Push_NonRightAngle(float angle) 
        //{
            
        //}
        //private void Push_RightAngle(Rot4 rot,Pawn target) 
        //{
        //    if (rot == Rot4.North) 
        //    {
        //        for (int i = 0;i<Prop.distance;i++) 
        //        {
        //            IntVec3 intVec3 = new IntVec3(target.Position.x,target.Position.y,target.Position.z+1);
        //            if (!StopPush(intVec3, target))
        //            {
        //                target.Position = intVec3;
        //                Log.Message("目标坐标{0},预定坐标{1}".Translate(target.Position, intVec3));
        //            }
        //        }
        //    }
        //    if (rot == Rot4.South)
        //    {
        //        for (int i = 0; i < Prop.distance; i++)
        //        {
        //            IntVec3 intVec3 = new IntVec3(target.Position.x, target.Position.y, target.Position.z - 1); 
        //            if (!StopPush(intVec3, target))
        //            {
        //                target.Position = intVec3;
        //                Log.Message("目标坐标{0},预定坐标{1}".Translate(target.Position, intVec3));
        //            }
        //        }
        //    }
        //    if (rot == Rot4.West)
        //    {
        //        for (int i = 0; i < Prop.distance; i++)
        //        {
        //            IntVec3 intVec3 = new IntVec3(target.Position.x-1, target.Position.y, target.Position.z);
        //            if (!StopPush(intVec3,target))
        //            {
        //                target.Position = intVec3;
        //                Log.Message("目标坐标{0},预定坐标{1}".Translate(target.Position, intVec3));
        //            }
        //        }
        //    }
        //    if (rot == Rot4.East)
        //    {
        //        for (int i = 0; i < Prop.distance; i++)
        //        {
        //            IntVec3 intVec3 = new IntVec3(target.Position.x+1, target.Position.y, target.Position.z);
        //            if (!StopPush(intVec3, target))
        //            {
        //                target.Position = intVec3;
        //                Log.Message("目标坐标{0},预定坐标{1}".Translate(target.Position, intVec3));
        //            }
        //        }
        //    }
        //}
        //private bool StopPush(IntVec3 intVec3, Pawn target) 
        //{
        //    if (!intVec3.InBounds(target.Map))
        //    {
        //        target.Destroy();
        //        return true;
        //    }
        //    if (intVec3.GetTerrain(target.Map).passability == Traversability.Impassable)
        //    {
        //        target.equipment.bondedWeapon = null;
        //        target.Kill(null);
        //        target.Corpse.Destroy();
        //        return true;
        //    }
        //    if (intVec3.GetFirstBuilding(target.Map) != null)
        //    {
        //        target.TakeDamage(new DamageInfo(DamageDefOf.Blunt, 30f, 0f, -1, null, target.RaceProps.body.corePart, null, DamageInfo.SourceCategory.ThingOrUnknown, target, true, true));
        //        return true;
        //    }
        //    return false;
        //}
    }
}