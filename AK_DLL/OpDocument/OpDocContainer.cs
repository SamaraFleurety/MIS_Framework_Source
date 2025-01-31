using AK_DLL.Document;
using AKA_Ability;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    public class OpDocContainer : DocumentBase
    {
        public VAbility_Operator va;

        public OpDocContainer() : base() { }

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