using AKA_Ability.CastConditioner;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace AKA_Ability.Gizmos
{
    //当前仅给召唤技能用的 额外gizmo渲染。召唤技能有召唤和选择所有召唤物2个按钮。
    //说实话多写一个"选择所有召唤物"技能可能也行。都行。
    //也有可能以后有的技能需要额外一个什么能量条之类的，就不适合多写个技能了
    //偷懒，这玩意既是Property又是worker 可能以后需要改
    //有一定可能性技能实例不绑定默认gizmo全部用这套也行
    //写了个大概又感觉好像暂时没啥必要
    /*public abstract class ExtraGizmoDrawer_Base
    {
        public string iconPath;
        public string label;
        public string description;

        //先不整这么复杂
        //public List<CastConditioner_Base> castConditioners = new();

        #region 非xml可填参数
        public Texture2D Icon => ContentFinder<Texture2D>.Get(iconPath);
        public AKAbility_Base parent;

        public Command cachedGizmo = null;

        #endregion

        public Command GetExtraGizmo()
        {
            if (cachedGizmo == null) InitExtraGizmo();
            return cachedGizmo;
        }

        protected abstract void InitExtraGizmo();

        public virtual void UpdateExtraGizmo()
        {
        }

        public virtual ExtraGizmoDrawer_Base DeepCopy()
        {
            ExtraGizmoDrawer_Base newDrawer = (ExtraGizmoDrawer_Base)Activator.CreateInstance(GetType());
            newDrawer.iconPath = iconPath;
            newDrawer.label = label;
            newDrawer.description = description;

            return newDrawer;
        }
    }*/
}
