using Verse;

namespace AK_DLL.Document
{
    public abstract class DocumentBase : IExposable
    {
        public Thing parent;

        public virtual string DefaultID => GetType().FullName;

        public DocumentBase(Thing parent)
        {
            this.parent = parent;
        }

        public virtual void ExposeData()
        {
            
        }
    }
}
