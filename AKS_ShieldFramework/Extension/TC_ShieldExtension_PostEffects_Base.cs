using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKS_Shield.Extension
{
    //大概是需要放在护盾本体下面的
    public class TC_ShieldExtension_PostEffects_Base : TC_ShieldExtension_Base
    {
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            Register();
        }

        protected virtual void Register()
        {
            if (!CompShield.registedCompEffectors.Contains(this))
            {
                Log.Message($"registered {this.GetType()}");
                CompShield.registedCompEffectors.Add(this);
            }
        }

        public virtual void Notify_Reset() { }

        public virtual void Notify_Break(DamageInfo dinfo) { }

        public virtual void Notify_Absorb(DamageInfo dinfo, bool dodged = false) { }
    }
}
