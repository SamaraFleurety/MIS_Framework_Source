using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AK_DLL
{
    public class OperatorClothSetDef : Def
    {
        //public string label;
        public int order;    //每个干员的每个order只能同时有一个
        public HairDef hair = null;
        public ThingDef weapon = null;
        public List<ThingDef> apparels = new List<ThingDef>();
        public VoicePackDef voice = null;
    }
}