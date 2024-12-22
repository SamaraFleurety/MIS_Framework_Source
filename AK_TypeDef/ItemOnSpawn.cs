using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class ItemOnSpawn
    {
        public ThingDef item;
        public ThingDef stuff = null;
        public QualityCategory? quality;
        public int amount = 1;
    }
}
