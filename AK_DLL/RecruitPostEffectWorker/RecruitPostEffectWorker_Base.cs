using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public abstract class RecruitPostEffectWorker_Base
    {
        public OperatorDef def;
        public Pawn operatorPawn;

        public RecruitPostEffectWorker_Base(OperatorDef def, Pawn operatorPawn)
        {
            this.def = def;
            this.operatorPawn = operatorPawn;
        }

        public abstract void RecruitPostEffect();
    }
}
