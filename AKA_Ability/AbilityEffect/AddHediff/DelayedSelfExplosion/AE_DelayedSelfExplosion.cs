using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AE_DelayedSelfExplosion : AbilityEffect_AddHediff
    {
        protected override void PreProcess()
        {
            this.hediffDef = DefDatabase<HediffDef>.GetNamed("AK_Hediff_SelfExplosion");
        }

        protected override void postAddHediff(Hediff h)
        {
            HC_SelfExplosion comp = (h as HediffWithComps).TryGetComp<HC_SelfExplosion>();
            if (comp == null)
            {
                Log.Error("因为未知原因," + h.pawn.Name + "身上的自爆hediff组件错误");
            }
            else
            {
                comp.AfterTick = this.afterCount * (int)unit;
                comp.Radius = this.radius;
                comp.Damage = this.damage;
            }
        }
        public AE_DelayedSelfExplosion()
        {
            this.onSelf = true;
            this.severity = 1f;
        }
        public int afterCount = 1;  //多少单位后爆炸
        public TimeToTick unit = TimeToTick.hour; //单位
        public float radius = 2.2f;  //爆炸半径
        public int damage = 10;      //爆炸伤害
    }
}
