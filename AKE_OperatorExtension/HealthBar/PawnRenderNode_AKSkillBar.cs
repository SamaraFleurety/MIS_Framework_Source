using UnityEngine;
using Verse;

namespace AKE_OperatorExtension
{
    //渲染节点的xmls属性字段接口，保持默认
    //如果节点需要渲染，GraphicFor和MeshSetFor是必须实现的
    public class PawnRenderNodeProperties_AKSkillBar : PawnRenderNodeProperties
    {
        public PawnRenderNodeProperties_AKSkillBar()
        {
            nodeClass = typeof(PawnRenderNodeProperties_AKSkillBar);
            workerClass = typeof(PawnRenderNodeWorker_AKSkillBar);
        }
    }

    public class PawnRenderNode_AKSkillBar : PawnRenderNode
    {
        public PawnRenderNode_AKSkillBar(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        protected override void EnsureMeshesInitialized()
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
