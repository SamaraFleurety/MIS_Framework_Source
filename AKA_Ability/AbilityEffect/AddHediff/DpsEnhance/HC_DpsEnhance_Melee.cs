using AK_DLL;
using System.Collections.Generic;
using Verse;

namespace AKA_Ability
{
    public class HC_DpsEnhance_Melee : HediffComp
    {

        #region 属性
        private HCP_DpsEnhance_Melee exactProps = new();
        public HCP_DpsEnhance_Melee Props
        {
            get { return (HCP_DpsEnhance_Melee)base.props; }
        }

        public List<toolEnhance> Enhances
        {
            get { return this.exactProps.enhances; }
            set { this.exactProps.enhances = value; }
        }

        public int Interval
        {
            get { return this.exactProps.interval; }
            set { this.exactProps.interval = value; }
        }

        public int ProcedureCount
        {
            get { return this.exactProps.procedureCount; }
            set { this.exactProps.procedureCount = value; }
        }

        public int EnhanceEndTime
        {
            get { return this.exactProps.enhanceEndTime; }
            set { this.exactProps.enhanceEndTime = value; }
        }
        #endregion

        #region 规范化组件
        public override void CompPostMake()
        {
            base.CompPostMake();
            this.exactProps = this.Props;
        }

        public override string CompLabelInBracketsExtra
        {
            get { return $"增加近战攻击力,剩余{this.EnhanceEndTime - this.pastRSec}秒"; }
        }

        public override void CompExposeData()
        {
            this.EndEnhance();
        }
        #endregion

        private int tick = 0;
        private int pastRSec = 0;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.tick++;
            if (this.tick >= (int)TimeToTick.rSecond * this.Interval)  //默认每 1秒(60tick) 渐进1次
            {
                pastRSec++;
                this.tick = 0;
            }
        }

        private void EndEnhance()
        {
            RestoreTools(this.Enhances);
            this.parent.Severity -= 10f;
        }

        private void RestoreTools(List<toolEnhance> enhances)
        {
            if (enhances == null) return;
            foreach (toolEnhance i in enhances)
            {
                if (i.tool == null) continue;
                i.tool.power = i.originalPower;
                i.tool.cooldownTime = i.originalCD;
            }
        }
        private void EnhanceTools(List<Tool> tools, List<toolEnhance> enhances)
        {
            if (tools == null || enhances == null) return;
            foreach (toolEnhance enhance in enhances)
            {
                if (enhance.tool == null && !enhance.shouldSkip) enhance.tool = findTool(enhance, tools);
                if (enhance.tool != null)
                {
                    enhance.tool.power += enhance.powerOffsetTotal / (float)this.ProcedureCount;
                    enhance.tool.cooldownTime += enhance.CDOffsetTotal / (float)this.ProcedureCount;
                }
            }
        }

        private Tool findTool(toolEnhance enhance, List<Tool> tools)
        {
            foreach (Tool i in tools)
            {
                if (i.cooldownTime == enhance.originalCD && i.power == enhance.originalPower)
                {
                    return i;
                }
            }
            Log.Warning($"CD:{enhance.originalCD} dmg:{enhance.originalPower} 找不到对应攻击方式");
            enhance.shouldSkip = true;
            return null;
        }
    }
}
