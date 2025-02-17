using Verse;
using UnityEngine;

namespace AKA_Ability
{
    public class PawnRenderNode_HeldWeaponIcon : PawnRenderNode
    {
        public PawnRenderNode_HeldWeaponIcon(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree) : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.equipment?.Primary == null) return null;
            return pawn.equipment.Primary.Graphic;
        }

        public override Color ColorFor(Pawn pawn)
        {
            return Color.white;
        }
    }
}
