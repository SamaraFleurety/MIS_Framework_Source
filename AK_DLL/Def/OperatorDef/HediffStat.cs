using Verse;

namespace AK_DLL
{
    public class HediffStat
    {
        public HediffDef hediff;
        public BodyPartDef part = null;
        public string partCustomLabel = null; //决定左手右肺等 有多个部位时的特选处理。来自body def中，part的custom label
        public float serverity = 1f;  //这个是招募时给的状态的严重度，和矿石病随机无关
        //以下是矿石病增加hediff时的加权随机
        public int randWeight = 1;
        //public int randWorse = 1;
    }
}
