using AK_DLL.Bezier;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKE_OperatorExtension
{
    public class Ext_MultiSegBezierBullet : DefModExtension
    {
        public BezierCurveProperty curveProperty = new();

        /// <summary>
        /// 每多少个格子分一次
        /// 实际算法时，如果10格分一次，但实际距离是25格，则会分2次，每次12.5格
        /// 也就是分出来的每段的格子数c , count <= c < count * 2
        /// </summary>
        public int segmentSliceCellCount = 10; 
    }
}
