using RimWorld;

namespace SFLib
{
    public static class TypeDef
    {
    }

    public static class HarmonyPrefixRet
    {
        public static bool skipOriginal = false;
        public static bool keepOriginal = true;
    }

    [DefOf]
    public static class SFDefOf
    {
        static SFDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SFDefOf));
        }

        public static RenderSkipFlagDef Body;
    }
}
