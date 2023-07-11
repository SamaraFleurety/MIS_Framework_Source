using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AK_DLL
{
    public class HC_DpsEnhance_Ranged : HediffComp
    {
        #region 属性
        private HCP_DpsEnhance_Ranged exactProps = new HCP_DpsEnhance_Ranged();

        public HCP_DpsEnhance_Ranged Props
        {
            get { return (HCP_DpsEnhance_Ranged)base.props; }
        }
        public RangedStat StatOffset
        {
            get { return this.exactProps.statOffset; }
            set { this.exactProps.statOffset = value; }
        }

        public int Duration
        {
            get { return this.exactProps.duration; }
            set { this.exactProps.duration = value; }
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
            get { return $"增加远程攻击力,剩余{this.Duration}秒"; }
        }

        public override void CompExposeData()
        {
            this.endEnhance();
        }
        #endregion

        private RangedStat originalStat;
        private int tick = 60;
        private Thing weapon;
        private bool enhanced = false;

        private void applyStats(Thing w, RangedStat stats, bool saveOrigin)
        {
            //保存原始数据时，说明stats是加值，反之是覆盖值。
            int multipler = saveOrigin ? 1 : 0;
            VerbProperties verb = w.def.Verbs[0];
            ProjectileProperties bullet = verb.defaultProjectile.projectile;
            if (stats.stats != null)
            {
                //处理statBases。不知道怎么搜索。
                foreach (StatModifier i in stats.stats)
                {
                    foreach (StatModifier j in this.weapon.def.statBases)
                    {
                        if (i.stat == j.stat)
                        {
                            if (saveOrigin)
                            {
                                this.originalStat.stats.Add(new StatModifier());
                                this.originalStat.stats.Last().stat = j.stat;
                                this.originalStat.stats.Last().value = j.value;
                            }
                            j.value = i.value + j.value * multipler;
                            break;
                        }
                    }
                }
                //写一个type数组然后用迭代搞不好也行。我觉得比写一堆if还麻烦和不直观。
                if (stats.preCD != 0)
                {
                    if (saveOrigin) this.originalStat.preCD = verb.warmupTime;
                    verb.warmupTime = stats.preCD + verb.warmupTime * multipler;
                }
                if (stats.range != 0)
                {
                    if (saveOrigin) this.originalStat.range = verb.range;
                    verb.range = stats.range + verb.range * multipler;
                }
                if (stats.burstInterval != 0)
                {
                    if (saveOrigin) this.originalStat.burstInterval = verb.ticksBetweenBurstShots;
                    verb.ticksBetweenBurstShots = stats.burstInterval + verb.ticksBetweenBurstShots * multipler;
                }
                if (stats.burstCount != 0)
                {
                    if (saveOrigin) this.originalStat.burstCount = verb.burstShotCount;
                    verb.burstShotCount = stats.burstCount + verb.burstShotCount * multipler;
                }
                //改了子弹当然不需要再改子弹属性了
                if (stats.bullet != null)
                {
                    if (saveOrigin)
                    {
                        this.originalStat.bullet = verb.defaultProjectile.defName;
                    }
                    verb.defaultProjectile = DefDatabase<ThingDef>.GetNamed(stats.bullet);
                }
                else
                {
                    if (stats.damageType != null)
                    {
                        if (saveOrigin) this.originalStat.damageType = bullet.damageDef.defName;
                        bullet.damageDef = DefDatabase<DamageDef>.GetNamed(stats.damageType);
                    }
                    if (stats.damage != 0)
                    {
                        //私有字段，拿反射做的
                        System.Reflection.FieldInfo damage = bullet.GetType().GetField("damageAmountBase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (saveOrigin) this.originalStat.damage = (int)damage.GetValue(bullet);
                        damage.SetValue(bullet, stats.damage + (int)damage.GetValue(bullet) * multipler);
                    }
                    if (stats.armorPenetrate != 0)
                    {
                        System.Reflection.FieldInfo ap = bullet.GetType().GetField("armorPenetrationBase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (saveOrigin) this.originalStat.armorPenetrate = (float)ap.GetValue(bullet);
                        ap.SetValue(bullet, stats.armorPenetrate + (float)ap.GetValue(bullet) * multipler);
                    }
                    if (stats.bulletSpeed != 0)
                    {
                        if (saveOrigin) this.originalStat.bulletSpeed = bullet.speed;
                        bullet.speed = stats.bulletSpeed + bullet.speed * multipler;
                    }
                }
                if (stats.fireSound != null)
                {
                    if (saveOrigin) this.originalStat.fireSound = verb.soundCast.defName;
                    verb.soundCast = DefDatabase<SoundDef>.GetNamed(stats.fireSound);
                }
            }
        }

        private void endEnhance()
        {
            applyStats(weapon, this.originalStat, false);
            this.parent.Severity -= 10f;
        }
        public override void CompPostTick(ref float severityAdjustment)
        {
            ++tick;
            if (this.tick >= 60)
            {
                this.tick = 0;
                this.Duration -= 1;
                if (!enhanced)
                {
                    this.enhanced = true;
                    this.originalStat = new RangedStat();
                    this.weapon = AK_Tool.GetDoc(base.Pawn).weapon;
                    applyStats(weapon, this.StatOffset, true);
                }
                else if (this.Duration <= 0)
                {
                    this.endEnhance();
                }
            }
        }
    }
}
