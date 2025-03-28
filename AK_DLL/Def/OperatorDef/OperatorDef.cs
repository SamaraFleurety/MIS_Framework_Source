﻿using AK_TypeDef;
using AKA_Ability;
using AKM_MusicPlayer;
using FS_LivelyRim;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

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

        //public List<OperatorAbilityDef> abilities = new List<OperatorAbilityDef>();
        public List<AKAbilityDef> AKAbilities = new List<AKAbilityDef>(); //技能

        public int age = 16;//年龄
        public int realAge = -1; //实际年龄
        public bool isMale = false;//性别 谁jb把这玩意写成bool的
        public List<HediffStat> hediffInate = new List<HediffStat>(); //天生自带hediff 源石病之类的

        public VoicePackDef voicePackDef = null;

        public Dictionary<string, PawnRelationDef> relations;

        public List<TraitAndDegree> traits;//干员特性
        public ThingDef weapon;//干员武器                                                  
        public List<ThingDef> apparels;//干员衣服
        public List<ThingDef> accessory = new(); //干员配件。和衣服的区别是在换装时不会丢掉。适合填入比如护盾。
        public List<ItemOnSpawn> items;
        public BodyTypeDef bodyTypeDef;//干员的体型
        public HeadTypeDef headTypeDef;
        private List<SkillAndFire> skills;//技能列表；要是哪天排序卡得不行就给改成树
        private bool skillSorted = false;
        public Color skinColor = new Color(1, 1, 1, 1); /*= PawnSkinColors.GetSkinColor(0.5f)*///皮肤颜色
        public Color hairColor = new Color(1, 1, 1, 1); /*= PawnSkinColors.GetSkinColor(1f)*///头发颜色
        public HairDef hair;//头发类型
        public BeardDef beard;//胡须

        public OperatorClassDef operatorType;//干员类型/职业

        [MayRequireBiotech]
        public string stand;//精2立绘
        public string commonStand;  //精0立绘
        public List<string> fashion; //换装立绘的路径 和小人身上的衣服无关
        public List<string> fashionAnimation = new();//spine2d动态立绘皮肤的defName列表

        //换装后，体现在rw小人服装上的变化。key的int是换装在List<string> fashion中的下标+3。
        //按理说应该和上面的干员衣服整合一起，但现在已经几百个干员了，要整合工作量太大。立项的时候没考虑做换装。
        public List<OperatorFashionSetDef> clothSets = new List<OperatorFashionSetDef>();
        public List<string> live2dModel = new();

        //因为并不知道是否有某种立绘，所以用字典存。约定-1为头像，0是精0立绘，1是精2立绘，2-后面是换装
        //这里的V3，x和y是x轴和y轴的偏移，z其实是缩放
        public Dictionary<int, Vector3> standOffsets = new Dictionary<int, Vector3>();
        public string headPortrait;         //IMGUI主界面选中时 左下角详情栏上面的头像

        public ThoughtDef thoughtReceived = null;  //其他所有人都会给这个干员一个想法 当前是和弦独有
        public int TRStage = -1;  //全部丢进同一个想法 节省性能

        public float ticketCost = 1f;   //招募消耗 为了平衡所以可以是非整数票

        public XenotypeDef xenoType;

        public AbilityDef operatorID = null;    //干员身份证的容器 其他派系角色可以自定义个图标不同的这玩意

        public List<Type> postEffects = new List<Type>();

        public bool alwaysHidden = false;  //是否隐藏。被隐藏的干员不会在招募ui显示，仅能通过特殊手段出现。fixme:没做。

        public Type documentType = typeof(OperatorDocument);   //写了碧蓝之后发现档案甚至都可以扩充

        public bool ignoreAutoFill = false; //跳过自动填充
        #endregion

        #region 快捷属性
        /*//为了兼容性 这玩意不好写成static
        public LiveModelDef Live2DModelDef(string live2dModel)
        {
            return DefDatabase<LiveModelDef>.GetNamed(live2dModel);
        }*/

        public static bool currentlyGenerating = false;

        //缓存 招募时给的衣服。这个时候有可能还没生成doc
        private static List<Thing> clothTemp = new List<Thing>();
        public string Prefix
        {
            get { return AK_Tool.GetPrefixFrom(this.defName); }
        }
        public string OperatorID
        {
            get { return AK_Tool.GetOperatorIDFrom(this.defName); }
        }
        public List<SkillAndFire> SortedSkills
        {
            get
            {
                if (skillSorted) return skills;
                for (int i = 0; i < skills.Count; ++i)
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

        public Texture2D PreferredStand(int preferredSkin)
        {
            Texture2D texture;
            if (preferredSkin == 0) texture = ContentFinder<Texture2D>.Get(this.commonStand);
            else if (preferredSkin == 1) texture = ContentFinder<Texture2D>.Get(this.stand);
            else texture = ContentFinder<Texture2D>.Get(this.fashion[preferredSkin - 2]);
            return texture;
        }

        public bool CurrentRecruited
        {
            get
            {
                OperatorDocument doc = GC_OperatorDocumentation.opDocArchive[this.OperatorID];
                return doc == null || !doc.currentExist;
            }
        }
        #endregion

        //新换装写法 如果给定def就换，如果def是空的就换成原装
        public void ChangeFashion(OperatorFashionSetDef def, Pawn p)
        {
            OperatorDocument doc = p.GetDoc();
            if (doc == null)
            {
                Log.Error($"[AK] trying to change fashion for non-operator: {p.Name}");
                return;
            }

            currentlyGenerating = true;
            doc.DestroyFashionSet();

            operator_Pawn = p;
            clothTemp = new List<Thing>();

            int apparelTextureIndex = -1;
            if (def == null)
            {
                if (apparels != null)
                {
                    foreach (ThingDef apparelDef in this.apparels)
                    {
                        Apparel apparel = Recruit_Inventory_Wear(apparelDef, operator_Pawn, true);
                    }
                }
                operator_Pawn.story.hairDef = hair ?? HairDefOf.Bald;
                doc.voicePack = this.voicePackDef;
                if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null && ModLister.GetActiveModWithIdentifier("paluto22.ak.combatextended") == null)
                {
                    return;
                }
                else if (this.weapon != null)
                {
                    if (doc.weapon != null && !doc.weapon.DestroyedOrNull()) doc.weapon.Destroy();
                    ThingWithComps weapon = (ThingWithComps)ThingMaker.MakeThing(this.weapon);
                    CompBiocodable comp = weapon.GetComp<CompBiocodable>();
                    if (comp != null) comp.CodeFor(operator_Pawn);
                    operator_Pawn.equipment.AddEquipment(weapon);
                    doc.weapon = weapon;
                }
            }
            else
            {
                OperatorFashionSetDef set = def;
                if (set.apparelTextureIndex is int index) apparelTextureIndex = index;
                foreach (ThingDef apparelDef in set.apparels)
                {
                    Apparel apparel = Recruit_Inventory_Wear(apparelDef, operator_Pawn, true);
                }
                if (set.hair != null) operator_Pawn.story.hairDef = set.hair;
                if (set.voice != null)
                {
                    doc.voicePack = set.voice;
                }
                if (set.weapon != null)
                {
                    if (doc.weapon != null && !doc.weapon.DestroyedOrNull()) doc.weapon.Destroy();
                    if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null && ModLister.GetActiveModWithIdentifier("paluto22.ak.combatextended") == null)
                    {
                        return;
                    }
                    ThingWithComps weapon = (ThingWithComps)ThingMaker.MakeThing(set.weapon);
                    CompBiocodable comp = weapon.GetComp<CompBiocodable>();
                    if (comp != null) comp.CodeFor(operator_Pawn);
                    operator_Pawn.equipment.AddEquipment(weapon);
                    doc.weapon = weapon;
                }
            }

            foreach (var apparel in operator_Pawn.apparel.WornApparel)
            {
                Log.Message($"ap {apparel}");
                if (apparel is Apparel_Operator ap)
                {
                    Log.Message($"found ap is {ap}");
                    ap.SetGraphicIndex(apparelTextureIndex);
                }
            }

            operator_Pawn.style.Notify_StyleItemChanged();
            operator_Pawn.style.MakeHairFilth();
            doc.RegisterFashionSet(clothTemp);
            clothTemp.Clear();
            currentlyGenerating = false;
        }

        public virtual Pawn Recruit(IntVec3 intVec, Map map)
        {
            currentlyGenerating = true;

            operator_Pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Colonist, Faction.OfPlayer, forcedXenotype: xenoType));

            Recruit_Hediff();

            Recruit_PersonalStat();

            Recruit_AddRelations();

            //operator_Pawn.story.CrownType = CrownType.Average;

            ThingWithComps weapon = Recruit_Inventory();

            if (ModLister.GetActiveModWithIdentifier("mis.arkmusic") != null) Recruit_ArkSongExtension();


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
            VAbility_Operator operatorID = Recruit_OperatorID(weapon);
            operator_Pawn.AddDoc(new OpDocContainer(operator_Pawn) { va = operatorID });
            clothTemp.Clear();

            Recruit_Ability(operatorID);

            if (!postEffects.NullOrEmpty())
            {
                foreach (Type RPEW in postEffects)
                {
                    RecruitPostEffectWorker_Base worker = (RecruitPostEffectWorker_Base)Activator.CreateInstance(RPEW, this, operator_Pawn);
                    worker?.RecruitPostEffect();
                }
            }

            //GC_OperatorDocumentation.cachedOperators.Add(operator_Pawn, operatorID.Document);

            GenSpawn.Spawn(operator_Pawn, intVec, map);
            CameraJumper.TryJump(new GlobalTargetInfo(intVec, map));


            currentlyGenerating = false;

            return operator_Pawn;
        }

        public virtual Pawn Recruit(Map map)
        {
            Pawn result = null;
            currentlyGenerating = true;
            IntVec3 intVec;
            if (map != null)
            {
                if (RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, 0.2f, false, null))
                {
                    result = Recruit(intVec, map);
                }
            }
            currentlyGenerating = false;

            return result;
        }

        public virtual void AutoFill()
        {
            if (ignoreAutoFill) return;

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

            ignoreAutoFill = true;
        }

        #region RecruitSubMethods
        protected static Pawn operator_Pawn;
        protected void Recruit_Ability(VAbility_Operator vanillaAbility)
        {
            if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null)
            {
                return;
            }
            AbilityTracker tracker = vanillaAbility.AKATracker;
            if (this.AKAbilities != null && this.AKAbilities.Count > 0)
            {
                foreach (AKAbilityDef i in this.AKAbilities)
                {
                    tracker.AddAbility(i);
                    //AKAbilityMaker.MakeAKAbility(i, tracker);
                }
            }
        }
        protected void Recruit_AddRelations()
        {
            operator_Pawn.relations.ClearAllRelations();
            if (this.relations == null || this.relations.Count == 0) return;
            foreach (KeyValuePair<string, PawnRelationDef> node in relations)
            {
                if (GC_OperatorDocumentation.opDocArchive.ContainsKey(node.Key))
                {
                    operator_Pawn.relations.AddDirectRelation(node.Value, GC_OperatorDocumentation.opDocArchive[node.Key].pawn);
                }
            }
        }

        protected VAbility_Operator Recruit_OperatorID(Thing weapon)
        {
            //档案系统
            GC_OperatorDocumentation.AddPawn(this.OperatorID, this, operator_Pawn, weapon, clothTemp);
            OperatorDocument document = GC_OperatorDocumentation.opDocArchive[this.OperatorID];
            document.voicePack = voicePackDef;

            //干员身份证，改放在原版技能里了
            if (this.operatorID == null) operatorID = AKDefOf.AK_VAbility_Operator;
            VAbility_Operator vAbility = AbilityUtility.MakeAbility(operatorID, operator_Pawn) as VAbility_Operator;
            vAbility.AKATracker = new AK_AbilityTracker
            {
                doc = document,
                owner = operator_Pawn
            };
            operator_Pawn.abilities.abilities.Add(vAbility);
            return vAbility;
        }

        protected void Recruit_Hediff()
        {
            operator_Pawn.health.hediffSet.Clear();
            foreach (Hediff hediff_Pawn in operator_Pawn.health.hediffSet.hediffs)
            {
                if (hediff_Pawn.def.isBad)
                {
                    operator_Pawn.health.RemoveHediff(hediff_Pawn);
                }
            }

            //外星人发色兼容

            if (ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null)
            {
                HediffWithComps hediffAlienPatch = HediffMaker.MakeHediff(AKDefOf.AK_Hediff_AlienRacePatch, operator_Pawn, operator_Pawn.health.hediffSet.GetBrain()) as HediffWithComps;
                HC_ForceColors comp = hediffAlienPatch.TryGetComp<HC_ForceColors>();
                comp.exactProps.skinColor = this.skinColor;
                comp.exactProps.hairColor = this.hairColor;
            }

            if (this.hediffInate != null && this.hediffInate.Count > 0)
            {
                foreach (HediffStat i in this.hediffInate)
                {
                    AbilityEffect_AddHediff.AddHediff(operator_Pawn, i.hediff, i.part, customLabel: i.partCustomLabel, severity: i.serverity);
                }
            }
            return;
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
            operator_Pawn.story.hairDef = hair == null ? HairDefOf.Bald : hair;
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
            if (GC_OperatorDocumentation.opDocArchive.ContainsKey(AK_Tool.GetOperatorIDFrom(this.defName)))
            {
                Dictionary<SkillDef, int> skills = GC_OperatorDocumentation.opDocArchive[AK_Tool.GetOperatorIDFrom(this.defName)].skillLevel;
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
                    Recruit_Inventory_Additem(i.item, i.amount);
                    /*Thing newThing = ThingMaker.MakeThing(i.item);
                    newThing.stackCount = i.amount;
                    operator_Pawn.inventory.TryAddAndUnforbid(newThing);*/
                }
            }
            //装备衣物和配件
            operator_Pawn.apparel.DestroyAll();
            clothTemp = new List<Thing>();
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
                if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null && ModLister.GetActiveModWithIdentifier("paluto22.ak.combatextended") == null)
                {
                    return null;
                }
                weapon = (ThingWithComps)ThingMaker.MakeThing(this.weapon);
                CompBiocodable comp = weapon.GetComp<CompBiocodable>();
                if (comp != null) comp.CodeFor(operator_Pawn);
                operator_Pawn.equipment.AddEquipment(weapon);
            }
            return weapon;
        }

        protected Apparel Recruit_Inventory_Wear(ThingDef apparelDef, Pawn p, bool isFashion = true)
        {
            Apparel apparel = (Apparel)ThingMaker.MakeThing(apparelDef);
            CompBiocodable comp = apparel.GetComp<CompBiocodable>();
            comp?.CodeFor(p);
            p.apparel.Wear(apparel, true, false);
            p.outfits.forcedHandler.SetForced(apparel, true);
            if (isFashion)
            {
                clothTemp.Add(apparel);
            }
            return apparel;
        }

        protected Thing Recruit_Inventory_Additem(ThingDef itemDef, int cnt = 1)
        {
            Thing t = ThingMaker.MakeThing(itemDef);
            t.stackCount = cnt;
            operator_Pawn.inventory.TryAddAndUnforbid(t);
            return t;
        }

        protected void Recruit_ArkSongExtension()
        {
            DefExt_ArkSong ext = this.GetModExtension<DefExt_ArkSong>();
            if (ext == null) return;
            ThingDef recordDef = DefDatabase<ThingDef>.GetNamed("AKM_Item_Record");
            if (ext.arkSong != null)
            {
                ThingClass_MusicRecord t = Recruit_Inventory_Additem(recordDef, 1) as ThingClass_MusicRecord;
                t.recordedSong = ext.arkSong;
            }
            foreach (ArkSongDef i in ext.arkSongs)
            {
                ThingClass_MusicRecord t = Recruit_Inventory_Additem(recordDef, 1) as ThingClass_MusicRecord;
                t.recordedSong = i;
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
            if (AK_Tool.Live2DActivated) AutoFill_Live2D();
        }

        private void AutoFill_Live2D()
        {
            //检查live2D的格式。
            if (ModLister.GetActiveModWithIdentifier("FS.LivelyRim") == null) return;
            foreach (string j in live2dModel)
            {
                FS_Utilities.VerifyL2DDefname(nickname, j);
            }
        }

        private void AutoFill_Apparel()
        {
            if (this.apparels == null) this.apparels = new List<ThingDef>();

            ThingDef tempThing;

            foreach (string thingType in TypeDef.apparelType)
            {
                tempThing = DefDatabase<ThingDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(this.defName, thingType));
                if (tempThing != null && !this.apparels.Contains(tempThing) && !accessory.Contains(tempThing)) this.apparels.Add(tempThing);
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