using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AKE_OperatorExtension
{
    //砍口垒那种大破机制
    //因为设计上不鼓励脱下衣服，衣服也不能再制作，所以实际机制是：此衣服有耐久，但不会低于1；根据衣服耐久减少贴图会变化
    public class Apparel_Breakdown : Apparel
    {
        public override int HitPoints
        {
            get => base.HitPoints;
            set
            {
                base.HitPoints = Mathf.Clamp(value, 1, MaxHitPoints);

                Notify_HPChanged();
            }
        }

        Ext_ApparelBreakGraphic ext = null;
        int lastIndex = -1;
        float lastHPRatio = -1;
        string cachedWornGraphicPath = null;

        float HPRatio => HitPoints / (float)MaxHitPoints;

        public string WornGraphicPathByHP
        {
            get
            {
                if (cachedWornGraphicPath == null)
                {
                    lastHPRatio = -999;
                    Notify_HPChanged();
                }
                return cachedWornGraphicPath;
            }
        }

        public void Notify_HPChanged()
        {
            if (HPRatio == lastHPRatio) return;

            ext ??= def.GetModExtension<Ext_ApparelBreakGraphic>();
            if (ext == null)
            {
                Log.Error($"[AKE] {def.defName}是可大破的衣服，但是无对应ext");
                return;
            }

            lastHPRatio = HPRatio;
            cachedWornGraphicPath = ext.WornGraphicPathByHPRatio(HPRatio, out int index);

            //hp不同但是使用的图相同也没必要刷新
            if (index != lastIndex)
            {
                lastIndex = index;
                Notify_ColorChanged();
            }
        }

        public override IEnumerable<Gizmo> GetWornGizmos()
        {
            foreach (Gizmo gizmo in base.GetWornGizmos()) yield return gizmo;

            if (Prefs.DevMode)
            {
                yield return new Command_Action
                {
                    defaultLabel = "[Dev]加10hp",
                    defaultDesc = "desc",
                    action = () =>
                    {
                        this.HitPoints += 10;
                    }
                };
                yield return new Command_Action
                {
                    defaultLabel = "[Dev]减10hp",
                    defaultDesc = "desc",
                    action = () =>
                    {
                        this.HitPoints -= 10;
                    }
                };
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref cachedWornGraphicPath, "cachedPath");   //不知道读档的什么b地方会调用 没这个缓存会报错
        }
    }
}
