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
