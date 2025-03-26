namespace AKA_Ability.Range
{
    //严格来讲这堆光辉的技能可能该单开dll 但要不是光辉不会有这些时髦系统 评价是吃水不忘挖井人
    public abstract class RangeWorker_Base
    {
        public AKAbility_Base parent;

        public RangeWorker_Base(AKAbility_Base parent)
        {
            this.parent = parent;
        }

        public abstract float Range();
    }
}
