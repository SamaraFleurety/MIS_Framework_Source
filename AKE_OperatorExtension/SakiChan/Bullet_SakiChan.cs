using AK_DLL;
using AK_DLL.Bezier;
using RimWorld;
using System.Collections.Generic;
using System.Collections.Generic.RedBlack;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace AKE_OperatorExtension
{

    public class Bullet_SakiChan : Bullet
    {
        //每tick视为过去多少现实时间
        //const float REAL_TIME_PER_TICK = 0.01667f;

        #region 贝塞尔弹道相关
        #region ----啥比向量计算

        //当前所在片段
        //int currentSegment = 0;

        //年龄
        float tickAfterSpawned;
        //当前曲线段内的年龄
        //int tickAfterSpawned_CurrentCurve = 0;
        //生命周期tick总数
        float lifeTick;

        int shouldInSegemnt = 0;
        //一开始在0段, 也就是取起点和下一个片段点
        int ShouldInSegemnt
        {
            get
            {
                return shouldInSegemnt;
                //return (int)((float)tickAfterSpawned / lifeTick * BezierSegmentCount);
            }
        }

        //此弹道被切分为segement段后，获取第x个点。显然一共有segment+1个点
        Vector3 GetXthPoint(int x)
        {
            float seg = BezierSegmentCount;
            return new Vector3
                (
                    origin.x + (x / seg * (destination.x - origin.x)),
                    0,
                    origin.z + (x / seg * (destination.z - origin.z))
                );
        }
        #endregion 向量计算

        Ext_MultiSegBezierBullet ext_Bezier = null;
        Ext_MultiSegBezierBullet Ext_Bezier
        {
            get
            {
                ext_Bezier ??= this.def.GetModExtension<Ext_MultiSegBezierBullet>();
                return ext_Bezier;
            }
        }
        //一共分成多少段，最小是1
        int BezierSegmentCount
        {
            get
            {
                int segment = (int)((destination - origin).magnitude / Ext_Bezier.segmentSliceCellCount);
                if (segment <= 0) segment = 1;
                return segment;
            }
        }

        //BCC_SpeedAdjustable bezierCurve = null;

        //List<BCC_SpeedAdjustable> cachedAllCurves = new();

        RedBlackTree<float, float2> cachedAllPts = null;

        float curveArcLength;
        RedBlackTree<float, float2> CachedAllPts
        {
            get
            {
                if (cachedAllPts == null)
                {
                    List<BCC_SpeedAdjustable> curves = new();
                    for (int i = 0; i < BezierSegmentCount; ++i)
                    {
                        curves.Add(Ext_Bezier.curveProperty.GenCurveCubic(GetXthPoint(i), GetXthPoint(i + 1), flipOverride: verticalFlip) as BCC_SpeedAdjustable);
                    }

                    cachedAllPts = new();

                    float distanceTravelled = 0;
                    Vector3 lastPt = origin;
                    for (int i = 0; i < BezierSegmentCount; ++i)
                    {
                        foreach (Vector3 pt in curves[i].GetAllPoints())
                        {
                            float distance = (pt - lastPt).magnitude;
                            distanceTravelled += distance;
                            cachedAllPts.Add(distanceTravelled, new float2 { x = pt.x, z = pt.z });
                            lastPt = pt;
                        }
                    }

                    curveArcLength = distanceTravelled;
                }
                return cachedAllPts;
            }
        }

        //镜像翻转贝塞尔函数的全局值。不可以一段翻转一段不翻，不好看
        int verticalFlip;
        /*void RecalculateCurve()
        {
            int seg = ShouldInSegemnt;
            if (seg == currentSegment && bezierCurve != null) return;
            bezierCurve = Ext_Bezier.curveProperty.GenCurveCubic(GetXthPoint(seg), GetXthPoint(seg + 1), flipOverride: verticalFlip) as BCC_SpeedAdjustable;
            currentSegment = seg;
            tickAfterSpawned_CurrentCurve = 0;
        }*/

        GameObject mygo_BulletWithTrail = null;

        GameObject Mygo_Bullet
        {
            get
            {
                if (mygo_BulletWithTrail == null)
                {
                    GameObject prefab = AKEDefOf.AK_Prefab_BulletTrailSaki.LoadPrefab();
                    mygo_BulletWithTrail = GameObject.Instantiate(prefab);
                    mygo_BulletWithTrail.transform.position = this.DrawPos;
                }

                return mygo_BulletWithTrail;
            }
        }

        public float Speed
        {
            get
            {
                float speed = (float)(-0.8 * (x() - 0.45) * (x() - 0.45) + 0.5);
                //Log.Message($"speed: {distanceTravelled} / {curveArcLength} = x: {x()}, res: {speed}");
                return speed;

                float x()
                {
                    return distanceTravelled / curveArcLength;
                }
            }
        }
        float calculatedTick = 0;
        Vector3 calculatedPos;
        float distanceTravelled = 0;
        public override Vector3 DrawPos
        {
            get
            {
                if (tickAfterSpawned != calculatedTick)
                {
                    RedBlackNode<float, float2> node = CachedAllPts.Search_RetNode(Speed + distanceTravelled, 3);
                    if (node == null) //不太可能触发
                    {
                        calculatedPos = origin;
                    }
                    else
                    {
                        calculatedPos = new Vector3(node.Data.x, base.DrawPos.y, node.Data.z);
                    }
                    calculatedTick = tickAfterSpawned;
                    distanceTravelled += Speed;
                }

                return calculatedPos;
                /*if (shouldInSegemnt >= BezierSegmentCount)
                {
                    return destination;
                }
                float spd = Speed;
                float overflowDistance;
                Vector3 bezierPt = origin;
                while (spd > 0)
                {
                    RecalculateCurve();
                    bezierPt = bezierCurve.GetPoint(spd, out overflowDistance);
                    spd = overflowDistance;
                    if (overflowDistance > 0)
                    {
                        spd = overflowDistance;
                        ++shouldInSegemnt;

                        //到达终点
                        if (shouldInSegemnt >= BezierSegmentCount)
                        {
                            calculatedPos = destination;
                            return destination;
                        }
                    }
                }
                calculatedPos = bezierPt;
                return calculatedPos;*/

                /*RecalculateCurve();
                float t = Mathf.Clamp01(tickAfterSpawned_CurrentCurve / (StartingTicksToImpact / (float)BezierSegmentCount));
                //Vector3 bezierPoint = bezierCurve.GetPoint(t);
                return new Vector3(bezierPoint.x, base.DrawPos.y, bezierPoint.z);*/
            }
        }

        #endregion 贝塞尔弹道

        #region 原版函数

        public override void Launch(Thing launcher, Vector3 origin, LocalTargetInfo usedTarget, LocalTargetInfo intendedTarget, ProjectileHitFlags hitFlags, bool preventFriendlyFire = false, Thing equipment = null, ThingDef targetCoverDef = null)
        {
            base.Launch(launcher, origin, usedTarget, intendedTarget, hitFlags, preventFriendlyFire, equipment, targetCoverDef);
            lifeTick = lifetime;
            verticalFlip = Rand.Chance(Ext_Bezier.curveProperty.verticalFlipChance) ? 1 : -1;
        }
        protected override void Tick()
        {
            if (!Spawned || Destroyed) return;
            //++tickAfterSpawned_CurrentCurve;
            ++tickAfterSpawned;
            Mygo_Bullet.transform.position = this.DrawPos;
        }

        protected override void TickInterval(int delta)
        {
            base.TickInterval(delta);
            if (distanceTravelled < curveArcLength) ticksToImpact += delta;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            GameObject.Destroy(mygo_BulletWithTrail);

        }

        public override void ExposeData()
        {
            base.ExposeData();
            //Scribe_Values.Look(ref tickAfterSpawned_CurrentCurve, "tickAfterSpawned_CC");
            Scribe_Values.Look(ref tickAfterSpawned, "tickAfterSpawned", 0);
            //Scribe_Values.Look(ref currentSegment, "currentSegment");
            //Scribe_Deep.Look(ref bezierCurve, "bezierCurve");
            Scribe_Values.Look(ref lifeTick, "life");
            Scribe_Values.Look(ref verticalFlip, "flip", 1);
            Scribe_Values.Look(ref shouldInSegemnt, "shouldInSegment", 0);
        }
        #endregion
    }

    //首字母小写是hlsl那边的标准
    public class float2
    {
        public float x;
        public float z;
    }
}
