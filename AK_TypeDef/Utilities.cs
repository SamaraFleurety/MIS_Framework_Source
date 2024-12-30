using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_TypeDef
{
    [StaticConstructorOnStartup]
    public static class GenericUtilities
    {
        //检索圆形范围内可到达格子 轨道交易信标同款
        public static List<IntVec3> TradeableCellsAround(IntVec3 pos, Map map, float radius)
        {
            List<IntVec3> tradeableCells = new List<IntVec3>();
            //tradeableCells.Clear();
            if (!pos.InBounds(map))
            {
                return tradeableCells;
            }
            Region region = pos.GetRegion(map);
            if (region == null)
            {
                return tradeableCells;
            }
            RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null, delegate (Region r)
            {
                foreach (IntVec3 cell in r.Cells)
                {
                    if (cell.InHorDistOf(pos, radius))
                    {
                        tradeableCells.Add(cell);
                    }
                }
                return false;
            }, 16, RegionType.Set_Passable);
            return tradeableCells;
        }
    }
}
