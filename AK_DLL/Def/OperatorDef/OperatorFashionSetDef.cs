using RimWorld;
using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    public class OperatorFashionSetDef : Def
    {
        //public string label;
        public int order;                   //每个干员的每个order只能同时有一个
        public HairDef hair = null;
        public ThingDef weapon = null;
        public List<ThingDef> apparels = new List<ThingDef>();
        public int? apparelTextureIndex;    //不直接改变换装的thing，而是改变Apparel_Operator的内部index来实现外观改变。会比 List<ThingDef> apparels 更后调用(即先改thing，然后挨个改index)
        public VoicePackDef voice = null;
        public int? standIndex;             //换装也能改变游戏界面左下角显示立绘

        //nl表情兼容 带上此换装会强制禁用
        public bool forceDisableNL = false;
    }
}