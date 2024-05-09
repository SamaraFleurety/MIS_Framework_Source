using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace AK_DLL 
{ 
    public class Trap : Building_TrapExplosive
    {
        public override void Tick()
        {
            base.Tick();
            age++;
            if (age > duration) 
            {
                this.Destroy();
            }
        }
        public override void Draw()
        {
            base.Draw();
        }
        public int age = 0;
        public int duration;
    }
}
