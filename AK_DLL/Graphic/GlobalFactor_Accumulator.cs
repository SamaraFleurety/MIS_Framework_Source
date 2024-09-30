using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public static class GlobalFactor_Accumulator
    {
        public static float BurstFlashFactor = 0;
        public static float RotateAngle = 0;
        public static void Update()
        {
            BurstFlashFactor = (BurstFlashFactor + 0.025f) % 1.5f;
            RotateAngle = (RotateAngle + 0.25f) % 360;
        }
    }
}
