namespace AK_DLL
{
    public static class GlobalFactor_Accumulator
    {
        public static float BurstFlashFactor { get; private set; }
        public static float RotateAngle { get; private set; }

        public static void Update()
        {
            BurstFlashFactor = (BurstFlashFactor + 0.015f) % 1.5f;
            RotateAngle = (RotateAngle + 0.25f) % 360;
        }
    }
}
