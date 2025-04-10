using RimWorld;
using System;

namespace AKA_Ability.CastConditioner
{
    public abstract class CastConditioner_Base
    {
        //想了想觉得还是有参数
        public string failReason = "AKA_Disabled";

        public bool ignoredByAuto = false;

        public bool invert = false;  //如果是true的话，会反着输出结果

        public virtual bool Castable_New(AKAbility_Base instance)
        {
            if (ignoredByAuto && instance is AKAbility_Auto) return true;

            bool res = Castable(instance);
            if (invert) res = !res;
            return res;
        }
        //比上面那个缺功能，不要再在外面用了
        [Obsolete]
        public abstract bool Castable(AKAbility_Base instance);
    }
}