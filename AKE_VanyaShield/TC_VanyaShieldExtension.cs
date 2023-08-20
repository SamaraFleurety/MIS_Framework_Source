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
        public string bubbleStaticTexPath = null;
        public string bubbleRotateTexPath = null;

        public float reflectionRatio = 0;

        public TCP_VanyaShieldExtension()
        {
            compClass = typeof(TC_VanyaShieldExtension);
        }
    }

    public class TC_VanyaShieldExtension : ThingComp
    {
        public bool HideVanillaBubble => Props.hideVanillaBubble;

        TCP_VanyaShieldExtension Props => (TCP_VanyaShieldExtension)props;

        bool bubbleRefreshed = false;
        Material staticBubble = null;
        Material rotateBubble = null;
        int time = 0;

        public override void CompDrawWornExtras()
        {
            if (ModLister.GetActiveModWithIdentifier("Mlie.VanyaShield") == null) return;
            Vanya_ShieldBelt shield = parent as Vanya_ShieldBelt;

            PropertyInfo info = shield.GetType().GetProperty("ShieldState", BindingFlags.NonPublic | BindingFlags.Instance);
            ShieldState shieldState = (ShieldState)info.GetValue(shield);
            info = shield.GetType().GetProperty("ShouldDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
            bool shouldDisplay = (bool)info.GetValue(shield);

            if (shieldState != ShieldState.Active || !shouldDisplay) return;

            var num = 2f;
            var vector = shield.Wearer.Drawer.DrawPos;
            vector.y = AltitudeLayer.Blueprint.AltitudeFor();

            float angle = 0;
            var s = new Vector3(num, 1f, num);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(vector, Quaternion.AngleAxis(angle, Vector3.up), s);
            if (!bubbleRefreshed) RefreshBubbleMaterial();
            if (staticBubble != null) Graphics.DrawMesh(MeshPool.plane10, matrix, staticBubble, 2);
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
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            Log.Message($"受到伤害{dinfo.Amount}:{totalDamageDealt}");
        }
        /*public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
        }*/

        void RefreshBubbleMaterial()
        {
            if (Props.bubbleStaticTexPath != null) staticBubble = MaterialPool.MatFrom(Props.bubbleStaticTexPath, ShaderDatabase.Transparent);
            if (Props.bubbleRotateTexPath != null) rotateBubble = MaterialPool.MatFrom(Props.bubbleRotateTexPath, ShaderDatabase.Transparent);
        }
    }
}
