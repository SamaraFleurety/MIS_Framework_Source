using AKA_Ability;
using AKA_Ability.TickCondition;
using AKR_Random;
using AKR_Random.RewardSet;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    //为预备干员或者种族之类的使用的def。此def不进招募台，也不限1，并且部分属性可以在一定范围内随机。
    public class ReserveOperatorDef : OperatorDef
    {
        public bool ignoreFixedAge = false;
        public ReserveOperatorDef() : base()
        {
            alwaysHidden = true;
            ignoreAutoFill = true;
        }

        //生成新人时，使用这个
        public PawnKindDef pawnkind;
        public PawnKindDef RecruitPawnkind => pawnkind ?? PawnKindDefOf.Colonist;

        public FactionDef faction = null;
        public Faction cachedFaction;
        public Faction RecruitFaction
        {
            get
            {
                if (cachedFaction != null) return cachedFaction;

                FactionDef def = null;
                if (faction != null) def = faction;
                else if (pawnkind != null && pawnkind.defaultFactionDef != null) def = pawnkind.defaultFactionDef;

                if (def != null)
                {
                    cachedFaction = Find.FactionManager.FirstFactionOfDef(def);
                }
                else
                {
                    cachedFaction = Faction.OfPlayer;
                }
                return cachedFaction;
            }
        }

        //名字遵循原版rule pack
        //pawnkind里面有
        //public RulePackDef nameRule = null;
        #region 角色可随机属性
        //可以让生成出来的角色仅在一定范围内随机

        public List<DefWithWeight> bodyTypeRange = null;
        public RandomizerNode_Rewards randNode_BodyType;
        //随机一个bodytypedef给当前正在刷的任务。下面类似名字的属性同理。
        public BodyTypeDef BodyTypeThisPawn
        {
            get
            {
                if (bodyTypeRange == null) return bodyTypeDef;
                randNode_BodyType ??= DefWithWeightToRandNode(bodyTypeRange);

                return randNode_BodyType.TryIssueGachaResult().First() as BodyTypeDef;
            }
        }

        public List<DefWithWeight> headTypeRange = null;
        public RandomizerNode_Rewards randNode_HeadType;
        public HeadTypeDef HeadTypeThisPawn
        {
            get
            {
                if (headTypeRange == null) return headTypeDef;
                randNode_HeadType ??= DefWithWeightToRandNode(headTypeRange);

                return randNode_HeadType.TryIssueGachaResult().First() as HeadTypeDef;
            }
        }

        public List<DefWithWeight> hairRange = null;
        public RandomizerNode_Rewards randNode_Hair;
        public HairDef HairThisPawn
        {
            get
            {
                if (hairRange == null) return base.hair;
                randNode_Hair ??= DefWithWeightToRandNode(hairRange);

                return randNode_Hair.TryIssueGachaResult().First() as HairDef;
            }
        }

        //public List<ColorOption> hairColorRange= null;
        ColorGenerator hairColorRange = null;
        public Color HairColorThisPawn
        {
            get
            {
                if (hairColorRange == null) return base.hairColor;
                //return hairColorRange.RandomElementByWeight(co => co.weight).only;
                return hairColorRange.NewRandomizedColor();
            }
        }

        public List<DefWithWeight> backstoryChildRange = null;
        public List<DefWithWeight> backstoryAdultRange = null;

        public RandomizerNode_Rewards randNode_storyChild;
        public BackstoryDef BackStoryChildThisPawn
        {
            get
            {
                if (backstoryChildRange == null) return childHood;
                randNode_storyChild ??= DefWithWeightToRandNode(backstoryChildRange);

                return randNode_storyChild.TryIssueGachaResult().First() as BackstoryDef;
            }
        }

        public RandomizerNode_Rewards randNode_storyAdult = null;
        public BackstoryDef BackStoryAdultThisPawn
        {
            get
            {
                if (backstoryAdultRange == null) return adultHood;
                randNode_storyChild ??= DefWithWeightToRandNode(backstoryChildRange);

                return randNode_storyChild.TryIssueGachaResult().First() as BackstoryDef;
            }
        }

        RandomizerNode_Rewards DefWithWeightToRandNode(List<DefWithWeight> weights)
        {
            RandomizerNode_Rewards randNode = new();
            foreach (DefWithWeight weight in weights)
            {
                randNode.rewards.Add(weight.TransformToRewardSet<Rewards_Def>());
            }
            return randNode;
        }

        protected override void FixAlienHairColor()
        {
            if (ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null)
            {
                HediffWithComps hediffAlienPatch = HediffMaker.MakeHediff(AKDefOf.AK_Hediff_AlienRacePatch, operator_Pawn, operator_Pawn.health.hediffSet.GetBrain()) as HediffWithComps;
                HC_ForceColors comp = hediffAlienPatch.TryGetComp<HC_ForceColors>();
                comp.exactProps.skinColor = this.skinColor;
                comp.exactProps.hairColor = HairColorThisPawn;
            }
        }
        #endregion

        #region 招募

        public override Pawn Recruit_NoMap()
        {
            currentlyGenerating = true;

            operator_Pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(RecruitPawnkind, RecruitFaction, forcedXenotype: xenoType));

            Recruit_Hediff();

            Recruit_PersonalStat();

            //估计预备干员写不了关系
            //Recruit_AddRelations();

            Recruit_Inventory();

            if (ModLister.GetActiveModWithIdentifier("mis.arkmusic") != null) Recruit_ArkSongExtension();

            //基因
            if (ModLister.BiotechInstalled)
            {
                operator_Pawn.genes.ClearXenogenes();
            }
            //播放语音
            this.voicePackDef?.recruitSound.PlaySound();

            //档案系统
            VAbility_AKATrackerContainer operatorID = Recruit_VAB();

            //对vab容器进行aka技能以外的处理
            Recruit_OperatorID(operatorID);
            //(operatorID.AKATracker as AK_AbilityTracker).doc = doc;
            clothTemp.Clear();

            Recruit_AKAbility(operatorID);

            Recruit_PostEffects();

            currentlyGenerating = false;

            return operator_Pawn;
        }

        protected override VAbility_AKATrackerContainer Recruit_VAB()
        {
            VAbility_AKATrackerContainer res = base.Recruit_VAB();
            res.AKATracker = new AbilityTracker(operator_Pawn);
            res.AKATracker.tickCondition = new TickCondion_Base(res.AKATracker);
            return res;
        }

        protected override void Recruit_OperatorID(VAbility_AKATrackerContainer vab)
        {
        }

        protected override void Recruit_PersonalStat()
        {
            //避免Bug更改
            operator_Pawn.needs.food.CurLevel = operator_Pawn.needs.food.MaxLevel;

            //pawnkind里面有rule pack
            //operator_Pawn.Name = new NameTriple(this.name, this.nickname, this.surname);//“名”“简”“姓”

            //性别更改
            operator_Pawn.gender = this.isMale ? Gender.Male : Gender.Female;

            if (!ignoreFixedAge)
            {
                operator_Pawn.ageTracker.AgeBiologicalTicks = this.age * (long)TimeToTick.year;
                operator_Pawn.ageTracker.AgeChronologicalTicks = this.realAge * (long)TimeToTick.year;
            }

            //发型与体型设置
            operator_Pawn.story.bodyType = this.BodyTypeThisPawn;
            operator_Pawn.story.headType = this.HeadTypeThisPawn ?? DefDatabase<HeadTypeDef>.GetNamed("Female_NarrowPointy");
            operator_Pawn.story.hairDef = HairThisPawn ?? HairDefOf.Bald;
            operator_Pawn.style.beardDef = this.beard ?? BeardDefOf.NoBeard;
            operator_Pawn.story.skinColorOverride = this.skinColor;
            operator_Pawn.story.HairColor = HairColorThisPawn;

            //特性更改
            /*operator_Pawn.story.traits.allTraits.Clear();
            foreach (TraitAndDegree TraitAndDegree in this.traits)
            {
                operator_Pawn.story.traits.GainTrait(new Trait(TraitAndDegree.def, TraitAndDegree.degree));
            }*/
            operator_Pawn.story.Childhood = BackStoryChildThisPawn;
            operator_Pawn.story.Adulthood = this.age < BackstoryAdultAgeThreshold ? null : BackStoryAdultThisPawn;
            //背景设置

            //清除因为自动生成的故事和特性导致的，某些工作被禁用的缓存
            operator_Pawn.Notify_DisabledWorkTypesChanged();

            /*operator_Pawn.skills.skills.Clear();
            foreach (SkillAndFire skillDef in this.skills)
            {
                SkillRecord skill = new(operator_Pawn, skillDef.skill)
                {
                    passion = skillDef.fireLevel,
                    Level = skillDef.level
                };
                operator_Pawn.skills.skills.Add(skill);
            }*/
            //技能属性更改
        }
        #endregion
    }
}
