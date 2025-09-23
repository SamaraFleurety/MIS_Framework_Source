using Verse;

namespace AK_DLL.Counter
{
    public class TCP_QuantityCountable : CompProperties
    {
        public string id = null; //计数器的id，如果为空则使用parent.def.defName

        public CountLevel countLevel = CountLevel.Global;
        public TCP_QuantityCountable()
        {
            compClass = typeof(TC_QuantityCountable);
        }
    }

    public class TC_QuantityCountable : ThingComp, IQuantityCountable
    {
        TCP_QuantityCountable Props => (TCP_QuantityCountable)props;
        public virtual string ID
        {
            get
            {
                if (Props != null && Props.id != null) return Props.id;
                return parent.def.defName;
            }
        }

        public bool ShouldRegister => true;

        public virtual CountLevel CountLevel
        {
            get
            {
                if (Props != null) return Props.countLevel;
                return CountLevel.Global;
            }
        }

        public virtual Thing ParentThing => parent;


        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            TryRegister();
        }
        public virtual void TryRegister()
        {
            if (ParentThing == null) return;
            bool res = CountableManager.Instance.TryAddCountable(this);
            if (res) RegisterEffect();
        }
        public virtual void RegisterEffect()
        {
        }

        public virtual void TryDeregister(Map map = null)
        {
            if (ParentThing == null) return;
            bool res = CountableManager.Instance.TryRemoveCountable(this, map);
            if (res) DeregisterEffect();
        }

        public virtual void DeregisterEffect()
        {
        }

    }
}
