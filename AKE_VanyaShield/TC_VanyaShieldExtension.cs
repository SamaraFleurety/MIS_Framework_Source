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
        public GraphicData bubbleStaticOverlay = null;
        [Obsolete]
        public string bubbleStaticTexPath = null;
        public string bubbleRotateTexPath = null;

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
        protected bool shouldDrawNow = false;

        public bool HideVanillaBubble => Props.hideVanillaBubble;
        public float ReflectionRatio => Props.reflectionRatio;

        bool bubbleRefreshed = false;
        [Obsolete]
        Material staticBubble = null;
        Material rotateBubble = null;
        int time = 0;

        //private Graphic graphic;

        private Graphic StaticBubbleGraphic => Props.bubbleStaticOverlay.GraphicColoredFor(Parent);
        public override void CompDrawWornExtras()
        {
            shouldDrawNow = false;
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

            var num = 2f;
            var vector = shield.Wearer.Drawer.DrawPos;
            vector.y = AltitudeLayer.Blueprint.AltitudeFor();

            float angle = 0;
            var s = new Vector3(num, 1f, num);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            if (!bubbleRefreshed) RefreshBubbleMaterial();
            //静态护盾
            Vector3 loc = Wearer.DrawPos;
            if (Wearer.Rotation != Rot4.South) loc.y += 5f;         //显示在人物上面
            else loc.y -= 5f;                                       //只有朝南是显示在人物下面
            if (Props.bubbleStaticOverlay != null && StaticBubbleGraphic != null && Wearer != null) StaticBubbleGraphic.Draw(loc, Wearer.Rotation, Parent);
            if (staticBubble != null) Graphics.DrawMesh(MeshPool.plane10, matrix, staticBubble, 2); //过时静态护盾 记得删
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
            if (Props.bubbleStaticTexPath != null) staticBubble = MaterialPool.MatFrom(Props.bubbleStaticTexPath, ShaderDatabase.Transparent);
            if (Props.bubbleRotateTexPath != null) rotateBubble = MaterialPool.MatFrom(Props.bubbleRotateTexPath, ShaderDatabase.Transparent);
        }

        
    }
}
