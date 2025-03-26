using System.Collections.Generic;
using Verse;

namespace AK_DLL.Document
{
    public class DocumentTracker : IExposable
    {
        public Thing parent;

        public Dictionary<string, DocumentBase> documents = new();

        List<string> saveTempKey = new();
        List<DocumentBase> saveTempVal = new();
        
        public void ExposeData()
        {
            Scribe_References.Look(ref parent, "parent");
            Scribe_Collections.Look(ref documents, "documents", LookMode.Value, LookMode.Deep, ref saveTempKey, ref saveTempVal);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                foreach (string key in documents.Keys)
                {
                    documents[key].parent = parent;
                }
            }
        }
    }
}
