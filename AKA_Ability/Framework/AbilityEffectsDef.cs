using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AbilityEffectsDef : Def
    {
        public List<AbilityEffectBase> compEffectList;
        public bool allowInterrupt = false;   //如果是true 那会跟游戏王一样依次执行ae效果直到返回第一个false为止 还没做
    }
}
