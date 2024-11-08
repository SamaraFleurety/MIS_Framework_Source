﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    public class AE_SpawnMote : AbilityEffectBase
    {
        ThingDef moteDef;
        protected override bool DoEffect(AKAbility caster, LocalTargetInfo target)
        {
            /*Mote mote = (Mote)ThingMaker.MakeThing(moteDef);
            //mote.Attach(caster.CasterPawn);
            GenSpawn.Spawn(mote, target.Cell, caster.CasterPawn.Map);
            Log.Message($"spawning mote at {target.Cell.x}, {target.Cell.z}; {caster.CasterPawn.Map}");*/

            MoteMaker.MakeStaticMote(target.Cell, caster.CasterPawn.Map, moteDef);
            return base.DoEffect(caster, target);
        }
    }
}