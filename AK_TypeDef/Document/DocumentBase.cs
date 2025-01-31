using Verse;

namespace AK_DLL.Document
{
    public abstract class DocumentBase : IExposable
    {
        //存读档的时候会动态加载
        public Thing parent;

        public Pawn ParentPawn => parent as Pawn;

        public virtual string DefaultID => GetType().FullName;

        //仅存读档用。文档运行，parent是必须的。
        public DocumentBase()
        {

        }

        public DocumentBase(Thing parent) : this() 
        {
            this.parent = parent;
        }

        public virtual void ExposeData()
        {
        }
    }
}
