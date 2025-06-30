using AK_DLL;
using System;
using Verse;

namespace AKA_Ability.Cooldown
{
    public class CooldownProperty
    {
        public Type cooldownClass;

        public int maxCharge = 1;

        //招募时初始自带sp;
        public int initSP = 0;

        public int SPPerCharge = 1;
        public TimeToTick SPUnit = TimeToTick.rSecond;

        //回复速度 默认充能速度是 CDPerCharge * CDUnit / rate 每充能
        public int rate = 1;

        public CooldownProperty()
        {
            this.cooldownClass = typeof(Cooldown_Regen);
        }
    }

    //就是舟的sp
    //谁tm写的cd
    //继承可能有点问题 没写基类 但是自回+1其实也挺基础的，随便吧
    public class Cooldown_Regen : IExposable
    {
        public AKAbility_Base parent;
        public CooldownProperty prop;

        public int skillPoint;
        public virtual int SP   //当前sp 就是舟那种sp 越高越趋于增加充能
        {
            get => skillPoint;
            set => skillPoint = value;
        }
        //自回速率
        public virtual int Rate => prop.rate;

        public int charge;
        public virtual int Charge
        {
            get => charge;
            set => charge = value;
        }
        public virtual int MaxCharge => prop.maxCharge;
        public virtual int MaxSP => prop.SPPerCharge * (int)prop.SPUnit;
        public virtual bool NeedsReload => Charge != MaxCharge;

        //仅用于给子类继承，直接用这个new会报错
        public Cooldown_Regen()
        {
        }
        public Cooldown_Regen(CooldownProperty property, AKAbility_Base ability)
        {
            parent = ability;
            prop = property;
            charge = 0;
            SP = prop.initSP;
            Tick(0);
        }

        public virtual void ExposeData()
        {
            Scribe_Values.Look(ref skillPoint, "CD");
            Scribe_Values.Look(ref charge, "charge");
            BackCompatibility.PostExposeData(this);
        }

        //在tick里面只动sp 在下面的刷新充能去判断是否应该增加充能
        public virtual void Tick(int amt)
        {
            if (Charge >= MaxCharge) return;
            SP += amt * Rate;
            RefreshChargeAndSP();
        }

        //判断是否应该增加充能 -- 可能在tick 攻击 受击等多个情况需要调用
        public virtual void RefreshChargeAndSP()
        {
            while (SP > MaxSP && charge < MaxCharge)
            {
                SP -= MaxSP;
                ++Charge;
            }
            if (charge >= MaxCharge) SP = 0;
        }

        //当前充能已经冷却了多少 100%就是冷却好了可以加1充能
        //同时决定gizmo的白框。冷却越多白框填充越多
        public virtual float CooldownPercent()
        {
            if (charge == MaxCharge) return 0;
            return (float)this.SP / (float)this.MaxSP;
        }

        public virtual void CostCharge(int cost)
        {
            if (cost == 0) return;
            if (charge == MaxCharge) SP = 0;    //之前是满充能 需要刷新cd。否则维持之前充能进度
            Charge -= cost;
            Charge = Math.Max(Charge, 0);
        }

        public override string ToString()
        {
            return "SP" + this.SP + "，MaxSP" + this.MaxSP + "，MaxCharge" + this.MaxCharge + "，Charge" + this.Charge;
        }

        //给攻击回复和受击回复sp留的。设计是仅注册有必要的技能而不是每次受伤都遍历所有技能
        //cd里面才决定是否应该注册
        public virtual void SpawnSetup() { }

        public virtual void PostDespawn() { }

        public virtual void Notify_PawnStricken(ref DamageInfo dinfo) { }
        
        public virtual void Notify_PawnHitTarget(ref DamageInfo dinfo) { }
    }
}