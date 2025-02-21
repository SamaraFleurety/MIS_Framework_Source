using UnityEngine;
using Verse;
using RimWorld;
using System.Security.Cryptography;
using System;
using Verse.Noise;
using static HarmonyLib.Code;
using static RimWorld.FleshTypeDef;
using static UnityEngine.GraphicsBuffer;

namespace AKA_Ability.AbilityEffect
{
    public class AE_Effecter : AbilityEffectBase
    {
        public EffecterDef effecterDef;

        public float muzzleOffset = 0.5f;

        public bool invert = false;

        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            var hitPosV3 = target.HasThing ? target.Thing.DrawPos : target.Cell.ToVector3Shifted();
            var map = caster.CasterPawn.Map;
            Effecter effecter = effecterDef.Spawn();
            effecter.offset = Vector3.forward.RotatedBy((hitPosV3 - caster.CasterPawn.DrawPos).Yto0().AngleFlat()) * muzzleOffset;

            var targA = (invert ? target : caster.CasterPawn).ToTargetInfo(map);
            var targB = (invert ? caster.CasterPawn : target).ToTargetInfo(map);

            if (effecterDef.maintainTicks > 0)
            {
                map.effecterMaintainer.AddEffecterToMaintain(effecter, targA, targB, effecterDef.maintainTicks);
            }
            else
            {
                effecter.Trigger(targA, targB);
            }
            return base.DoEffect(caster, target);
        }
    }

    public class AE_ThrowFleck : AbilityEffectBase
    {
        public FleckDef fleckDef;

        public IntRange throwCountRange;

        public FloatRange speedRange, scaleRange, angleRange;


        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            var map = caster.CasterPawn.Map;
            int count = throwCountRange.RandomInRange;
            var direction = (target.Cell - caster.CasterPawn.Position).AngleFlat;
            var fcd = FleckMaker.GetDataStatic(caster.CasterPawn.DrawPos, map, fleckDef);

            for (int i = 0; i < count; i++)
            {
                var angle = angleRange.RandomInRange;
                fcd.scale = scaleRange.RandomInRange;
                fcd.spawnPosition = caster.CasterPawn.DrawPos;
                fcd.rotation = direction + angle;
                fcd.velocitySpeed = speedRange.RandomInRange;
                fcd.velocityAngle = direction + angle;
                map.flecks.CreateFleck(fcd);
            }
            return base.DoEffect(caster, target);
        }
    }


    public class SubEffecter_Sweep : SubEffecter
    {
        private int ticksUntilMote;

        private int moteCount;

        private int index;

        public SubEffecter_Sweep(SubEffecterDef def, Effecter parent)
            : base(def, parent)
        {
            moteCount = def.burstCount.RandomInRange;
            ticksUntilMote = def.initialDelayTicks;
            index = 0;
        }

        public override void SubEffectTick(TargetInfo A, TargetInfo B)
        {
            ticksUntilMote--;
            if (ticksUntilMote <= 0 && index < moteCount)
            {
                var direction = (B.Cell - A.Cell).AngleFlat + def.angle.min;
                var increment = def.angle.Span / (moteCount - 1);
                direction += increment * index;

                var fcd = FleckMaker.GetDataStatic(A.CenterVector3, A.Map, def.fleckDef);

                fcd.scale = def.scale.RandomInRange;
                fcd.spawnPosition = A.CenterVector3 + parent.offset;
                fcd.rotation = direction;
                fcd.velocitySpeed = def.speed.RandomInRange;
                fcd.velocityAngle = direction;
                A.Map.flecks.CreateFleck(fcd);
                index++;

                ticksUntilMote = def.ticksBetweenMotes;
            }
        }
    }

    public class AE_AttachedMote : AbilityEffectBase
    {
        public ThingDef moteDef;

        public float rotationRate = 10;


        protected override bool DoEffect(AKAbility_Base caster, LocalTargetInfo target)
        {
            var mote = MoteMaker.MakeAttachedOverlay(caster.CasterPawn, moteDef, Vector3.zero);
            mote.rotationRate = rotationRate;
            return base.DoEffect(caster, target);
        }
    }
}
