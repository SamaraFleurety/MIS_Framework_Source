using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class ScriptOnlyDef
    {
        public string label;
        public string description;
    }
    public class Ext_OperatorExtraInfo : DefModExtension
    {
        public ScriptOnlyDef apparelInfo;
        public ScriptOnlyDef shieldInfo;
    }
}
