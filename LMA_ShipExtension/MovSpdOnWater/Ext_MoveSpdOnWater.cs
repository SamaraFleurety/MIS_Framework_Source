using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace LMA_Lib.Traits
{
    //舰娘在水上时享受移速加成。path cost会除以这个值。
    public class Ext_MoveSpdOnWater : DefModExtension
    {
        public float speedFactor = 1;
    }
}
