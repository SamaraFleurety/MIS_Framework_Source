using AK_DLL;
using LMA_Lib.FCS;
using RimWorld;

namespace LMA_Lib
{
    public class ShipDocument : OperatorDocument
    {
        #region 运行时数据
        //火控数据求和
        private ShipFCS FCS = new();
        public bool FCS_Dirty = false;

        public ShipFCS ShipFCS
        {
            get
            {
                if (FCS_Dirty)
                {
                    FCS = new();
                    foreach (Apparel ap in pawn.apparel.WornApparel)
                    {
                        Ext_FireControlSystem ext = ap.def.GetModExtension<Ext_FireControlSystem>();
                        if (ext == null) continue;
                        FCS += ext.FCS;
                    }
                }
                return FCS;
            }
        }
        #endregion

    }
}
