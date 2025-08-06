using AKA_Ability.SharedData;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability.CastConditioner
{
    public class CC_NeedSharedCharge : CC_NeedCharge
    {
        public override bool Castable(AKAbility_Base instance)
        {
            if (instance.container.sharedData is not SD_SharedCharge sdsc)
            {
                Log.Error($"[AKA] 技能尝试读取技能共享数据失败");
                return false;
            }
            return sdsc.cooldown.Charge >= chargeRequire;
        }
    }
}
