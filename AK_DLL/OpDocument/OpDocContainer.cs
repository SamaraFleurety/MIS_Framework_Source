using AK_DLL.Document;
using Verse;

namespace AK_DLL
{
    public class OpDocContainer : DocumentBase
    {
        public VAbility_Operator va;

        public OpDocContainer() : base()
        {
        }

        public OpDocContainer(Thing parent) : base(parent)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref va, "va");
        }
    }
}