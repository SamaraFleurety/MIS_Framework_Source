using RimWorld;
using Verse;

namespace AK_DLL
{
    //干员的衣服，可以切换贴图(形态)
    //当前不需要支持变参数
    public class Apparel_Operator : Apparel
    {
        public int graphicIndex;
        private Graphic cachedGraphic;
        public override Graphic Graphic
        {
            get
            {
                cachedGraphic ??= (def.graphicData as GraphicData_MultiFoam)?.GetGraphicWithIndex(graphicIndex, this);
                return cachedGraphic;
            }
        }

        public virtual string WornGraphicPath_MultiFoam => (def.graphicData as GraphicData_MultiFoam)?.GetWornGraphicPathWithIndex(graphicIndex, this);

        public virtual void SetGraphicIndex(int newIndex)
        {
            graphicIndex = newIndex;
            cachedGraphic = null;
            Map map = this.Map;
            Thing source = this;
            if (map == null)
            {
                map = Wearer?.Map;
                source = Wearer;
            }
            if (map != null && source != null)
            {
                source.DirtyMapMesh(map);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref graphicIndex, "gIndex");
        }
    }
}