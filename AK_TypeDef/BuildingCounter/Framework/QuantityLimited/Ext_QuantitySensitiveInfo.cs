using RimWorld;
using System.Text;
using Verse;

namespace AK_DLL.Counter
{
    public class Ext_QuantitySensitiveInfo : DefModExtension
    {
        //计数器的id，如果为空则使用parent.def.defName
        //不能去遍历匹配comp，因为不保证一个建筑只有一个计数comp
        public string id = null;

        public CountLevel level = CountLevel.Global;

        public int quantityThreshold = 1;
        public bool lowerThan = true;    //true表示低于等于阈值时触发，false表示高于等于阈值时触发
        public bool allowEqual = false;  //是否允许等于阈值时触发

        public AcceptanceReport AllowPlacing(int count)
        {
            if (count == quantityThreshold)
            {
                if (allowEqual) return true;
                else goto LABEL_NOT_ALLOW;
            }
            bool res = lowerThan ? (count < quantityThreshold) : (count > quantityThreshold);
            if (res) return true;
            else goto LABEL_NOT_ALLOW;

            LABEL_NOT_ALLOW:
            StringBuilder sb = new();
            sb.Append("[AK.BC] QuantitySensitiveBuildingFailReason".Translate());
            sb.Append(" ");
            sb.Append("[AK.BC] QSBFR_Require".Translate());

            if (level == CountLevel.Local) sb.Append("[AK.BC] CountLevel_Local".Translate());
            else sb.Append("[AK.BC] CountLevel_Global".Translate());

            if (lowerThan)
                sb.Append("<");
            else
                sb.Append(">");

            if (allowEqual) sb.Append("= ");
            sb.Append(quantityThreshold);
            sb.Append("[AK.BC] QSBFR_ActualCount".Translate());
            sb.Append(count);

            //大概会显示：“放置失败。要求本地图内此建筑数量：<3，实际数量：5”
            return sb.ToString();
        }
    }
}
