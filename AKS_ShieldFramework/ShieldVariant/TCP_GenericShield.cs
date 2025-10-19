using AK_DLL;
using AKS_Shield.Effector;
using AKS_Shield.Extension;
using RimWorld;
using System;
using System.Collections.Generic;
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
        public int ticksToReset = 3600;

        //吸收伤害
        public bool canAbsorbMeleeDmg = true;

        public float meleeAbsorbFactor = 1;
        public float rangedAbsorbFactor = 1;

        public float meleeDodgeChanceFactor = 1;   //会乘以pawn的近战闪避  我靠，好像dinfo不能区分近战远程伤害
        //public float rangedDodgeChance = 0;        //0-1，躲避子弹的概率。原版并不能演黑客帝国。

        public List<Type> postBreakEffects = new();              //仅当命中并且本次命中导致护盾破碎时调用
        public List<Type> postAbsorbDmgEffects = new();          //命中后调用 是否闪避是参数

        public TCP_GenericShield()
        {
            compClass = typeof(TC_GenericShield);
        }
    }

    public class TC_GenericShield : ThingComp
    {
        private readonly SoundDef EnergyShield_Broken = SoundDef.Named("EnergyShield_Broken");
        public TCP_GenericShield Props => props as TCP_GenericShield;

        protected float energy = 0;

        public virtual float Energy
        {
            get { return energy; }

            set { energy = value; }
        }

        protected int ticksReset = 0;

        public Apparel Parent => parent as Apparel;
        public virtual Pawn Wearer => Parent.Wearer;

        public virtual bool ShouldDisplay
        {
            get
            {
                Pawn wearer = Wearer;
                return wearer != null && wearer.Spawned && !wearer.Destroyed && !wearer.Dead && !wearer.Downed && Energy > 0 && wearer.Drafted;
            }
        }


        private Vector3 impactAngleVect;

        public virtual float EnergyMax => Props.energyMax;

        public virtual float EnergyPercent => Energy / EnergyMax;

        public virtual float EnergyRegenRate => Props.energyRegenRate / 60;

        public HashSet<TC_ShieldExtension_PostEffects_Base> registedCompEffectors = new();

        #region 原版

        public override void PostPostMake()
        {
            base.PostPostMake();
            Energy = EnergyMax;
        }
        public override void CompTick()
        {
            Tick(1);
        }
        public virtual void Tick(int amt)
        {
            if (Energy > 0)
            {
                if (Energy < EnergyMax)
                {
                    Energy += EnergyRegenRate * amt;
                    if (!Props.allowEnergyOverflow)
                    {
                        Energy = Math.Min(Energy, EnergyMax);
                    }
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
            if (Energy <= 0)
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

            //判断闪避
            if (Props.meleeDodgeChanceFactor > 0 && Wearer != null && Rand.Chance(Wearer.GetStatValue(StatDefOf.MeleeDodgeChance) * Props.meleeDodgeChanceFactor))
            {
                MoteMaker.ThrowText(Wearer.DrawPos, Wearer.Map, "TextMote_Dodge".Translate(), 1.9f);
                absorbed = true;
                foreach (Type effector in Props.postAbsorbDmgEffects)
                {
                    DoAbsorbDamageEffect(effector, Wearer, this, ref dinfo, dodged: true);
                    foreach (TC_ShieldExtension_PostEffects_Base c in registedCompEffectors)
                    {
                        c.Notify_Absorb(dinfo, dodged: true);
                    }
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

                    Energy -= dinfo.Amount * Props.energyLostPerDmg * Props.meleeAbsorbFactor;
                }
                else
                {
                    Energy -= dinfo.Amount * Props.energyLostPerDmg * Props.rangedAbsorbFactor;
                }
            }
            else
            {
                Energy -= dinfo.Amount * Props.energyLostPerDmg;
            }

            //吸收了伤害 执行后效
            foreach (Type effector in Props.postAbsorbDmgEffects)
            {
                DoAbsorbDamageEffect(effector, Wearer, this, ref dinfo);
            }
            if (Energy <= 0)
            {
                Break(ref dinfo);       //break后效在这里面
            }
            else
            {
                HittedFleck(dinfo);
            }

            foreach (TC_ShieldExtension_PostEffects_Base c in registedCompEffectors)
            {
                c.Notify_Absorb(dinfo);
            }
            absorbed = true;
        }

        public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
        {
            if (Wearer == null || Wearer.Faction.HostileTo(Faction.OfPlayer)) yield break;
            foreach (var gizmo in GetGizmos()) yield return gizmo;
        }

        public virtual IEnumerable<Gizmo> GetGizmos()
        {
            yield return new Gizmo_ShieldStatus()
            {
                shield = this
            };
        }
        #endregion

        #region 原版护盾gizmo

        public virtual string VanillaGizmoLabel => parent.LabelCap;
        public virtual string VanillaGizmoDesc => parent.def.description;
        #endregion

        #region 舟味护盾条
        private static bool? cachedAKLibActiveStatue = null;
        private static bool AKLibActived
        {
            get
            {
                cachedAKLibActiveStatue ??= ModLister.GetActiveModWithIdentifier("MIS.Framework") != null;
                return (bool)cachedAKLibActiveStatue;
            }
        }

        private static bool? cachedCameraPlusModActiveStatue = null;
        private static bool CameraPlusModEnabled
        {
            get
            {
                if (AKLibActived) return AK_Mod.CameraPlusModEnabled;
                cachedCameraPlusModActiveStatue ??= ModLister.GetActiveModWithIdentifier("brrainz.cameraplus") != null;
                return (bool)cachedCameraPlusModActiveStatue;
            }
        }
        private static bool? cachedSimpleCameraModActiveStatue = null;
        private static bool SimpleCameraModEnabled
        {
            get
            {
                if (AKLibActived) return AK_Mod.SimpleCameraModEnabled;
                cachedSimpleCameraModActiveStatue ??= ModLister.GetActiveModWithIdentifier("ray1203.SimpleCameraSetting") != null;
                return (bool)cachedSimpleCameraModActiveStatue;
            }
        }

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
        private float ZoomRootSize => Find.CameraDriver.ZoomRootSize;
        private float GetZoomRatio()
        {
            if (AKLibActived && AK_ModSettings.zoomWithCamera)
            {
                return Mathf.Max(ZoomRootSize, 11) / 11;
            }
            return 1f;
        }
        private static float Width
        {
            get
            {
                if (AKLibActived) return AK_ModSettings.barWidth * 0.01f;
                return 150 * 0.01f;
            }
        }
        private static float Height
        {
            get
            {
                if (AKLibActived) return AK_ModSettings.barHeight * 0.001f;
                return 75 * 0.01f;
            }
        }
        private static float Margin
        {
            get
            {
                if (AKLibActived) return AK_ModSettings.barMargin * 0.01f;
                return -100 * 0.01f;
            }
        }
        private static Vector2 BarSize => new(Width, Height);
        private static Vector3 BottomMargin => new(0f, 0f, Margin - (Height * 2));

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
            if (!ShouldDisplay)
            {
                return;
            }
            Vector3 drawPos = Wearer.DrawPos;
            float zoomRatio = GetZoomRatio();
            float zoomWidthRatio;
            float zoomYRatio;
            if (CameraPlusModEnabled || SimpleCameraModEnabled)
            {
                zoomWidthRatio = zoomRatio > 4.35f ? 4.35f : zoomRatio;
                zoomYRatio = zoomRatio > 5f ? 5f : zoomRatio;
            }
            else
            {
                zoomWidthRatio = zoomRatio > 3.75f ? 3.75f : zoomRatio;
                zoomYRatio = zoomRatio > 3f ? 3f : zoomRatio;
            }
            GenDraw.FillableBarRequest fbr = default;
            if (CameraPlusModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + (BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.9f : zoomYRatio));
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 1.75f ? zoomRatio * 1.5f : zoomRatio;
            }
            else if (SimpleCameraModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + (BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio));
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 3f ? zoomRatio * 1.05f : zoomRatio;
            }
            else
            {
                fbr.center = drawPos + (Vector3.up * 3f) + (BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio));
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 6.5f ? zoomRatio * 1.25f : zoomRatio;
            }
            fbr.filledMat = BarFilledMat;
            fbr.unfilledMat = BarUnfilledMat;
            fbr.rotation = Rot4.North;
            fbr.fillPercent = EnergyPercent;
            GenDraw.DrawFillableBar(fbr);
            Matrix4x4 matrix = default;
            Vector3 scale = new(0.25f, 1f, 0.25f);
            Vector3 iconPos = new(fbr.center.x - (fbr.size.x / 2) - 0.075f, fbr.center.y, fbr.center.z);
            matrix.SetTRS(iconPos, Rot4.North.AsQuat, scale);
            Graphics.DrawMesh(MeshPool.plane025, matrix, material: IconDefend, 2);
        }
        #endregion

        protected virtual void Break(ref DamageInfo dinfo)
        {
            Energy = 0;
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

            foreach (TC_ShieldExtension_PostEffects_Base c in registedCompEffectors)
            {
                c.Notify_Break(dinfo);
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
            Energy = EnergyMax * Props.energyRatioOnReset;

            foreach (TC_ShieldExtension_PostEffects_Base c in registedCompEffectors)
            {
                c.Notify_Reset();
            }
        }
        //被攻击后特效 抄的
        protected virtual void HittedFleck(DamageInfo dinfo)
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

        static Dictionary<Type, AbsorbDamageEffector_Base> effectorMaps = new();
        static AbsorbDamageEffector_Base GetEffectorInstance(Type effectorType)
        {
            if (effectorMaps.ContainsKey(effectorType)) { return effectorMaps[effectorType]; }
            AbsorbDamageEffector_Base effector = (AbsorbDamageEffector_Base)Activator.CreateInstance(effectorType);
            effectorMaps.Add(effectorType, effector);
            return effector;
        }
        #endregion


        /*public override string CompInspectStringExtra()
        {
            return $""
        }*/
    }
}
