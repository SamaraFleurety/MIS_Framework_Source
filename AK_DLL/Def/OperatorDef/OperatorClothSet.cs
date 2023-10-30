using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AK_DLL
{
    public class OperatorClothSet
    {
        [MustTranslate]
        public string label;
        public HairDef hair = null;
        public ThingDef weapon = null;
        public List<ThingDef> apparels = new List<ThingDef>();
        public VoicePackDef voice = null;
    }
}
