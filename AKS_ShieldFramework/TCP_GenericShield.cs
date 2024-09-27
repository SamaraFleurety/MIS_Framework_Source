using AKS_Shield.Effector;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AKS_Shield
{
    public class TCP_GenericShield : CompProperties
    {
        //基础数据
        public float energyMax = 100;
        public float energyRegenRate = 0.1f;
        public float energyLostPerDmg = 0.033f;
        public bool allowEnergyOverflow = false;   //如果能量超过上限 是否重置为上限。为true时超过上限仍不会无限自动回复

        //破盾再启动
        public float energyRatioOnReset = 0.2f;  //百分制，0-1
        public int ticksToReset = 2400;

        //吸收伤害
        public bool canAbsorbMeleeDmg = true;

        public float meleeAbsorbFactor = 1;
        public float rangedAbsorbFactor = 1;

        public float meleeDodgeChanceFactor = 1;   //会乘以pawn的近战闪避  我靠，好像dinfo不能区分近战远程伤害
        //public float rangedDodgeChance = 0;        //0-1，躲避子弹的概率。原版并不能演黑客帝国。

        public List<Type> postBreakEffects = new List<Type>();              //仅当命中并且本次命中导致护盾破碎时调用
        public List<Type> postAbsorbDmgEffects = new List<Type>();          //命中后调用 是否闪避是参数

        public TCP_GenericShield()
        {
            compClass = typeof(TC_GenericShield);
        }
    }

    public class TC_GenericShield : ThingComp
    {
        private readonly SoundDef EnergyShield_Broken = SoundDef.Named("EnergyShield_Broken");
        public TCP_GenericShield Props => props as TCP_GenericShield;

        public float energy = 0;

        protected int ticksReset = 0;

        public Apparel Parent => parent as Apparel;
        public virtual Pawn Wearer => Parent.Wearer;

        public virtual bool ShouldDisplay
        {
            get
            {
                Pawn wearer = Wearer;
                return wearer != null && wearer.Spawned && !wearer.Destroyed && !wearer.Dead && !wearer.Downed && energy > 0 && wearer.Drafted;
            }
        }


        private Vector3 impactAngleVect;

        public float EnergyPercent => energy / Props.energyMax;


        #region 原版
        public override void CompTick()
        {
            Tick(1);
        }
        public void Tick(int amt)
        {
            if (energy > 0 && energy < Props.energyMax)
            {
                energy += Props.energyRegenRate * amt;
                if (!Props.allowEnergyOverflow)
                {
                    energy = Math.Min(energy, Props.energyMax);
                }
            }
            else
            {
                ticksReset += amt;
                if (ticksReset >= Props.ticksToReset)
                {
                    Reset();
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref energy, "en");
            Scribe_Values.Look(ref ticksReset, "tick", 0);
        }

        public override void PostPreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            Wearer.Drawer.renderer.renderTree.SetDirty();
            if (energy <= 0)
            {
                absorbed = false;
                return;
            }
            if (dinfo.Instigator == Wearer || dinfo.Def == DamageDefOf.Extinguish)
            {
                absorbed = true;
                return;
            }
            if (dinfo.Def == DamageDefOf.SurgicalCut)
            {
                absorbed = false;
                return;
            }

            if (Props.meleeDodgeChanceFactor > 0 && Wearer != null && Rand.Chance(Wearer.GetStatValue(StatDefOf.MeleeDodgeChance) * Props.meleeDodgeChanceFactor))
            {
                MoteMaker.ThrowText(Wearer.DrawPos, Wearer.Map, "TextMote_Dodge".Translate(), 1.9f);
                absorbed = true;
                foreach (Type effector in Props.postAbsorbDmgEffects)
                {
                    DoAbsorbDamageEffect(effector, Wearer, this, ref dinfo, dodged: true);
                }
                return;
            }

            //得知道来源才能判定距离
            if (dinfo.Instigator != null)
            {
                if (Wearer != null && dinfo.Instigator.Position.AdjacentTo8WayOrInside(Wearer.Position))
                {
                    if (!Props.canAbsorbMeleeDmg)
                    {
                        absorbed = false;
                        return;
                    }

                    energy -= dinfo.Amount * Props.energyLostPerDmg * Props.meleeAbsorbFactor;
                }
                else
                {
                    energy -= dinfo.Amount * Props.energyLostPerDmg * Props.rangedAbsorbFactor;
                }
            }
            else
            {
                energy -= dinfo.Amount * Props.energyLostPerDmg;
            }

            //吸收了伤害 执行后效
            foreach (Type effector in Props.postAbsorbDmgEffects)
            {
                DoAbsorbDamageEffect(effector, Wearer, this, ref dinfo);
            }
            if (energy <= 0)
            {
                Break(ref dinfo);
            }
            else
            {
                HittedFleck(dinfo);
            }

            absorbed = true;
        }

        #region 舟味护盾条
        private Material barFillMat = null;
        private Material BarFilledMat
        {
            get
            {
                barFillMat ??= SolidColorMaterials.SimpleSolidColorMaterial(new Color32(245, 245, 245, 190));
                return barFillMat;
            }
        }

        private Material barUnfilledMat = null;
        private Material BarUnfilledMat
        {
            get
            {
                barUnfilledMat ??= SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.50f));
                return barUnfilledMat;
            }
        }
        private Vector3 IconMargin => Vector3.back * 1.125f + Vector3.left * 0.8f;
        private static Vector3 BottomMargin => Vector3.back * 1.125f;
        private static readonly Vector2 BarSize = new Vector2(1.5f, 0.025f);

        const string DEF_IconTexPath = "UI/Abilities/icon_sort_def";
        private Material iconDefend = null;
        private Material IconDefend
        {
            get
            {
                iconDefend ??= MaterialPool.MatFrom(DEF_IconTexPath, ShaderDatabase.Transparent);
                return iconDefend;
            }
        }
        public override void CompDrawWornExtras()
        {
            if (false)
            {
                return;
            }
            if (!ShouldDisplay)
            {
                return;
            }
            /*if (BarFilledMat == null || BarUnfilledMat == null)
            {
                BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(245, 245, 245, 190));
                BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.50f));
            }*/
            //Log.Message("有盾");
            GenDraw.FillableBarRequest fbr = default;
            fbr.center = Wearer.DrawPos + (Vector3.up * 3f) + BottomMargin;
            fbr.size = BarSize;
            fbr.filledMat = BarFilledMat;
            fbr.unfilledMat = BarUnfilledMat;
            //fbr.margin = 0;
            fbr.rotation = Rot4.North;
            fbr.fillPercent = EnergyPercent;
            /*Vanya_ShieldBelt shield = Parent;
            if (shield != null)
            {
                SheildPercent = shield.Energy / Mathf.Max(1f, shield.GetStatValue(StatDefOf.EnergyShieldEnergyMax));
            }
            else
            {
                SheildPercent = 0f;
            }*/
            GenDraw.DrawFillableBar(fbr);
            //GenDefendIcon();
            Matrix4x4 matrix = default;
            Vector3 scale = new Vector3(0.25f, 1f, 0.25f);
            matrix.SetTRS(Wearer.DrawPos + IconMargin, Rot4.North.AsQuat, scale);
            Graphics.DrawMesh(MeshPool.plane025, matrix, material: IconDefend, 2);
        }
        #endregion
        /*public override void PostDraw()
        {
            float angle = (DateTime.Now.Ticks / 10000 / 60) % 3600;
            angle /= 10;
            base.PostDraw();
        }*/
        #endregion
        protected virtual void Break(ref DamageInfo dinfo)
        {
            energy = 0;
            ticksReset = 0;

            if (Wearer.Map != null)
            {
                EnergyShield_Broken.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
                FleckMaker.Static(Wearer.TrueCenter(), Wearer.Map, FleckDefOf.ExplosionFlash, 12f);
                for (var i = 0; i < 6; i++)
                {
                    FleckMaker.ThrowDustPuff(
                        Wearer.TrueCenter() + (Vector3Utility.HorizontalVectorFromAngle(Rand.Range(0, 360)) *
                                               Rand.Range(0.3f, 0.6f)), Wearer.Map, Rand.Range(0.8f, 1.2f));
                }
            }

            foreach (Type effector in Props.postBreakEffects)
            {
                DoAbsorbDamageEffect(effector, Wearer, this, ref dinfo);
            }
        }

        protected virtual void Reset()
        {
            if (Wearer.Spawned)
            {
                SoundDefOf.EnergyShield_Reset.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
                FleckMaker.ThrowLightningGlow(Wearer.TrueCenter(), Wearer.Map, 3f);
            }

            ticksReset = 0;
            energy = Props.energyMax * Props.energyRatioOnReset;
        }
        //被攻击后特效 抄的
        protected void HittedFleck(DamageInfo dinfo)
        {
            if (Wearer.Map != null)
            {
                SoundDefOf.EnergyShield_AbsorbDamage.PlayOneShot(new TargetInfo(Wearer.Position, Wearer.Map));
                impactAngleVect = Vector3Utility.HorizontalVectorFromAngle(dinfo.Angle);
                var loc = Wearer.TrueCenter() + (impactAngleVect.RotatedBy(180f) * 0.5f);
                var num = Mathf.Min(10f, 2f + (dinfo.Amount / 10f));
                FleckMaker.Static(loc, Wearer.Map, FleckDefOf.ExplosionFlash, num);
                var num2 = (int)num;
                for (var i = 0; i < num2; i++)
                {
                    FleckMaker.ThrowDustPuff(loc, Wearer.Map, Rand.Range(0.8f, 1.2f));
                }
            }
        }

        #region 效果器处理 以后要是有别的分类方法可以移走
        static void DoAbsorbDamageEffect(Type effectorType, Pawn wearer, TC_GenericShield shield, ref DamageInfo dinfo, bool dodged = false)
        {
            AbsorbDamageEffector_Base effector = GetEffectorInstance(effectorType);
            effector?.PostAbsorbDamage(wearer, shield, ref dinfo, dodged);
        }

        static Dictionary<Type, AbsorbDamageEffector_Base> effectorMaps = new Dictionary<Type, AbsorbDamageEffector_Base>();
        static AbsorbDamageEffector_Base GetEffectorInstance(Type effectorType)
        {
            if (effectorMaps.ContainsKey(effectorType)) { return effectorMaps[effectorType]; }
            AbsorbDamageEffector_Base effector = (AbsorbDamageEffector_Base)Activator.CreateInstance(effectorType);
            effectorMaps.Add(effectorType, effector);
            return effector;
        }
        #endregion
    }
}
