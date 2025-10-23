using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    //帧动画，性能不好，因为用得少所以没优化
    //要是以后用得多的话去做个gpu instancing和shader的实现
    public class TCP_DrawAnimationOverlay : CompProperties
    {
        public string path;

        public int frameCount;

        public float framePerSec = 1;

        public float scale = 9f;

        public Vector3 offset = new Vector3(0, 0, 0);
        public TCP_DrawAnimationOverlay()
        {
            compClass = typeof(TC_DrawAnimationOverlay);
        }
    }

    public class TC_DrawAnimationOverlay : ThingComp
    {
        TCP_DrawAnimationOverlay Props => (TCP_DrawAnimationOverlay)this.props;
        float crtFrame = 0;
        int lastTick = 0;

        List<Material> frames = null;

        void InitMaterial()
        {
            if (frames != null) return;
            frames = new();
            for(int i = 0; i < Props.frameCount; ++i)
            {
                string tempPath = Props.path + $"/{i}";
                Material mat = MaterialPool.MatFrom(tempPath, ShaderDatabase.Transparent);
                //Log.Message($"{mat == null} at {tempPath}");
                frames.Add(mat);
            }
        }

        public override void PostDraw()
        {
            InitMaterial();
            if (Find.TickManager.TicksGame - lastTick >= 1)
            {
                crtFrame += Props.framePerSec * (Find.TickManager.TicksGame - lastTick) / 60f;
                lastTick = Find.TickManager.TicksGame;
                if (crtFrame >= Props.frameCount) crtFrame = 0;
            }

            float num = Props.scale;
            var s = new Vector3(num, 1f, num);
            Vector3 vector = parent.DrawPos + Props.offset;
            float angle = 0;
            vector.y = AltitudeLayer.Blueprint.AltitudeFor();
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            Material mat = frames[(int)crtFrame];

            Graphics.DrawMesh(MeshPool.plane10, matrix, mat, 2);
            base.PostDraw();
        }
    }
}
