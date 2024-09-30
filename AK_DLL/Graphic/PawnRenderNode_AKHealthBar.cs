using Verse;
using UnityEngine;

namespace AK_DLL
{
    //渲染节点的xmls属性字段接口，保持默认
    //如果节点需要渲染，GraphicFor和MeshSetFor是必须实现的
    public class PawnRenderNodeProperties_AKHealthBar : PawnRenderNodeProperties
    {
        public PawnRenderNodeProperties_AKHealthBar()
        {
            nodeClass = typeof(PawnRenderNode_AKHealthBar);
            workerClass = typeof(PawnRenderNodeWorker_AKHealthBar);
        }
    }
    public class PawnRenderNode_AKHealthBar : PawnRenderNode
    {
        public PawnRenderNode_AKHealthBar(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }
        public override Graphic GraphicFor(Pawn pawn)
        {
            return new Graphic();
        }
        public override GraphicMeshSet MeshSetFor(Pawn pawn)
        {
            return new GraphicMeshSet(new Mesh());
        }
    }
}
