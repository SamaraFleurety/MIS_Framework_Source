using AK_DLL;
using AKA_Ability;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKE_OperatorExtension
{
    public class Pawn_Phrolova : Pawn
    {
        const float HEAL_PER_PULSE = 10;
        const int HEAL_PULSE_INTERVAL = 60;

        const int REPRODUCE_INTERVAL = TimeToTickDirect.hour * 12;
        protected override void Tick()
        {
            base.Tick();
            //fll自愈
            if (Find.TickManager.TicksGame % HEAL_PULSE_INTERVAL == 0)
            {
                AbilityEffect_Heal.Heal(this, HEAL_PER_PULSE);
            }

            //fll器官会再生
            if (Find.TickManager.TicksGame % REPRODUCE_INTERVAL == 0)
            {
                //原版没做缓存 好啥比
                foreach(var part in this.health.hediffSet.hediffs)
                {
                    if (part is Hediff_MissingPart missing) 
                    {
                        health.hediffSet.hediffs.Remove(missing);
                        break;
                    }
                }
            }
        }

        //纯不准kill
        public override void Kill(DamageInfo? dinfo, Hediff exactCulprit = null)
        {
            return;
        }
    }
}
