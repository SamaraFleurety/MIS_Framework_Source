using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using AK_DLL;

namespace AKE_OperatorExtension
{
    public class Bullet_SakiChan : Bullet
    {
        //这是untiy原版组件，时间强行和unity时间绑定，但是泰南有自己的想法。这里尊重泰南的想法
        public static bool allowChangeTime = false;
        public static ConditionalWeakTable<TrailRenderer, object> cacedTrailRender = new();

        //每tick视为过去多少现实时间
        const float REAL_TIME_PER_TICK = 0.01667f;

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
        int tickAfterSpawned = 0;
        public override Vector3 DrawPos
        {
            get
            {
                Vector3 direction = (destination - origin).normalized;
                //入参是弧度
                float pendulum = Mathf.Sin(tickAfterSpawned * Mathf.Deg2Rad * FREQUENCY) * 1;
                
                Vector3 perpendicular = new Vector3(-direction.y, 0, direction.x);
                return base.DrawPos + perpendicular * pendulum;
            }
        }

        protected override void Tick()
        {
            if (!Spawned || Destroyed) return;
            ++tickAfterSpawned;
            //return;
            allowChangeTime = true;
            UComp_Trail.time += REAL_TIME_PER_TICK;
            Mygo_Bullet.transform.position = this.DrawPos;
            allowChangeTime = false;
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            base.Destroy(mode);
            GameObject.Destroy(mygo_BulletWithTrail);
            
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref tickAfterSpawned, "tickAfterSpawned");
        }
    }
}
