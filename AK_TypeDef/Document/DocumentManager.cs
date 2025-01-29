using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Document
{
    public class DocumentManager : IExposable
    {
        public Dictionary<string, DocumentBase> documentTracker = new();

        List<string> saveTempKey = new();
        List<DocumentBase> saveTempVal = new();
        public void ExposeData()
        {
            Scribe_Collections.Look(ref documentTracker, "documents", LookMode.Value, LookMode.Deep, ref saveTempKey, ref saveTempVal);
        }
    }
}
