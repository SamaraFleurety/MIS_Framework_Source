using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace AKA_Ability
{
    public class PawnRenderNodeSubWorker_StatusEffect : PawnRenderSubWorker
    {
        const int columnCount = 3;
        const float gap = 0.5f;

        public override void TransformOffset(PawnRenderNode node, PawnDrawParms parms, ref Vector3 offset, ref Vector3 pivot)
        {
            offset += Vector3.forward;
            var nodes = GetAllNodes(parms).Where(n => n.Props.subworkerClasses?.Contains(typeof(PawnRenderNodeSubWorker_StatusEffect)) ?? false);
            offset += GetOffset(nodes.Count(), nodes.FirstIndexOf(n => n == node));
        }

        public override void TransformScale(PawnRenderNode node, PawnDrawParms parms, ref Vector3 scale)
        {
            float nodes = GetAllNodes(parms).Where(n => n.Props.subworkerClasses?.Contains(typeof(PawnRenderNodeSubWorker_StatusEffect)) ?? false).Count();
            if (nodes == 1)
            {
                return;
            }
            scale *= ((float)columnCount - 1) / columnCount;
        }

        Vector3 GetOffset(int total, int index)
        {
            if (total == 1) return Vector3.zero;
            float row = 0;
            int column;
            if (total < columnCount * 2 - 1)
            {
                column = total == 2 ? 2 : (total + 1) / 2;

                if (index > column - 1)
                {
                    row += gap;
                    index -= column;
                    column = total - column;
                }
            }
            else
            {
                row = (index / columnCount) * gap;
                var reminder = total % columnCount;
                column = (total - reminder > index) ? columnCount : reminder;
                index = index % columnCount;
            }
            return new Vector3(GetRowOffset(column, index), 0, row);
        }

        float GetRowOffset(float rowWidth, int index)
        {
            if (rowWidth == 1) return 0;
            return ((-(rowWidth - 1) / 2) + index) * gap;
        }

        IEnumerable<PawnRenderNode> GetAllNodes(PawnDrawParms parms)
        {
            return GetAllNodes(parms.pawn.Drawer.renderer.renderTree.rootNode);
        }

        IEnumerable<PawnRenderNode> GetAllNodes(PawnRenderNode parentNode)
        {
            yield return parentNode;
            if (parentNode.children != null)
            {
                foreach (var node in parentNode.children)
                {
                    foreach (var subNode in GetAllNodes(node))
                    {
                        yield return subNode;
                    }
                }
            }
        }
    }
}
