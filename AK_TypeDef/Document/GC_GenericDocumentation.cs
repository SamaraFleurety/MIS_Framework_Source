using AK_DLL.Counter;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Verse;

namespace AK_DLL.Document
{
    public class GC_Generic : GameComponent
    {
        public static GC_Generic instance;

        //key没有其他强引用时，这里面的key的键值对会在gc时被自动移除。但是gc不是实时的。
        //给每个thing额外增加一个文档tracker。里面可以含有多个文档。
        public ConditionalWeakTable<Thing, DocumentTracker> documents = new();
        List<Thing> key = new();
        List<DocumentTracker> val = new();

        public CountableManager countableManager = new();

        public GC_Generic(Game game)
        {
            instance = this;
        }

        MethodInfo fieldWeakTableKeys = typeof(ConditionalWeakTable<Thing, DocumentTracker>).GetProperty("Keys", BindingFlags.Instance | BindingFlags.NonPublic).GetGetMethod(true);

        ICollection<Thing> WeakTableKeys
        {
            get
            {
                return (ICollection<Thing>)fieldWeakTableKeys.Invoke(documents, new object[] { });
            }
        }

        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
            {
                key = new();
                val = new();
                foreach (Thing t in WeakTableKeys)
                {
                    if (t == null || t.Discarded) continue;
                    key.Add(t);
                    documents.TryGetValue(t, out DocumentTracker value);
                    val.Add(value);
                }
            }
            Scribe_Collections.Look(ref key, "weakTableKey", saveDestroyedThings: true, LookMode.Reference);
            Scribe_Collections.Look(ref val, "weakTableVal", LookMode.Deep);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                documents = new();
                for (int i = 0; i < key.Count; ++i)
                {
                    documents.Add(key[i], val[i]);
                }
                key = new();
                val = new();
            }
        }

    }
}
