using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    public class AbilityEffectsDef : Def
    {
        public List<AbilityEffectBase> compEffectList;
        public bool allowInterrupt = false;   //如果是true 那会跟游戏王一样依次执行ae效果直到返回第一个false为止 还没做
    }
}
