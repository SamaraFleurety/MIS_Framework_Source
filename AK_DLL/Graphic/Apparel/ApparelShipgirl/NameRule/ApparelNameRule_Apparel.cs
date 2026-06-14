using AK_DLL.SharedSingleton;

namespace AK_DLL.Apparels
{
    //因为没有衣服def了所以衣服def写ext里面了
    public class ApparelNameRule_Apparel : ISharedSingleton
    {
        public virtual string Label(Apparel_Shipgirl ap)
        {
            return ap.Ext.apparelInfo.label;
        }

        public virtual string Description(Apparel_Shipgirl ap)
        {
            return ap.Ext.apparelInfo.description;
        }
    }
}
