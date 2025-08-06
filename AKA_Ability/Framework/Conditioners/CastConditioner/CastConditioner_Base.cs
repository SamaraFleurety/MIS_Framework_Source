using System;
using Verse;

namespace AKA_Ability.CastConditioner
{
    public abstract class CastConditioner_Base
    {
        //翻译引用
        [TranslationHandle]
        public string refField = "CastConditioner";
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
        //public想改成internal的 积重难返了 拉倒吧
        public abstract bool Castable(AKAbility_Base instance);
    }
}