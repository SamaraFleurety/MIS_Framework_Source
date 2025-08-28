using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Counter
{
    public class TCP_QuantityCountable : CompProperties
    {
        public string id = null; //计数器的id，如果为空则使用parent.def.defName

        public CountLevel countLevel = CountLevel.Global;
        public TCP_QuantityCountable()
        {
            compClass = typeof(TC_Counter);
        }
    }

    public class TC_Counter : ThingComp, IQuantityCountable
    {
        TCP_QuantityCountable Props => (TCP_QuantityCountable)props;
        public string ID
        {
            get
            {
                if (Props != null && Props.id != null) return Props.id;
                return parent.def.defName;
            }
        }

        public bool ShouldRegister => true;

        public CountLevel CountLevel
        {
            get
            {
                if (Props != null) return Props.countLevel;
                return CountLevel.Global;
            }
        }

        public Thing ParentThing => parent;


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            TryRegister();
        }
        public void TryRegister()
        {
            if (ParentThing == null) return;
            bool res = CountableManager.Instance.TryAddCountable(this);
            if (res) RegisterEffect();
        }
        public void RegisterEffect()
        {
        }

        public void TryDeregister(Map map = null)
        {
            if (ParentThing == null) return;
            bool res = CountableManager.Instance.TryRemoveCountable(this, map);
            if (res) DeregisterEffect();
        }

        public void DeregisterEffect()
        {
        }

    }
}
