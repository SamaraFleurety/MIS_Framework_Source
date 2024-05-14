using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using VanyaMod;
using System.Reflection;
using RimWorld;
using UnityEngine;

namespace AKE_VanyaShield
{
    public class TCP_VanyaShieldExtension : CompProperties
    {
        public bool hideVanillaBubble = false;

        public GraphicData bubbleStaticOverlay = null;          //静态护盾贴图
        //public Dictionary<Rot4, Vector3> bubbleStaticOverlayOffsets = new Dictionary<Rot4, Vector3>();

        //[Obsolete]
       //public string bubbleStaticTexPath = null;
        public string bubbleRotateTexPath = null;
        public Vector3 bubbleRotateOffsets = Vector3.zero;

        public float reflectionRatio = -1;

        public TCP_VanyaShieldExtension()
        {
            compClass = typeof(TC_VanyaShieldExtension);
        }
    }

    public class TC_VanyaShieldExtension : ThingComp
    {
        TCP_VanyaShieldExtension Props => (TCP_VanyaShieldExtension)props;
        protected Vanya_ShieldBelt Parent => (Vanya_ShieldBelt)parent;
        protected Pawn Wearer => Parent.Wearer;

        public bool HideVanillaBubble => Props.hideVanillaBubble;
        public float ReflectionRatio => Props.reflectionRatio;

        bool bubbleRefreshed = false;

        Material rotateBubble = null;
        int time = 0;

        //private Graphic graphic;
        private Graphic StaticBubbleGraphic => Props.bubbleStaticOverlay.GraphicColoredFor(Parent);

        //共享的渲染信息
        public bool shouldDrawNow = false;
        public override void CompDrawWornExtras()
        {
            shouldDrawNow = false;      //别删 别的comp扩展要用
            if (ModLister.GetActiveModWithIdentifier("Mlie.VanyaShield") == null) return;
            if (Wearer == null) return;

            //有能量而且征召时才显示气泡
            Vanya_ShieldBelt shield = Parent;

            PropertyInfo info = shield.GetType().GetProperty("ShieldState", BindingFlags.NonPublic | BindingFlags.Instance);
            ShieldState shieldState = (ShieldState)info.GetValue(shield);
            info = shield.GetType().GetProperty("ShouldDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
            bool shouldDisplay = (bool)info.GetValue(shield);

            if (shieldState != ShieldState.Active || !shouldDisplay) return;
            shouldDrawNow = true;

            //静态护盾
            /*Rot4 rot = Wearer.Rotation;
            if (Props.bubbleStaticOverlay.graphicClass == typeof(Graphic_Single)) rot = Rot4.North;     //单图护盾不该旋转

            Vector3 loc = Wearer.DrawPos;
            if (Props.bubbleStaticOverlay.graphicClass == typeof(Graphic_Single))           //单层贴图默认+1
            {
                loc.y += 1f;
            }
            else if (Props.bubbleStaticOverlay.graphicClass == typeof(Graphic_Multi))       //多向静态贴图，应用比如碧蓝的舰装
            {
                if (Wearer.Rotation != Rot4.South) loc.y += 1f;         //显示在人物上面
                else loc.y -= 1f;                                       //只有朝南是显示在人物下面
            }

            loc += Props.bubbleStaticOverlay.DrawOffsetForRot(rot);
            /*if (Props.bubbleStaticOverlayOffsets.ContainsKey(rot))
            {
                loc += Props.bubbleStaticOverlayOffsets[rot];
            }*//*

            if (Props.bubbleStaticOverlay != null && StaticBubbleGraphic != null && Wearer != null) StaticBubbleGraphic.Draw(loc, rot, Parent);*/

            DrawStaticOverlay(Props.bubbleStaticOverlay, Wearer);

            //旋转护盾
            var num = 2f;
            var vector = shield.Wearer.Drawer.DrawPos;
            vector.y = AltitudeLayer.Blueprint.AltitudeFor();
            vector += Props.bubbleRotateOffsets;

            float angle = 0;
            var s = new Vector3(num, 1f, num);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            if (!bubbleRefreshed) RefreshBubbleMaterial();
            //静态护盾
            //if (staticBubble != null) Graphics.DrawMesh(MeshPool.plane10, matrix, staticBubble, 2); //过时静态护盾 记得删
            //帧动画护盾
            if (rotateBubble != null)
            {
                time = (time + 1) % 360;
                angle = time;
                matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, rotateBubble, 1);
            }
        }

        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            //Log.Message($"受到伤害{dinfo.Amount}:{totalDamageDealt}");
        }
        /*public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
        }*/

        private void RefreshBubbleMaterial()
        {
            //if (Props.bubbleStaticTexPath != null) staticBubble = MaterialPool.MatFrom(Props.bubbleStaticTexPath, ShaderDatabase.Transparent);
            if (Props.bubbleRotateTexPath != null) rotateBubble = MaterialPool.MatFrom(Props.bubbleRotateTexPath, ShaderDatabase.Transparent);
        }

        public static void DrawStaticOverlay(GraphicData graphicData, Pawn wearer)
        {
            if (graphicData == null || wearer == null) return;
            Graphic graphic = graphicData.GraphicColoredFor(wearer);
            if (graphic == null) return;

            Rot4 rot = wearer.Rotation;
            if (graphicData.graphicClass == typeof(Graphic_Single)) rot = Rot4.North;     //单图护盾不该旋转

            Vector3 loc = wearer.DrawPos;
            if (graphicData.graphicClass == typeof(Graphic_Single))           //单层贴图默认+1
            {
                loc.y += 1f;
            }
            else if (graphicData.graphicClass == typeof(Graphic_Multi))       //多向静态贴图，应用比如碧蓝的舰装
            {
                if (wearer.Rotation != Rot4.South) loc.y += 1f;         //显示在人物上面
                else loc.y -= 1f;                                       //只有朝南是显示在人物下面
            }

            loc += graphicData.DrawOffsetForRot(rot);

            graphic.Draw(loc, rot, wearer);
        }
    }
}
