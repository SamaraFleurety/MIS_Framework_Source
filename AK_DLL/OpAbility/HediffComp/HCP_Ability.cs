using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using System.Threading.Tasks;

namespace AK_DLL
{
    //预防出错保留 大概率根本没啥用
    public class HCP_Ability : HediffCompProperties
    {
        public HCP_Ability()
        {
            this.compClass = typeof(HC_Ability);
        }
    }
}