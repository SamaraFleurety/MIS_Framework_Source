using Verse;

namespace PA_AKPatch
{
    //带此ext的op视为与nl face不兼容（通常为贴图带脸），会隐藏掉nl的脸
    public class Ext_MarkNLIncompatible : DefModExtension
    {
        //public bool ignoreGraphic = false;
    }
}
