using Verse;

namespace AKS_Shield.Effector
{
    public abstract class AbsorbDamageEffector_Base
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wearer"></param>
        /// <param name="shield"></param>
        /// <param name="dinfo"></param>
        /// <param name="dodged">是否触发护盾闪避</param>
        public abstract void PostAbsorbDamage(Pawn wearer, TC_GenericShield shield, ref DamageInfo dinfo, bool dodged = false);
    }
}
