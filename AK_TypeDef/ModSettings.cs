using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class AK_TypeDef : Mod
    {
        public AK_TypeDef(ModContentPack content) : base(content)
        {
            Utilities_Unity.Init();
        }
    }
}
