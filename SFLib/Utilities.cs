namespace SFLib
{
    /*
    [StaticConstructorOnStartup]
    public static class Utilities
    {
        static Utilities ()
        {
            try
            {
                AutoPatchColorSyncApparel();
            }
            catch
            {
                Log.Error("[SF]致命错误：未初始化");
            }
        }

        private static void AutoPatchColorSyncApparel()
        {
            SFConfigDef config = DefDatabase<SFConfigDef>.GetRandom();
            if (config == null) return;
            foreach (ThingDef def in config.skinColorSyncApparels)
            {
                def.graphicData.shaderType = ShaderTypeDefOf.CutoutComplex;
                if (def.comps == null) def.comps = new List<CompProperties>();
                def.comps.Add(new TCP_SyncSkinColor());
            }
        }
    }
    */
}
