using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public static class GlobalFactor_Accumulator
    {
        private static float BurstFlashFactor = 0;
        private static float RotateAngle = 0;
        public static float GetBurstFlashFactor => BurstFlashFactor;
        public static float GetRotateAngle => RotateAngle;
        public static void Update()
        {
            BurstFlashFactor = (BurstFlashFactor + 0.015f) % 1.5f;
            RotateAngle = (RotateAngle + 0.25f) % 360;
        }
    }
}
