﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL.Bezier
{
    //一个用于生成贝塞尔曲线的属性类
    //属性类中，存储的数值是以一条从左到右的水平的长度为1的标准向量为基准。在生成具体曲线实例时会根据需要进行缩放
    public class BezierCurveProperty
    {
        //起点固定在(0,0,0)，终点为(1,0,0)

        //2个控制点都可以在一个方形里面随机
        public Vector3 control1LeftBottom = new Vector3(0.33f, 0f, 0.33f);
        public Vector3 control1RightTop = new Vector3(0.4f, 0f, 0.66f);

        public Vector3 control2LeftBottom = new Vector3(0.33f, 0f, 0.33f);
        public Vector3 control2RightTop = new Vector3(0.66f, 0f, 0.66f);

        //弹道垂直翻转的概率，使其子弹轨迹会类似dna螺旋
        public float verticalFlipChance = 0.5f;

        //生成一个确定的曲线
        public BezierCurveCubic GenCurveCubic(Vector3 start, Vector3 destination)
        {
            return new BezierCurveCubic(this, start, destination);
        }
    }

    //一个已经确定的3阶贝塞尔曲线实例
    public class BezierCurveCubic : IExposable
    {
        public Vector3 start;
        public Vector3 control1;
        public Vector3 control2;
        public Vector3 end;

        //仅存读档用，不准直接用这个去new
        public BezierCurveCubic()
        {
        }

        public BezierCurveCubic(BezierCurveProperty prop, Vector3 start, Vector3 end)
        {
            this.start = start;
            this.end = end;

            float verticalFlip = Rand.Chance(prop.verticalFlipChance) ? -1 : 1;

            Vector3 p1 = GenRandomControlPoint(prop.control1LeftBottom, prop.control1RightTop);
            control1 = GetTransformedPoint(p1);

            Vector3 p2 = GenRandomControlPoint(prop.control2LeftBottom, prop.control2RightTop);
            control2 = GetTransformedPoint(p2);

            Vector3 GenRandomControlPoint(Vector3 leftBottom, Vector3 rightTop)
            {
                return new Vector3
                    (
                        Rand.Range(leftBottom.x, rightTop.x),
                        0,
                        Rand.Range(leftBottom.z, rightTop.z)
                    );
            }

            Vector3 GetTransformedPoint(Vector3 p)
            {
                return new Vector3
                    (
                        start.x + (end.x - start.x) * p.x - (end.y - start.y) * p.y,
                        0,
                        start.z + (end.z - start.y) * p.x + (end.x - start.x) * p.y * verticalFlip
                    );
            }
        }

        public Vector3 GetPoint (float t)
        {
            return BezierUtil.GetPointCubic(start, control1, control2, end, t);
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref start, "start");
            Scribe_Values.Look(ref control1, "control1");
            Scribe_Values.Look(ref control2, "control2");
            Scribe_Values.Look(ref end, "end");
        }
    }
}
