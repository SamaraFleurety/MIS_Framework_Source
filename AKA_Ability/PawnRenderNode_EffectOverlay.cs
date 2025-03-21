using Verse;
using UnityEngine;

namespace AKA_Ability
{
    public class PawnRenderNode_EffectOverlay : PawnRenderNode
    {
        public PawnRenderNode_EffectOverlay(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }
        public override Graphic GraphicFor(Pawn pawn)
        {
            return GraphicDatabase.Get<Graphic_Single>(props.texPath, props.shaderTypeDef.Shader);
        }

        public override Color ColorFor(Pawn pawn)
        {
            return Color.white;
        }
    }
}
