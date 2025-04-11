using Verse;
using Verse.Sound;

namespace AK_DLL.DynaLoad
{
    //仅在干员存在时，从硬盘加载音频文件。取代原版的开局装载所有音频(会有1g)
    public class SubSoundDef_DynaLoading : SubSoundDef
    {
        public string modID = "MIS.Arknights";

        public static string loadingFromMod;

        bool resolved = false;
        public static bool shouldResolve = false;

        public override void ResolveReferences()
        {
            if (resolved || !shouldResolve) return;
            loadingFromMod = modID;
            base.ResolveReferences();
        }
    }
}
