using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKE_VanyaShield
{
    public class TCP_ShieldExtraRenderer : CompProperties
    {
        public GraphicData bubbleStaticOverlay = null;          //静态护盾贴图
        public TCP_ShieldExtraRenderer()
        {
            compClass = typeof(TC_ShieldExtraRenderer);
        }
    }

    public class TC_ShieldExtraRenderer : ThingComp
    {
        TCP_ShieldExtraRenderer Props => props as TCP_ShieldExtraRenderer;

        TC_VanyaShieldExtension compExt = null;

        TC_VanyaShieldExtension CompExt
        {
            get
            {
                if (compExt == null) compExt = parent.TryGetComp<TC_VanyaShieldExtension>();
                return compExt;
            }
        }

        public override void CompDrawWornExtras()
        {
            //不该发生
            if (CompExt == null || !(parent is Apparel apparel))
            {
                Log.Error($"[MIS] 护盾额外渲染必须和普通扩展comp一起使用");
                parent.AllComps.Remove(this);
                return;
            }
            if (!CompExt.shouldDrawNow) return;

            TC_VanyaShieldExtension.DrawStaticOverlay(Props.bubbleStaticOverlay, apparel.Wearer);
        }
    }

}
