using RimWorld;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKS_Shield.Extension
{
    //每秒旋转1度的护盾
    public class TCP_ExtraRenderRotate : CompProperties
    {
        public string bubbleRotateTexPath = null;
        public Vector3 bubbleRotateOffsets = Vector3.zero;
        public TCP_ExtraRenderRotate()
        {
            compClass = typeof(TC_ExtraRenderRotate);
        }
    }

    public class TC_ExtraRenderRotate : TC_ShieldExtension_Base
    {
        TCP_ExtraRenderRotate Props => props as TCP_ExtraRenderRotate;

        private Material rotateBubble = null;

        private Material RotateBubble
        {
            get
            {
                rotateBubble ??= MaterialPool.MatFrom(Props.bubbleRotateTexPath, ShaderDatabase.Transparent);
                return rotateBubble;
            }
        }
        public override void CompDrawWornExtras()
        {
            if (!CompShield.ShouldDisplay) return;
            var num = 2f;
            var vector = CompShield.Wearer.Drawer.DrawPos;
            vector.y = AltitudeLayer.Blueprint.AltitudeFor();
            vector += Props.bubbleRotateOffsets;

            //float angle = 0;

            //除以一千是一毫秒，再除以一千是1秒。每0.1秒转6度
            float angle = (long)((double)DateTime.Now.Ticks / 10000 / 1000 * 10 * 60) % 3600;
            angle /= 10;

            //Log.Message($"angle {DateTime.Now.Ticks} - {angle}");

            var s = new Vector3(num, 1f, num);
            var matrix = default(Matrix4x4);

            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, RotateBubble, 1);
        }
    }
}