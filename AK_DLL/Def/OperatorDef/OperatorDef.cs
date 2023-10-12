using System;
using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using RimWorld.Planet;
using FS_LivelyRim;

namespace AK_DLL
{
    public class OperatorDef : Def
    {
        #region 干员属性 xml里面的
        [MustTranslate]
        public string name;//名字
        [MustTranslate]
        public string surname;//姓氏
        [MustTranslate]
        public string nickname;//昵称

        public BackstoryDef childHood;//童年背景故事
        public BackstoryDef adultHood;//成年背景故事

        public List<OperatorAbilityDef> abilities = new List<OperatorAbilityDef>();//技能

        public int age = 16;//年龄
        public int realAge = -1; //实际年龄
        public bool isMale = false;//性别
        public List<HediffStat> hediffInate;

        public VoicePackDef voicePackDef;

        public Dictionary<string, PawnRelationDef> relations;

        public List<TraitAndDegree> traits;//干员特性
        public ThingDef weapon;//干员武器                                                  
        public List<ThingDef> apparels;//干员衣服
        public List<ThingDef> accessory; //干员配件。和衣服的区别是在换装时不会丢掉。适合填入比如护盾。
        public List<ItemOnSpawn> items;
        public BodyTypeDef bodyTypeDef;//干员的体型
        public HeadTypeDef headTypeDef;
        private List<SkillAndFire> skills;//技能列表；要是哪天排序卡得不行就给改成树
        private bool skillSorted = false;
        public Color skinColor = new Color(1, 1, 1, 1); /*= PawnSkinColors.GetSkinColor(0.5f)*///皮肤颜色
        public Color hairColor = new Color(1, 1, 1, 1); /*= PawnSkinColors.GetSkinColor(1f)*///头发颜色
        public HairDef hair;//头发类型
        public BeardDef beard;//胡须

        public OperatorClassDef operatorType;//干员类型

        public string stand;//精2立绘
        public string commonStand;  //精0立绘
        public List<string> fashion; //换装
        //换装后，体现在rw服装上的变化。key的int是换装在List<string> fashion中的下标。
        //按理说应该和上面的干员衣服整合一起，但现在已经几百个干员了，要整合工作量太大。立项的时候没考虑做换装。
        public Dictionary<int, OperatorClothSet> clothSet; 
        public List<LiveModelDef> live2dModel;

        //因为并不知道是否有某种立绘，所以用字典存。约定-1为头像，0是精0立绘，1是精2立绘，2-后面是换装
        //这里的V3，x和y是x轴和y轴的偏移，z其实是缩放
        public Dictionary<int, Vector3> standOffsets = new Dictionary<int, Vector3>();
        public Vector2 standOffset;
        public float standRatio = 3f;
        public string headPortrait;//头像
        public Vector2 headPortraitOffset;

        public ThoughtDef thoughtReceived = null;  //其他所有人都会给这个干员一个想法 当前是和弦独有
        public int TRStage = -1;  //全部丢进同一个想法 节省性能

        public float ticketCost = 1f;

        public XenotypeDef xenoType;

        #endregion

        #region 快捷属性
        public static bool currentlyGenerating = false;

        private static List<Thing> fashionSet;
        public string Prefix
        {
            get { return AK_Tool.GetPrefixFrom(this.defName); }
        }
        public string OperatorID
        {
            get { return AK_Tool.GetOperatorIDFrom(this.defName); }
        }
        public List<SkillAndFire> Skills
        {
            get
            {
                /*string operatorName = AK_Tool.GetOperatorNameFromDefName(this.defName);
                if (GameComp_OperatorDocumentation.operatorDocument.ContainsKey(operatorName)) return GameComp_OperatorDocumentation.operatorDocument[operatorName].skillAndFires;
                else*/
                return skills;
            }
        }
        public List<SkillAndFire> SortedSkills
        {
            get
            {
                if (skillSorted) return skills;
                for(int i = 0; i < skills.Count; ++i)
                {
                    int loc = TypeDef.statType[skills[i].skill.defName];
                    SkillAndFire temp;
                    while (i != loc)
                    {
                        temp = skills[i];
                        skills[i] = skills[loc];
                        skills[loc] = temp; 
                        loc = TypeDef.statType[skills[i].skill.defName];
                    }
                }
                skillSorted = true;
                return skills;
            }
        }
        public Texture2D PreferredStand
        {
            get
            {
                if (GameComp_OperatorDocumentation.opDocArchive.ContainsKey(this.OperatorID) == false)
                {
                    return ContentFinder<Texture2D>.Get(this.stand);
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        public virtual void Recruit(IntVec3 intVec, Map map)
        {
            currentlyGenerating = true;

            //operator_Pawn = PawnGenerator.GeneratePawn(new pa PawnKindDefOf.Colonist, Faction.OfPlayer, ge);
            operator_Pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Colonist, Faction.OfPlayer, forcedXenotype: xenoType));
            Hediff_Operator hediff = Recruit_Hediff();

            Recruit_PersonalStat();

            Recruit_AddRelations();

            //operator_Pawn.story.CrownType = CrownType.Average;

            ThingWithComps weapon = Recruit_Inventory();

            GenSpawn.Spawn(operator_Pawn, intVec, map);
            CameraJumper.TryJump(new GlobalTargetInfo(intVec, map));

            //基因
            if (ModLister.BiotechInstalled)
            {
                operator_Pawn.genes.ClearXenogenes();
                //operator_Pawn.genes = new Pawn_GeneTracker(operator_Pawn);
                //operator_Pawn.genes.SetXenotype(DefDatabase<XenotypeDef>.GetNamed("AK_BaseType"));
            }
            //播放语音
            this.voicePackDef.recruitSound.PlaySound();

            //档案系统
            GameComp_OperatorDocumentation.AddPawn(this.OperatorID, this, operator_Pawn, weapon, fashionSet);
            fashionSet.Clear();
            hediff.document = GameComp_OperatorDocumentation.opDocArchive[this.OperatorID];
            hediff.document.voicePack = this.voicePackDef;
            //hediff.document.operatorDef = this;

            Recruit_Ability(hediff);

            currentlyGenerating = false;
        }

        public virtual void Recruit(Map map)
        {
            currentlyGenerating = true;
            IntVec3 intVec;
            if (map != null)
            {
                if (RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, 0.2f, false, null))
                {
                    Recruit(intVec, map);
                }
            }
            currentlyGenerating = false;
        }

        public virtual void AutoFill()
        {
            //默认名字
            AutoFill_Name();

            //默认体型
            //if (this.bodyTypeDef == null) this.bodyTypeDef = BodyTypeDefOf.Thin;

            //默认武器
            AutoFill_Weapon();

            //默认衣服
            AutoFill_Apparel();

            //默认发型，没有也无所谓，生成时有另一个默认值
            if (this.hair == null)
            {
                this.hair = DefDatabase<HairDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(this.defName, "Hair"));
            }

            //默认头像和立绘
            AutoFill_StandPortrait();

            //默认语音
            AutoFill_VoicePack();

            AutoFill_BackStory();

            AutoFill_Age();
        }

#region RecruitSubMethods
        protected static Pawn operator_Pawn;
        protected void Recruit_Ability(Hediff_Operator hediff)
        {
            //绑定干员技能
            if (this.abilities != null && this.abilities.Count > 0)
            {

                foreach (OperatorAbilityDef i in this.abilities)
                {
                    HC_Ability HC = new HC_Ability(i);
                    hediff.comps.Add(HC);
                    HC.parent = hediff;
                    if (i.grouped) hediff.document.groupedAbilities.Add(HC);
                }
            }
            //禁用非第一个的可选技能
            for (int i = 1; i < hediff.document.groupedAbilities.Count; ++i)
            {
                hediff.document.groupedAbilities[i].enabled = false;
            }
        }
        protected void Recruit_AddRelations()
        {
            operator_Pawn.relations.ClearAllRelations();
            if (this.relations == null || this.relations.Count == 0) return;
            foreach (KeyValuePair<string, PawnRelationDef> node in relations)
            {
                if (GameComp_OperatorDocumentation.opDocArchive.ContainsKey(node.Key))
                {
                    operator_Pawn.relations.AddDirectRelation(node.Value, GameComp_OperatorDocumentation.opDocArchive[node.Key].pawn);
                }
            }
        }

        protected Hediff_Operator Recruit_Hediff()
        {
            operator_Pawn.health.hediffSet.Clear();
            foreach (Hediff hediff_Pawn in operator_Pawn.health.hediffSet.hediffs)
            {
                if (hediff_Pawn.def.isBad)
                {
                    operator_Pawn.health.RemoveHediff(hediff_Pawn);
                }
            }

            HediffDef hediffDef = HediffDef.Named("AK_Operator");
            FixAlienHairColor(hediffDef);
            
            Hediff_Operator hediff = HediffMaker.MakeHediff(hediffDef, operator_Pawn, operator_Pawn.health.hediffSet.GetBrain()) as Hediff_Operator;

            //增加多功能hediff
            operator_Pawn.health.AddHediff(hediff, null, null, null);

            if (this.hediffInate != null && this.hediffInate.Count > 0)
            {
                foreach (HediffStat i in this.hediffInate)
                {
                    AbilityEffect_AddHediff.AddHediff(operator_Pawn, i.hediff, i.part, severity : i.serverity );
                }
            }
            return hediff;
        }
        protected void FixAlienHairColor(HediffDef hediffDef)
        {
            //修复外星人会改发色的问题
            if (ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null)
            {
                HCP_ForceColors comp = new HCP_ForceColors
                {
                    skinColor = this.skinColor,
                    hairColor = this.hairColor
                };
                if (hediffDef.comps == null) hediffDef.comps = new List<HediffCompProperties>();
                if (!hediffDef.comps.Contains(comp)) hediffDef.comps.Add(comp);
            }
        }

        protected void Recruit_PersonalStat()
        {
            //避免Bug更改
            operator_Pawn.needs.food.CurLevel = operator_Pawn.needs.food.MaxLevel;

            operator_Pawn.Name = new NameTriple(this.name, this.nickname, this.surname);//“名”“简”“姓”

            //性别更改
            operator_Pawn.gender = (this.isMale) ? Gender.Male : Gender.Female;
            operator_Pawn.ageTracker.AgeBiologicalTicks = this.age * (long)TimeToTick.year;
            operator_Pawn.ageTracker.AgeChronologicalTicks = this.realAge * (long)TimeToTick.year;

            //发型与体型设置
            operator_Pawn.story.bodyType = this.bodyTypeDef;
            operator_Pawn.story.headType = this.headTypeDef ?? DefDatabase<HeadTypeDef>.GetNamed("Female_NarrowPointy");
            operator_Pawn.story.hairDef = this.hair == null ? HairDefOf.Bald : this.hair;
            operator_Pawn.style.beardDef = this.beard == null ? BeardDefOf.NoBeard : this.beard;
            operator_Pawn.story.skinColorOverride = this.skinColor;
            operator_Pawn.story.HairColor = this.hairColor;

            operator_Pawn.story.traits.allTraits.Clear();
            foreach (TraitAndDegree TraitAndDegree in this.traits)
            {
                operator_Pawn.story.traits.GainTrait(new Trait(TraitAndDegree.def, TraitAndDegree.degree));
            }
            //特性更改
            operator_Pawn.story.Childhood = this.childHood;
            operator_Pawn.story.Adulthood = this.age < 20 ? null : this.adultHood;
            //背景设置

            //清除因为自动生成的故事和特性导致的，某些工作被禁用的缓存
            operator_Pawn.Notify_DisabledWorkTypesChanged();

            operator_Pawn.skills.skills.Clear();
            foreach (SkillAndFire skillDef in this.skills)
            {
                SkillRecord skill = new SkillRecord(operator_Pawn, skillDef.skill)
                {
                    passion = skillDef.fireLevel,
                    Level = skillDef.level
                };
                operator_Pawn.skills.skills.Add(skill);
            }
            //技能属性更改
            if (GameComp_OperatorDocumentation.opDocArchive.ContainsKey(AK_Tool.GetOperatorIDFrom(this.defName)))
            {
                Dictionary<SkillDef, int> skills = GameComp_OperatorDocumentation.opDocArchive[AK_Tool.GetOperatorIDFrom(this.defName)].skillLevel;
                foreach (SkillRecord i in operator_Pawn.skills.skills)
                {
                    i.Level = skills[i.def];
                }
            }
            //从干员文档更新属性
        }
        protected ThingWithComps Recruit_Inventory()
        {
            operator_Pawn.inventoryStock.stockEntries.Clear();
            //增加物品
            if (this.items != null && this.items.Count > 0)
            {
                foreach (ItemOnSpawn i in this.items)
                {
                    Thing newThing = ThingMaker.MakeThing(i.item);
                    newThing.stackCount = i.amount;
                    operator_Pawn.inventory.TryAddAndUnforbid(newThing);
                }
            }
            //装备衣物和配件
            operator_Pawn.apparel.DestroyAll();
            fashionSet = new List<Thing>();
            if (apparels != null)
            {
                foreach (ThingDef apparelDef in this.apparels)
                {
                    Recruit_Inventory_Wear(apparelDef, operator_Pawn, true);
                }
            }
            if (accessory != null)
            {
                foreach (ThingDef accDef in this.accessory)
                {
                    Recruit_Inventory_Wear(accDef, operator_Pawn, false);
                }
            }
            //装备武器
            ThingWithComps weapon = null;
            if (this.weapon != null)
            {
                if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null && AK_ModSettings.debugOverride) {
                    return null;
                }
                weapon = (ThingWithComps)ThingMaker.MakeThing(this.weapon);
                CompBiocodable comp = weapon.GetComp<CompBiocodable>();
                if (comp != null) comp.CodeFor(operator_Pawn);
                operator_Pawn.equipment.AddEquipment(weapon);
            }
            return weapon;
        }

        protected void Recruit_Inventory_Wear(ThingDef apparelDef, Pawn p, bool isFashion = true)
        {
            Apparel apparel = (Apparel)ThingMaker.MakeThing(apparelDef);
            CompBiocodable comp = apparel.GetComp<CompBiocodable>();
            if (comp != null) comp.CodeFor(p);
            p.apparel.Wear(apparel, true, false);
            p.outfits.forcedHandler.SetForced(apparel, true);
            if (isFashion)
            {
                fashionSet.Add(apparel);
            }
        }
#endregion

#region AutoFillSubMethods 自动补齐相关方法
        private void AutoFill_Name()
        {
            if (this.name == null && this.nickname == null)
            {
                Log.Error($"{this.defName}缺乏任何形式的名字。");
                this.nickname = this.name = AK_Tool.GetOperatorIDFrom(this.defName);
            }
            else if (this.name == null && this.nickname != null) this.name = this.nickname;
            else if (this.name != null && this.nickname == null) this.nickname = this.name;

            if (this.surname == null)
            {
                this.surname = "Rhodes";
                Log.Error(this.nickname + "缺乏姓。");
            }
        }
        private void AutoFill_Weapon()
        {
            if (this.weapon == null)
            {
                this.weapon = DefDatabase<ThingDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(this.defName, "Weapon"));
            }
        }
        private void AutoFill_VoicePack()
        {
            if (this.voicePackDef == null)
            {
                this.voicePackDef = DefDatabase<VoicePackDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(this.defName, "VoicePack"));
                if (this.voicePackDef == null)
                {
                    this.voicePackDef = new VoicePackDef(this.Prefix, this.OperatorID);
                    return;
                }
            }
            this.voicePackDef.AutoFillVoicePack(this.Prefix, this.OperatorID);
        }
        private void AutoFill_StandPortrait()
        {
            string standPath;
            string temp;
            //FIXME 这写的啥玩意儿？记得把try去掉
            if (this.headPortrait == null)
            {
                string portraitPath = "UI/Image/" + this.operatorType.textureFolder + "/" + AK_Tool.GetOperatorIDFrom(this.defName) + "Portrait";
                this.headPortrait = portraitPath;
                try
                {
                    ContentFinder<Texture2D>.Get(this.headPortrait);
                }
                catch
                {
                    Log.Error($"{this.nickname}缺乏头像。");
                    this.headPortrait = "UI/Image/Caster/DuskPortrait";
                }
            }

            if (this.stand == null)
            {
                standPath = "UI/Image/" + this.operatorType.textureFolder + "/" + AK_Tool.GetOperatorIDFrom(this.defName) + "Stand";
                this.stand = standPath;
                try
                {
                    ContentFinder<Texture2D>.Get(this.stand);
                }
                catch
                {
                    Log.Error($"{this.nickname}缺乏立绘。");
                    this.stand = "UI/Image/Caster/DuskStand";
                }
            }

            if (this.commonStand == null)
            {
                standPath = "UI/Image/" + this.operatorType.textureFolder + "/" + AK_Tool.GetOperatorIDFrom(this.defName) + "Common";
                if (ContentFinder<Texture2D>.Get(standPath, false) != null) this.commonStand = standPath;
                else this.commonStand = null;
            }
            if (fashion == null) fashion = new List<string>();
            standPath = "UI/Image/" + this.operatorType.textureFolder + "/" + AK_Tool.GetOperatorIDFrom(this.defName) + "Fashion";
            if (ContentFinder<Texture2D>.Get(standPath, false) != null) this.fashion.Add(standPath);
            for (int i = 0; i < 10; ++i)
            {
                temp = standPath + TypeDef.romanNumber[i];
                if (ContentFinder<Texture2D>.Get(temp, false) != null) this.fashion.Add(temp);
            }
            AutoFill_Live2D();
        }

        private void AutoFill_Live2D()
        {
            //检查live2D的格式。
            if (ModLister.GetActiveModWithIdentifier("FS.LivelyRim") != null)
            {
                List<string> modelNames = new List<string>();
                if (live2dModel == null)
                {
                    live2dModel = new List<LiveModelDef>();
                    return;
                }
                foreach (LiveModelDef i in live2dModel)
                {
                    if (ModLister.GetActiveModWithIdentifier(i.modID) == null)
                    {
                        Log.Error($"FS.L2D. error with {nickname}'s live2d named {i} : missing mod with ID {i.modID}");
                        continue;
                    }
                    AssetBundle ab = FS_Tool.LoadAssetBundle(i.modID, i.assetBundle);
                    if (ab == null)
                    {
                        Log.Error($"FS.L2D. error with {nickname}'s live2d named {i} : missing assetbundle named {i.assetBundle}");
                        continue;
                    }
                    GameObject modelPrefab = ab.LoadAsset<GameObject>(i.modelName);
                    if (modelPrefab == null)
                    {
                        Log.Error($"FS.L2D. error with {nickname}'s live2d named {i} : missing model named {i.modelName}");
                        continue;
                    }
                }
            }
        }

        private void AutoFill_Apparel()
        {
            if (this.apparels == null) this.apparels = new List<ThingDef>();

            ThingDef tempThing;

            foreach (string thingType in TypeDef.apparelType)
            {
                tempThing = DefDatabase<ThingDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(this.defName, thingType));
                if (tempThing != null && !this.apparels.Contains(tempThing)) this.apparels.Add(tempThing);
            }
        }

        private void AutoFill_BackStory()
        {
            string basement = AK_Tool.GetThingdefNameFrom(this.defName, "BackStory") + "_";
            BackstoryDef bs;
            if (this.childHood.defName == "AK_BackStory_Unknown_Child")
            {
                bs = DefDatabase<BackstoryDef>.GetNamedSilentFail(basement + "Child");
                if (bs != null) this.childHood = bs;
            }
            if (this.adultHood == null && this.age > 19)
            {
                bs = DefDatabase<BackstoryDef>.GetNamedSilentFail(basement + "Adult");
                this.adultHood = bs ?? DefDatabase<BackstoryDef>.GetNamedSilentFail("AK_BackStory_Unknown_Adult");
            }
        }

        private void AutoFill_Age()
        {
            if (this.age <= 0) this.age = 16;
            if (this.realAge <= 0) this.realAge = this.age;
        }
#endregion
    }
}

