using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Document
{
    public class GC_Generic : GameComponent
    {
        public static GC_Generic Instance;

        public ConditionalWeakTable<Thing, DocumentManager> documents = new();

        public GC_Generic(Game game)
        {
            Instance = this;
        }

        public override void ExposeData()
        {
            base.ExposeData();
        }
    }
}
