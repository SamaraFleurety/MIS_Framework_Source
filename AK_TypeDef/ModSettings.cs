using Verse;

namespace AK_DLL
{
    public class AK_TypeDef : Mod
    {
        public AK_TypeDef(ModContentPack content) : base(content)
        {
            Utilities_Unity.Init();
        }
    }
}
