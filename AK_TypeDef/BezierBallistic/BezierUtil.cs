using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

//引进自鼠士德的MstToyBox，有改动
namespace AK_DLL.Bezier
{
    /// <summary>
    /// 贝塞尔曲线工具类，支持绘制二阶/三阶贝塞尔曲线与路径估算。
    /// 用法示例：
    /// Vector3 pos = MstBezierUtil.GetPointCubic(p0, p1, p2, p3, t);
    /// </summary>
    public static class BezierUtil
    {
        //二阶，会绘制出类似山丘的曲线
        public static Vector3 GetPointQuadratic(Vector3 start, Vector3 control, Vector3 end, float t)
        {
            float num = 1f - t;
            return num * num * start + 2f * num * t * control + t * t * end;
        }

        //三阶，可以绘制出类似S型的曲线，也可能十分多变
        public static Vector3 GetPointCubic(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector3 point = uuu * p0;                // (1 - t)^3 * P0
            point += 3f * uu * t * p1;               // 3 * (1 - t)^2 * t * P1
            point += 3f * u * tt * p2;               // 3 * (1 - t) * t^2 * P2
            point += ttt * p3;                       // t^3 * P3

            return point;
        }

        //每step取样，模拟绘制曲线
        public static void DrawQuadraticCurve(Vector3 start, Vector3 control, Vector3 end, Material mat, float layer, float width, int steps = 20)
        {
            Vector3 a = start;
            for (int i = 1; i <= steps; i++)
            {
                float t = (float)i / (float)steps;
                Vector3 point = GetPointQuadratic(start, control, end, t);
                GenDraw.DrawLineBetween(a, point, layer, mat, width);
                a = point;
            }
        }

        public static void DrawCubicCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Material mat, float layer, float width, int steps = 20)
        {
            Vector3 prev = p0; // 初始点为 p0，相当于 t=0 处
            for (int i = 1; i <= steps; i++)
            {
                float t = (float)i / steps;
                Vector3 point = GetPointCubic(p0, p1, p2, p3, t);
                GenDraw.DrawLineBetween(prev, point, layer, mat, width);
                prev = point;
            }
        }

        //看了下，这个东西基本上没有初等函数形式的解析解去求长度
        public static float EstimateQuadraticLength(Vector3 start, Vector3 control, Vector3 end, int steps = 20)
        {
            float length = 0f;
            Vector3 a = start;
            for (int i = 1; i <= steps; i++)
            {
                float t = (float)i / (float)steps;
                Vector3 point = GetPointQuadratic(start, control, end, t);
                length += Vector3.Distance(a, point);
                a = point;
            }
            return length;
        }

        public static float EstimateCubicLength(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int steps = 20)
        {
            float length = 0f;
            Vector3 prev = p0; // 初始点为 p0，相当于 t=0 处
            for (int i = 1; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector3 point = GetPointCubic(p0, p1, p2, p3, t);
                length += Vector3.Distance(prev, point);
                prev = point;
            }
            return length;
        }

        public static List<Vector3> GenerateCubicPoints(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int steps)
        {
            List<Vector3> result = new List<Vector3>(steps + 1);
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                result.Add(GetPointCubic(p0, p1, p2, p3, t));
            }
            return result;
        }
        public static List<Vector3> GenerateQuadraticPoints(Vector3 p0, Vector3 p1, Vector3 p2, int steps)
        {
            List<Vector3> result = new List<Vector3>(steps + 1);
            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;
                result.Add(GetPointQuadratic(p0, p1, p2, t));
            }
            return result;
        }
    }
}
