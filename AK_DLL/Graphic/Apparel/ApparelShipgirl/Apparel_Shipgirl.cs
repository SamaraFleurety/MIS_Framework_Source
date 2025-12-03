using AK_DLL.Document;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL.Apparels
{
    public class ApparelDef_Shipgirl : ThingDef
    {
        public Type nameRule = typeof(ApparelNameRule_Apparel);

        //未征召时隐藏服装
        public bool hideWhenUndraft = false;
        //服装额外分类。用于动态渲染时读png文件。这里面的string指：Bismarck_*此分类string*_(体型)_(朝向).png。此string不含_
        public string apparelExtraCategory = null;
    }

    //舰娘用，新标准衣服。所有衣服数值相同，用同一个def。此衣服渲染时贴图和描述会根据穿戴者变化。
    //主要注重穿着时效果，掉地上的贴图显示和普通衣服无异 -- 如果要不同贴图，多写一个def。
    public class Apparel_Shipgirl : Apparel
    {
        #region 参数
        public ApparelDef_Shipgirl DefInner => this.def as ApparelDef_Shipgirl;

        //必然绑定某个角色
        public OperatorDef operatorDef;

        //时装后缀，用于区分不同时装。
        //舰娘仅支持新版逻辑，即原皮也是服装
        public OperatorFashionSetDef fashion = null;

        Ext_OperatorExtraInfo ext = null;
        public Ext_OperatorExtraInfo Ext
        {
            get
            {
                if (ext == null)
                {
                    ext = fashion?.GetModExtension<Ext_OperatorExtraInfo>();
                    ext ??= this.operatorDef.GetModExtension<Ext_OperatorExtraInfo>();
                }
                return ext;
            }
        }
        #endregion

        #region 图像处理
        string cachedWornGraphicPath = null;

        //贴图根路径，不好说用这个路径是不是个坏主意
        string TextureRootPath => this.def.apparel.wornGraphicPath;

        public virtual string WornGraphicPathOverride
        {
            get
            {
                if (DefInner.hideWhenUndraft && !Wearer.Drafted) return null;

                if (cachedWornGraphicPath == null)
                {
                    if (fashion == null)
                    {
                        Log.Error($"[AK]新版衣服必须在套装里");
                        cachedWornGraphicPath = TextureRootPath;
                        return cachedWornGraphicPath;
                    }

                    string opID = AK_Tool.GetOperatorIDFrom(operatorDef.defName);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(TextureRootPath);

                    //按职业分文件夹
                    sb.Append("/");
                    sb.Append(operatorDef.operatorType.textureFolder);

                    //然后按舰名 纪传体式分文件夹
                    sb.Append("/");
                    sb.Append(opID);

                    //这里开始都是在处理png的文件名
                    sb.Append("/");

                    //角色名字
                    sb.Append(opID);

                    //换装后缀
                    if (fashion != null)
                    {
                        if (fashion.fashionName != null)
                        {
                            sb.Append("_");
                            //sb.Append(fashion.GetOperatorFashionNameFrom());
                            sb.Append(fashion.fashionName);
                        }
                    }

                    //服装分类(比如舰装)
                    if (DefInner.apparelExtraCategory != null)
                    {
                        sb.Append("_");
                        sb.Append(DefInner.apparelExtraCategory);
                    }

                    //大破相关处理
                    if (false)
                    {

                    }

                    //这后面是体型和朝向。此处不处理

                    cachedWornGraphicPath = sb.ToString();
                }
                return cachedWornGraphicPath;
            }
        }
        public void SetDirty()
        {
            ext = null;
            cachedWornGraphicPath = null;
            Notify_ColorChanged();
        }
        #endregion

        #region 文案处理
        ApparelNameRule_Apparel NameRuleWorker => GC_Generic.instance.singletonManager.GetSharedSingleton<ApparelNameRule_Apparel>(DefInner.nameRule);
        public override string LabelNoCount => NameRuleWorker.Label(this);

        //从原版复制的 不至于这也写个转译器吧
        string descriptionDetailedCached = null;
        public override string DescriptionDetailed
        {
            get
            {
                if (descriptionDetailedCached == null)
                {
                    string description = NameRuleWorker.Description(this);
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(description);

                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(string.Format("{0}: {1}", "Layer".Translate(), def.apparel.GetLayersString()));
                    stringBuilder.Append(string.Format("{0}: {1}", "Covers".Translate(), def.apparel.GetCoveredOuterPartsString(BodyDefOf.Human)));
                    if (def.equippedStatOffsets != null && def.equippedStatOffsets.Count > 0)
                    {
                        stringBuilder.AppendLine();
                        stringBuilder.AppendLine();
                        for (int i = 0; i < def.equippedStatOffsets.Count; i++)
                        {
                            if (i > 0)
                            {
                                stringBuilder.AppendLine();
                            }

                            StatModifier statModifier = def.equippedStatOffsets[i];
                            stringBuilder.Append($"{statModifier.stat.LabelCap}: {statModifier.ValueToStringAsOffset}");
                        }
                    }


                    descriptionDetailedCached = stringBuilder.ToString();
                }

                return descriptionDetailedCached;
            }
        }

        #endregion

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref operatorDef, "operatorDef");
            Scribe_Defs.Look(ref fashion, "fashionDef");
        }
    }
}
