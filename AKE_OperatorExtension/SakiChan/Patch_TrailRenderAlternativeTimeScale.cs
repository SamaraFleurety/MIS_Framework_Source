namespace AKE_OperatorExtension.HarmonyPatchs
{
    /*[HarmonyPatch(typeof(TrailRenderer), "time")]
    public class Patch_TrailRenderAlternativeTimeScale
    {
        [HarmonyPatch(MethodType.Setter)]
        [HarmonyPrefix]
        public static bool Prefix(TrailRenderer __instance)
        {
            if (Bullet_SakiChan.cacedTrailRender.TryGetValue(__instance, out var _) && !Bullet_SakiChan.allowChangeTime)
            {
                Log.Message($"skip");
                return HarmonyPrefixRet.skipOriginal;
            }
            return HarmonyPrefixRet.keepOriginal;
        }
    }*/
}
