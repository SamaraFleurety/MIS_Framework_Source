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
        //public static bool allowChangeTime = false;
        //public static ConditionalWeakTable<TrailRenderer, object> cacedTrailRender = new();

        //每tick视为过去多少现实时间
        //const float REAL_TIME_PER_TICK = 0.01667f;

        #region 贝塞尔弹道相关
        #region ----啥比向量计算

        //当前所在片段
        int currentSegment = 0;

        //年龄
        float tickAfterSpawned;
        //当前曲线段内的年龄
        int tickAfterSpawned_CurrentCurve = 0;
        //生命周期tick总数
        float lifeTick;

        //一开始在0段, 也就是取起点和下一个片段点
        int ShouldInSegemnt
        {
            get
            {
                return (int)((float)tickAfterSpawned / lifeTick * BezierSegmentCount);
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

        BezierCurveCubic bezierCurve = null;

        //镜像翻转贝塞尔函数的全局值。不可以一段翻转一段不翻，不好看
        int verticalFlip; 
        void RecalculateCurve()
        {
            int seg = ShouldInSegemnt;
            if (seg == currentSegment && bezierCurve != null) return;
            bezierCurve = Ext_Bezier.curveProperty.GenCurveCubic(GetXthPoint(seg), GetXthPoint(seg + 1), flipOverride: verticalFlip);
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
        /*TrailRenderer ucomp_Trail = null;
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
        }*/


        public override Vector3 DrawPos
        {
            get
            {
                RecalculateCurve();
                float t = Mathf.Clamp01(tickAfterSpawned_CurrentCurve / (StartingTicksToImpact / (float)BezierSegmentCount));
                Vector3 bezierPoint = bezierCurve.GetPoint(t);
                return new Vector3(bezierPoint.x, base.DrawPos.y, bezierPoint.z);
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
            ++tickAfterSpawned_CurrentCurve;
            ++tickAfterSpawned;
            Mygo_Bullet.transform.position = this.DrawPos;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            GameObject.Destroy(mygo_BulletWithTrail);

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tickAfterSpawned_CurrentCurve, "tickAfterSpawned_CC");
            Scribe_Values.Look(ref tickAfterSpawned, "tickAfterSpawned", 0);
            Scribe_Values.Look(ref currentSegment, "currentSegment");
            Scribe_Deep.Look(ref bezierCurve, "bezierCurve");
            Scribe_Values.Look(ref lifeTick, "life");
            Scribe_Values.Look(ref verticalFlip, "flip", 1);
        }
        #endregion
    }
}
