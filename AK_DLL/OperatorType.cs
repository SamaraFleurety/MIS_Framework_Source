using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AK_DLL
{
    public enum OperatorType : Byte
    {
        Caster,//术士
        Defender,//重装
        Guard,//近卫
        Vanguard,//先锋
        Specialist,//特种
        Supporter,//辅助
        Medic,//医疗
        Sniper,//狙击
        Count//用来计数 不然以后yj加种类就傻眼了
    }
}
