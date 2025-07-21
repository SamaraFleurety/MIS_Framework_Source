using Verse;

namespace AKA_Ability.AbilityEffect
{
    //带有此接口的AE，会判断目标是否合法，并影响是否可以释放
    public interface ITargetingValidator
    {
        public bool TargetingValidator(TargetInfo info);
    }
}
