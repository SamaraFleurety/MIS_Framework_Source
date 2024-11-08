using AKA_Ability.AbilityEffect;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AKA_Ability
{
    public class AKAbility_Summon : AKAbility_Targetor
    {
        public HashSet<Thing> summoneds = new();

        private AE_SummonBase effector = null;

        //需要知道第一个aes来确定召唤物上限，来决定禁用gizmo与否
        public AE_SummonBase Effector
        {
            get
            {
                if (effector == null)
                {
                    foreach (AbilityEffectBase ae in def.compEffectList)
                    {
                        if (ae is AE_SummonBase aes)
                        {
                            effector = aes;
                            break;
                        }
                    }
                }
                if (effector == null)
                {
                    Log.Error($"[AKA]错误：{def.label}技能是召唤技能但是没有召唤效果。");
                }
                return effector;
            }
        }

        public AKAbility_Summon(AbilityTracker tracker) : base(tracker)
        {
        }
        public AKAbility_Summon(AKAbilityDef def, AbilityTracker tracker) : base(def, tracker)
        {
        }

        public void Notify_SummonedSpawned(Thing summoned)
        {
            summoneds.Add(summoned);
        }

        public void Notify_SummonedVanished(Thing summoned)
        {
            if (summoneds.Contains(summoned)) { summoneds.Remove(summoned); }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref summoneds, "summons", LookMode.Reference);
        }
    }
}
