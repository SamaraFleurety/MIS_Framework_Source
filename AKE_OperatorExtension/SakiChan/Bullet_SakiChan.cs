using AK_DLL;
using AK_DLL.Bezier;
using RimWorld;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace AKE_OperatorExtension
{
    public class Bullet_SakiChan : Bullet
    {
        //这是untiy原版组件，时间强行和unity时间绑定，但是泰南有自己的想法。这里尊重泰南的想法
        public static bool allowChangeTime = false;
        public static ConditionalWeakTable<TrailRenderer, object> cacedTrailRender = new();

        //每tick视为过去多少现实时间
        const float REAL_TIME_PER_TICK = 0.01667f;

        #region 贝塞尔弹道相关
        Ext_MultiSegBezierBullet ext_Bezier = null;
        Ext_MultiSegBezierBullet Ext_Bezier
        {
            get
            {
                ext_Bezier ??= this.def.GetModExtension<Ext_MultiSegBezierBullet>();
                return ext_Bezier;
            }
        }
        int BezierSegmentCount
        {
            get
            {
                return (int)((destination - origin).magnitude / ext_Bezier.segmentSliceCellCount);
            }
        }

        int tickAfterSpawned_CurrentCurve = 0;

        #region ----啥比向量计算
        int currentSegment = 0;

        //生命周期tick总数
        float LifeTick => ticksToImpact - StartingTicksToImpact;

        int ShouldInSegemnt
        {
            get
            {
                return (int)(LifeTick / BezierSegmentCount);
            }
        }

        //此弹道被切分为segement段后，获取第x个点。显然一共有segment+1个点
        Vector3 GetXthPoint(int x)
        {
            float seg = BezierSegmentCount;
            return new Vector3
                (
                    origin.y + (x / seg * (destination.x - origin.x)),
                    0,
                    origin.y + (x / seg * (destination.y - origin.y))
                );
        }
        #endregion 向量计算

        BezierCurveCubic bezierCurve = null;

        void RecalculateCurve()
        {
            int seg = ShouldInSegemnt;
            if (seg == currentSegment) return;
            bezierCurve = Ext_Bezier.curveProperty.GenCurveCubic(GetXthPoint(seg - 1), GetXthPoint(seg));
            currentSegment = seg;
            tickAfterSpawned_CurrentCurve = 0;
        }

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

        //unity component
        TrailRenderer ucomp_Trail = null;
        TrailRenderer UComp_Trail
        {
            get
            {
                if (ucomp_Trail == null)
                {
                    ucomp_Trail = Mygo_Bullet.GetComponent<TrailRenderer>();
                    cacedTrailRender.Add(ucomp_Trail, null);
                    ucomp_Trail.time = int.MaxValue;
                }
                return ucomp_Trail;
            }
        }

        const float FREQUENCY = 12;


        public override Vector3 DrawPos
        {
            get
            {
                /*Vector3 direction = (destination - origin).normalized;
                //入参是弧度
                float pendulum = Mathf.Sin(tickAfterSpawned * Mathf.Deg2Rad * FREQUENCY) * 1;

                Vector3 perpendicular = new(-direction.y, 0, direction.x);
                return base.DrawPos + (perpendicular * pendulum);*/
                RecalculateCurve();
                float t = Mathf.Clamp01(tickAfterSpawned_CurrentCurve / (StartingTicksToImpact / BezierSegmentCount));
                return bezierCurve.GetPoint(t);
            }
        }

        #endregion 贝塞尔弹道

        protected override void Tick()
        {
            if (!Spawned || Destroyed) return;
            ++tickAfterSpawned_CurrentCurve;
            //return;
            //allowChangeTime = true;
            //UComp_Trail.time += REAL_TIME_PER_TICK;
            Mygo_Bullet.transform.position = this.DrawPos;
            //allowChangeTime = false;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            GameObject.Destroy(mygo_BulletWithTrail);

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tickAfterSpawned_CurrentCurve, "tickAfterSpawned");
            Scribe_Values.Look(ref currentSegment, "currentSegment");
            Scribe_Deep.Look(ref bezierCurve, "bezierCurve");
        }
    }
}
