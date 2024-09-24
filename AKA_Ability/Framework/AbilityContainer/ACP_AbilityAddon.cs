using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class ACP_AbilityAddon : AbilityCompProperties
    {
        public ACP_AbilityAddon()
        {
            this.compClass = typeof(AC_AbilityAddon);
        }
    }

    public class AC_AbilityAddon : AbilityComp
    {
        VAbility_AKATrackerContainer VA => parent as VAbility_AKATrackerContainer;
        AbilityTracker Tracker => VA.AKATracker;
        //不知道为啥ability的这个不是virtual
        public override void Initialize(AbilityCompProperties props)
        {
            base.Initialize(props);
            Tracker.SpawnSetup();
        }
    }
}
