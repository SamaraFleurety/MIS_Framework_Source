namespace AK_DLL.HarmonyPatchs
{
    /*[HarmonyPatch(typeof(ModContentPack), "AddDef")]
    public static class Patch_DefsInMod
    {
        [HarmonyPrefix]
        public static bool prefix(Def def)
        {
            if (ShouldSkipDefname(def.defName))
            {
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }

        public static bool ShouldSkipDefname(string defname)
        {
            if (defname.Contains("AK_") || defname.Contains("AKS")) return true;
            return false;
        }
    }*/
}
