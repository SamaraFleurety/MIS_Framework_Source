using RimWorld;
using System;
using System.Collections.Generic;
using System.Collections.Generic.RedBlack;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL.Bezier
{
    public class BCP_SpeedAdjustable : BezierCurveProperty
    {
        //精度，精度的意思是，对于每格（直线）长度，对曲线进行多少次采样
        public int precision = 10;
        public override BezierCurveCubic GenCurveCubic(Vector3 start, Vector3 destination, int? flipOverride = null)
        {
            return new BCC_SpeedAdjustable(this, start, destination, flipOverride);
        }
    }

    public class BCC_SpeedAdjustable : BezierCurveCubic
    {

        //采样精度 每次迭代递进多少。要性能可以做个自适应。懒得做。
        float stride;
        Vector3 lastPosition;
        float lastT = 0;

        public BCC_SpeedAdjustable()
        {
        }

        public BCC_SpeedAdjustable(BezierCurveProperty prop, Vector3 start, Vector3 end, int? flipOverride = null)
            : base(prop, start, end, flipOverride)
        {
            BCP_SpeedAdjustable props = prop as BCP_SpeedAdjustable;
            float magnitude = (start - end).magnitude;
            int pointCount = 5;
            if (magnitude <= 0.1)
            {
                Log.Warning($"[AK]不正确的生成参数: {start} -> {end}");
            }
            else
            {
                pointCount = (int)(magnitude * props.precision);
            }
            //precision = pointCount;
            stride = 1.0f / (float)pointCount;
            lastPosition = start;
            /*float dist = 0;
            for (int i = 0; i <= pointCount; i++)
            {
                float t = (float)i / (float)pointCount;
                Vector3 point = BezierUtil.GetPointCubic(start, control1, control2, end, t);
                dist += (point - start).magnitude;
                //cachedPoints.Add(dist, new float2() { x = point.x, z = point.z });
            }*/
        }
        //给定一个速度，返回下一个点的位置，以及溢出的距离（如果已经超过终点）
        public Vector3 GetPoint(float speed, out float overflowDistance)
        {
            while(lastT <= 1.0f && speed > 0)
            {
                lastT += stride;
                Vector3 point = BezierUtil.GetPointCubic(start, control1, control2, end, Mathf.Min(lastT, 1.0f));
                float magnitude = (point - lastPosition).magnitude;
                speed -= magnitude;
                lastPosition = point;
            }

            if (speed < 0) speed = 0;

            overflowDistance = speed;
            return lastPosition;
        }

        //忍术：得到所有点
        public IEnumerable<Vector3> GetAllPoints()
        {
            float t = 0;
            while (t <= 1)
            {
                yield return GetPoint(t);
                t += stride;
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref lastT, "lastT", 0);
            Scribe_Values.Look(ref stride, "stride", 0.01f);
            Scribe_Values.Look(ref lastPosition, "lastPosition", Vector3.zero);
        }
    }
}
