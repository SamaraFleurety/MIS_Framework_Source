using Verse;

namespace AK_DLL.Counter
{
    public class PW_QuantitySensitiveBuilding : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            Ext_QuantitySensitiveInfo ext = checkingDef.GetModExtension<Ext_QuantitySensitiveInfo>();
            if (ext == null)
            {
                Log.Error($"[AK.BC] PW_QuantitySensitiveBuilding必须和配套的ext一起使用");
                return false;
            }

            int count = CountableManager.Instance.GetCountablesCount(checkingDef.defName, ext.level);

            return base.AllowsPlacing(checkingDef, loc, rot, map, thingToIgnore, thing);
        }
    }
}
