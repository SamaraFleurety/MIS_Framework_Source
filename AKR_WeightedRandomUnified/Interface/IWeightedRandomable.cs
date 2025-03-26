namespace AKR_Random
{
    //有权重的单位 可能是node，也可能是最终奖品
    public interface IWeightedRandomable
    {
        public int Weight { get; }
    }
}
