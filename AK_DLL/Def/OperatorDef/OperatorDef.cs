using AK_DLL.DynamicLoading;
using AK_TypeDef;
using AKA_Ability;
using AKA_Ability.TickCondition;
using AKM_MusicPlayer;
using FS_LivelyRim;
using RimWorld;
using RimWorld.Planet;
using SpriteEvo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        public List<AKAbilityDef> AKAbilities = new(); //技能

        public int age = 16;//年龄
        public int realAge = -1; //实际年龄
        public bool isMale = false;//性别 谁jb把这玩意写成bool的
        public List<HediffStat> hediffInate = new(); //天生自带hediff 源石病之类的

        public VoicePackDef voicePackDef = null;

        public Dictionary<string, PawnRelationDef> relations;

        public List<TraitAndDegree> traits;//干员特性
        public ThingDef weapon = null;//干员武器                                                  
        public List<ItemOnSpawn> apparels = new();//干员衣服
        public List<ThingDef> accessory = new(); //干员配件。和衣服的区别是在换装时不会丢掉。适合填入比如护盾。
        public List<ItemOnSpawn> items;
        public BodyTypeDef bodyTypeDef;//干员的体型
        public HeadTypeDef headTypeDef;
        public List<SkillAndFire> skills;//技能列表；要是哪天排序卡得不行就给改成树
        private bool skillSorted = false;
        public Color skinColor = new(1, 1, 1, 1); //皮肤颜色
        public Color hairColor = new(1, 1, 1, 1); //头发颜色
        public HairDef hair;//头发类型
        public BeardDef beard;//胡须

        public OperatorClassDef operatorType;//干员类型/职业

        public string stand;//精2立绘
        public string commonStand = null;  //精0立绘
        public List<string> fashion; //换装立绘的路径 和小人身上的衣服无关

        //新式的静态立绘列表，支持动态加载。其中，-1是头像，0是精2，1是精0，2~1000是静态换装。
        //和原版默认根目录在Textures\里面类似，这里面的根目录是DynaLoad\Textures
        //虽然看似可以，但是不要跳着写换装，ui会报错
        public Dictionary<int, string> staticStands = new();
        //标记是否启用动态加载。如果不启用会沿用旧式开局完全加载。
        public bool dynaLoadStaticStands = false;

        public string animation;
        public List<string> fashionAnimation = new();//spine2d动态立绘皮肤的defName列表

        //换装后，体现在rw小人服装上的变化。key的int是换装在List<string> fashion中的下标+3。
        //按理说应该和上面的干员衣服整合一起，但现在已经几百个干员了，要整合工作量太大。立项的时候没考虑做换装。
        public List<OperatorFashionSetDef> clothSets = new();
        public List<string> live2dModel = new();

        //因为并不知道是否有某种立绘，所以用字典存。约定-1为头像，0是精0立绘，1是精2立绘，2-后面是换装
        //这里的V3，x和y是x轴和y轴的偏移，z其实是缩放
        public Dictionary<int, Vector3> standOffsets = new();
        public string headPortrait;         //IMGUI主界面选中时 左下角详情栏上面的头像

        public ThoughtDef thoughtReceived = null;  //其他所有人都会给这个干员一个想法 当前是和弦独有
        public int TRStage = -1;  //全部丢进同一个想法 节省性能

        public float ticketCost = 1f;   //招募消耗 为了平衡所以可以是非整数票

        public XenotypeDef xenoType;

        public AbilityDef operatorID = null;    //干员身份证的容器 其他派系角色可以自定义个图标不同的这玩意

        public List<Type> postEffects = new();

        public bool alwaysHidden = false;  //是否隐藏。被隐藏的干员不会在招募ui显示，仅能通过特殊手段出现
        public bool alwaysHideStand = false; //是否在任何情况下都不显示左下立绘

        public Type documentType = typeof(OperatorDocument);   //写了碧蓝之后发现档案甚至都可以扩充

        public bool ignoreAutoFill = false; //跳过自动填充

        //如果不为空，获取render tree时会使用这个值
        public PawnRenderTreeDef renderTreeOverride = null;

        public string modPackageID = "MIS.Arknights";  //about.xml里面的package id，用于动态加载资源

        public bool forceDisableNL = false;
        #endregion

        //年龄阈值，小于此年龄就无成人故事。
        protected const int BackstoryAdultAgeThreshold = 20;
        #region 快捷属性

        public static bool currentlyGenerating = false;

        //缓存 招募时给的衣服。这个时候有可能还没生成doc
        protected static List<Thing> clothTemp = new();
        public string Prefix => AK_Tool.GetPrefixFrom(this.defName);

        public string OperatorID => AK_Tool.GetOperatorIDFrom(this.defName);

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
            if (dynaLoadStaticStands)
            {
                //Log.Message($"[AK] getting {label} {preferredSkin}号立绘于:")
                staticStands.TryGetValue(preferredSkin, out string path);
                if (path == null)
                {
                    Log.Error($"[AK] {label} 不存在 {preferredSkin} 号立绘");
                    staticStands.TryGetValue(0, out path);
                }

                return Utilities_Unity.GetDynamicLoadTexture(path, modPackageID, true);
            }
            else
            {
                if (preferredSkin == 1)
                {
                    if (commonStand == null)
                    {
                        Log.Error($"[AK] 尝试获取，但是{label}不存在精0立绘");
                        texture = ContentFinder<Texture2D>.Get(this.stand);
                        return texture;
                    }
                    texture = ContentFinder<Texture2D>.Get(this.commonStand);
                }
                else if (preferredSkin == 0) texture = ContentFinder<Texture2D>.Get(this.stand);
                else
                {
                    int skinIndex = preferredSkin - 2;
                    if (fashion.NullOrEmpty() || skinIndex >= fashion.Count)
                    {
                        Log.Error($"[AK] 尝试获取，但是{label}不存在{skinIndex}号换装");
                        texture = ContentFinder<Texture2D>.Get(this.stand);
                        return texture;
                    }
                    texture = ContentFinder<Texture2D>.Get(this.fashion[skinIndex]);
                }
            }

            return texture;
        }

        public bool CurrentRecruited
        {
            get
            {
                OperatorDocument doc = GC_OperatorDocumentation.opDocArchive[this.OperatorID];
                return doc is not { currentExist: true };
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
            if (def == null)  //换回初始换装
            {
                foreach (ItemOnSpawn apparelDef in this.apparels)
                {
                    _ = Recruit_Inventory_Wear(apparelDef.item, operator_Pawn, true, apparelDef.stuff);
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
                    comp?.CodeFor(operator_Pawn);
                    operator_Pawn.equipment.AddEquipment(weapon);
                    doc.weapon = weapon;
                }
                doc.forceDisableNL = this.forceDisableNL;
            }
            else
            {
                if (def.apparelTextureIndex is { } textureIndex) apparelTextureIndex = textureIndex;
                foreach (ThingDef apparelDef in def.apparels)
                {
                    Recruit_Inventory_Wear(apparelDef, operator_Pawn, true);
                }
                if (def.hair != null) operator_Pawn.story.hairDef = def.hair;
                if (def.voice != null)
                {
                    doc.voicePack = def.voice;
                }
                if (def.weapon != null)
                {
                    if (doc.weapon != null && !doc.weapon.DestroyedOrNull()) doc.weapon.Destroy();
                    if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null && ModLister.GetActiveModWithIdentifier("paluto22.ak.combatextended") == null)
                    {
                        return;
                    }
                    ThingWithComps weaponEq = (ThingWithComps)ThingMaker.MakeThing(def.weapon);
                    CompBiocodable comp = weaponEq.GetComp<CompBiocodable>();
                    comp?.CodeFor(operator_Pawn);
                    operator_Pawn.equipment.AddEquipment(weaponEq);
                    doc.weapon = weaponEq;
                }
                doc.forceDisableNL = def.forceDisableNL;
            }

            foreach (Apparel apparel in operator_Pawn.apparel.WornApparel)
            {
                if (apparel is Apparel_Operator ap)
                {
                    ap.SetGraphicIndex(apparelTextureIndex);
                }
            }

            operator_Pawn.style.Notify_StyleItemChanged();
            operator_Pawn.style.MakeHairFilth();
            doc.RegisterFashionSet(clothTemp);
            clothTemp.Clear();
            currentlyGenerating = false;
        }

        public virtual Pawn Recruit_NoMap()
        {
            currentlyGenerating = true;

            ForceLoadResources();
            operator_Pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Colonist, Faction.OfPlayer, forcedXenotype: xenoType));

            Recruit_Hediff();

            Recruit_PersonalStat();

            Recruit_AddRelations();

            operator_Pawn.apparel?.DestroyAll();
            Recruit_Inventory();

            if (ModLister.GetActiveModWithIdentifier("mis.arkmusic") != null) Recruit_ArkSongExtension();

            //基因
            if (ModLister.BiotechInstalled)
            {
                operator_Pawn.genes.ClearXenogenes();
                //operator_Pawn.genes = new Pawn_GeneTracker(operator_Pawn);
                //operator_Pawn.genes.SetXenotype(DefDatabase<XenotypeDef>.GetNamed("AK_BaseType"));
            }
            //播放语音
            this.voicePackDef?.recruitSound.PlaySound();

            //档案系统
            VAbility_Operator operatorID = Recruit_VAB() as VAbility_Operator;

            //对vab容器进行aka技能以外的处理
            Recruit_OperatorID(operatorID);
            //(operatorID.AKATracker as AK_AbilityTracker).doc = doc;
            clothTemp.Clear();

            Recruit_AKAbility(operatorID);

            Recruit_PostEffects();

            if (ModLister.GetActiveModWithIdentifier("Paluto22.SpriteEvo") != null) Recruit_SpineModel();
            currentlyGenerating = false;
            return operator_Pawn;
        }

        public virtual Pawn Recruit(IntVec3 intVec, Map map)
        {
            Recruit_NoMap();
            GenSpawn.Spawn(operator_Pawn, intVec, map);
            CameraJumper.TryJump(new GlobalTargetInfo(intVec, map));

            return operator_Pawn;
        }

        public virtual Pawn Recruit(Map map)
        {
            Pawn result = null;
            currentlyGenerating = true;
            if (map != null)
            {
                if (RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 intVec, map, 0.2f, false, null))
                {
                    result = Recruit(intVec, map);
                }
            }
            else
            {
                Log.Error($"[AK]尝试招募干员，但是交互招募台不在任何地图上");
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
            this.hair ??= DefDatabase<HairDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(this.defName, "Hair"));

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

        protected void Recruit_PostEffects()
        {
            if (!postEffects.NullOrEmpty())
            {
                foreach (Type RPEW in postEffects)
                {
                    RecruitPostEffectWorker_Base worker = (RecruitPostEffectWorker_Base)Activator.CreateInstance(RPEW, this, operator_Pawn);
                    worker?.RecruitPostEffect();
                }
            }
        }
        //原版技能，用作舟技能的容器。可以使用不同的def但是必须是这个类型的子类。如果技能def是null那就不适用舟技能。
        //不提供别的原版技能支持。
        protected virtual VAbility_AKATrackerContainer Recruit_VAB()
        {
            if (operatorID == null) return null;
            VAbility_AKATrackerContainer vAbility = AbilityUtility.MakeAbility(operatorID, operator_Pawn) as VAbility_AKATrackerContainer;
            operator_Pawn.abilities.abilities.Add(vAbility);
            return vAbility;
        }

        private OperatorDocument Recruit_Document(Thing weapon)
        {
            GC_OperatorDocumentation.AddPawn(this.OperatorID, this, operator_Pawn, weapon, clothTemp);
            OperatorDocument document = GC_OperatorDocumentation.opDocArchive[this.OperatorID];
            document.voicePack = voicePackDef;
            return document;
        }
        protected void Recruit_AKAbility(VAbility_AKATrackerContainer vanillaAbility)
        {
            if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null || vanillaAbility == null)
            {
                return;
            }
            AbilityTracker tracker = vanillaAbility.AKATracker;
            if (!this.AKAbilities.NullOrEmpty())
            {
                foreach (AKAbilityDef i in this.AKAbilities)
                {
                    tracker.AddAbility(i);
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

        protected virtual void Recruit_OperatorID(VAbility_AKATrackerContainer vab)
        {
            //干员文档
            OperatorDocument document = Recruit_Document(operator_Pawn.equipment.Primary);
            if (vab is not VAbility_Operator vAbility)
            {
                Log.Error($"[AK] 招募{label}时出错:无有效的身份证容器");
                return;
            }
            vAbility.AKATracker = new AK_AbilityTracker
            {
                doc = document,
                owner = operator_Pawn,
            };
            vAbility.AKATracker.tickCondition = new TickCondion_Base(vAbility.AKATracker);

            //这不是干员文档。这是通用的全局注册系统。这是用来调用干员文档，而非直接存储干员数据。
            operator_Pawn.AddDoc(new OpDocContainer(operator_Pawn) { va = vAbility });
        }

        protected void Recruit_Hediff()
        {
            operator_Pawn.health.hediffSet.Clear();
            foreach (Hediff hediffPawn in operator_Pawn.health.hediffSet.hediffs)
            {
                if (hediffPawn.def.isBad)
                {
                    operator_Pawn.health.RemoveHediff(hediffPawn);
                }
            }

            //外星人发色兼容

            FixAlienHairColor();

            if (this.hediffInate is { Count: > 0 })
            {
                foreach (HediffStat i in this.hediffInate)
                {
                    AbilityEffect_AddHediff.AddHediff(operator_Pawn, i.hediff, i.part, customLabel: i.partCustomLabel, severity: i.serverity);
                }
            }
        }

        protected virtual void FixAlienHairColor()
        {
            if (ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null)
            {
                HediffWithComps hediffAlienPatch = HediffMaker.MakeHediff(AKDefOf.AK_Hediff_AlienRacePatch, operator_Pawn, operator_Pawn.health.hediffSet.GetBrain()) as HediffWithComps;
                HC_ForceColors comp = hediffAlienPatch.TryGetComp<HC_ForceColors>();
                comp.exactProps.skinColor = this.skinColor;
                comp.exactProps.hairColor = this.hairColor;
            }
            /*/修复外星人会改发色的问题
            if (ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null)
            {
                HCP_ForceColors comp = new HCP_ForceColors
                {
                    skinColor = this.skinColor,
                    hairColor = this.hairColor
                };
                if (hediffDef.comps == null) hediffDef.comps = new List<HediffCompProperties>();
                if (!hediffDef.comps.Contains(comp)) hediffDef.comps.Add(comp);
            }*/
        }

        protected virtual void Recruit_PersonalStat()
        {
            //避免Bug更改
            operator_Pawn.needs.food.CurLevel = operator_Pawn.needs.food.MaxLevel;

            operator_Pawn.Name = new NameTriple(this.name, this.nickname, this.surname);//“名”“简”“姓”

            //性别更改
            operator_Pawn.gender = this.isMale ? Gender.Male : Gender.Female;
            operator_Pawn.ageTracker.AgeBiologicalTicks = this.age * (long)TimeToTick.year;
            operator_Pawn.ageTracker.AgeChronologicalTicks = this.realAge * (long)TimeToTick.year;

            //发型与体型设置
            operator_Pawn.story.bodyType = this.bodyTypeDef;
            operator_Pawn.story.headType = this.headTypeDef ?? DefDatabase<HeadTypeDef>.GetNamed("Female_NarrowPointy");
            operator_Pawn.story.hairDef = hair ?? HairDefOf.Bald;
            operator_Pawn.style.beardDef = this.beard ?? BeardDefOf.NoBeard;
            operator_Pawn.story.skinColorOverride = this.skinColor;
            operator_Pawn.story.HairColor = this.hairColor;

            //特性更改
            operator_Pawn.story.traits.allTraits.Clear();
            foreach (TraitAndDegree traitAndDegree in this.traits)
            {
                operator_Pawn.story.traits.GainTrait(new Trait(traitAndDegree.def, traitAndDegree.degree));
            }
            operator_Pawn.story.Childhood = childHood;
            operator_Pawn.story.Adulthood = this.age < BackstoryAdultAgeThreshold ? null : adultHood;
            //背景设置

            //清除因为自动生成的故事和特性导致的，某些工作被禁用的缓存
            operator_Pawn.Notify_DisabledWorkTypesChanged();

            operator_Pawn.skills.skills.Clear();
            foreach (SkillAndFire skillDef in this.skills)
            {
                SkillRecord skill = new(operator_Pawn, skillDef.skill)
                {
                    passion = skillDef.fireLevel,
                    Level = skillDef.level
                };
                operator_Pawn.skills.skills.Add(skill);
            }
            //技能属性更改
            if (GC_OperatorDocumentation.opDocArchive.ContainsKey(AK_Tool.GetOperatorIDFrom(this.defName)))
            {
                Dictionary<SkillDef, int> skillLevels = GC_OperatorDocumentation.opDocArchive[AK_Tool.GetOperatorIDFrom(this.defName)].skillLevel;
                foreach (SkillRecord i in operator_Pawn.skills.skills)
                {
                    i.Level = skillLevels[i.def];
                }
            }
            //从干员文档更新属性
        }

        protected void Recruit_Inventory()
        {
            operator_Pawn.inventoryStock ??= new Pawn_InventoryStockTracker();
            operator_Pawn.inventoryStock.stockEntries.Clear();
            //增加物品
            if (this.items is { Count: > 0 })
            {
                foreach (ItemOnSpawn i in this.items)
                {
                    Recruit_Inventory_Additem(i.item, i.amount);
                }
            }
            //装备衣物和配件
            //operator_Pawn.apparel.DestroyAll();
            clothTemp = new List<Thing>();
            if (apparels != null)
            {
                foreach (ItemOnSpawn apparelDef in this.apparels)
                {
                    Recruit_Inventory_Wear(apparelDef.item, operator_Pawn, true, apparelDef.stuff);
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
            if (this.weapon != null)
            {
                if (ModLister.GetActiveModWithIdentifier("ceteam.combatextended") != null && ModLister.GetActiveModWithIdentifier("paluto22.ak.combatextended") == null)
                {
                    return;
                }
                ThingWithComps weaponEq = (ThingWithComps)ThingMaker.MakeThing(this.weapon);
                CompBiocodable comp = weaponEq.GetComp<CompBiocodable>();
                comp?.CodeFor(operator_Pawn);
                operator_Pawn.equipment.AddEquipment(weaponEq);
            }
        }

        protected Apparel Recruit_Inventory_Wear(ThingDef apparelDef, Pawn p, bool isFashion = true, ThingDef stuff = null)
        {
            Apparel apparel = (Apparel)ThingMaker.MakeThing(apparelDef, stuff);
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
                if (Recruit_Inventory_Additem(recordDef, 1) is ThingClass_MusicRecord record) record.recordedSong = ext.arkSong;
            }
            foreach (ArkSongDef i in ext.arkSongs)
            {
                if (Recruit_Inventory_Additem(recordDef, 1) is ThingClass_MusicRecord record) record.recordedSong = i;
            }
        }

        protected void Recruit_SpineModel()
        {
            if (animation == null) return;

            MethodInfo method = typeof(AnimationManager).GetMethod("RegisterPawnAnimation", BindingFlags.Public | BindingFlags.Static);
            GameObject spine = (GameObject)method.Invoke(null, new object[] { animation, operator_Pawn });
            spine.SetActive(true);
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
            this.weapon ??= DefDatabase<ThingDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(this.defName, "Weapon"));
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
            string temp;

            string portraitPath = "UI/Image/" + this.operatorType.textureFolder + "/" + AK_Tool.GetOperatorIDFrom(this.defName) + "Portrait";
            string standPath = "UI/Image/" + this.operatorType.textureFolder + "/" + AK_Tool.GetOperatorIDFrom(this.defName) + "Stand";
            if (!dynaLoadStaticStands)
            {
                if (this.headPortrait == null)
                {
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
                fashion ??= new List<string>();
                standPath = "UI/Image/" + this.operatorType.textureFolder + "/" + AK_Tool.GetOperatorIDFrom(this.defName) + "Fashion";
                if (ContentFinder<Texture2D>.Get(standPath, false) != null) this.fashion.Add(standPath);
                for (int i = 0; i < 10; ++i)
                {
                    temp = standPath + TypeDef.romanNumber[i];
                    if (ContentFinder<Texture2D>.Get(temp, false) != null) this.fashion.Add(temp);
                }
            }
            else  //新版动态加载立绘
            {
                if (!staticStands.ContainsKey(-1))
                {
                    temp = Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(portraitPath, modPackageID);
                    if (File.Exists(temp))
                    {
                        //string portrait = temp;
                        staticStands.Add(-1, portraitPath);
                    }
                    else
                    {
                        Log.Error($"[AK] {this.nickname}缺乏头像。");
                        staticStands.Add(-1, "UI/Image/Caster/DuskPortrait");
                    }
                }
                if (!staticStands.ContainsKey(0))
                {
                    temp = Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(standPath, modPackageID);
                    if (File.Exists(temp))
                    {
                        string stand = standPath;
                        staticStands.Add(0, stand);
                    }
                    else
                    {
                        Log.Error($"[AK] {this.nickname}缺乏默认立绘。");
                        staticStands.Add(0, "UI/Image/Caster/DuskStand");
                    }
                }
                if (!staticStands.ContainsKey(1))
                {
                    standPath = "UI/Image/" + this.operatorType.textureFolder + "/" + AK_Tool.GetOperatorIDFrom(this.defName) + "Common";
                    temp = Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(standPath, modPackageID);
                    if (File.Exists(temp))
                    {
                        string standCommon = standPath;
                        staticStands.Add(1, standCommon);
                    }
                }
                standPath = "UI/Image/" + this.operatorType.textureFolder + "/" + AK_Tool.GetOperatorIDFrom(this.defName) + "Fashion";
                for (int i = 0; i < 10; ++i)
                {
                    int fashionIndex = i + 2;
                    temp = standPath + TypeDef.romanNumber[i];
                    string temp2 = Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(temp, modPackageID);
                    if (!staticStands.ContainsKey(fashionIndex) && File.Exists(temp2))
                    {
                        string temp3 = temp;
                        staticStands.Add(fashionIndex, temp3);
                    }
                }
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
            ThingDef tempThing;

            HashSet<string> addedApparals = new();
            foreach (ItemOnSpawn i in new List<ItemOnSpawn>(apparels))
            {
                addedApparals.Add(i.item.defName);
            }

            foreach (string thingType in TypeDef.apparelType)
            {
                tempThing = DefDatabase<ThingDef>.GetNamedSilentFail(AK_Tool.GetThingdefNameFrom(this.defName, thingType));
                if (tempThing != null && !addedApparals.Contains(tempThing.defName) && !accessory.Contains(tempThing)) this.apparels.Add(new ItemOnSpawn() { item = tempThing });
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

        #region 动态加载媒体资源
        //加载这个干员所需的多媒体资源
        public void ForceLoadResources()
        {
            if (!dynaLoadStaticStands) return;
            List<ThingDef> allThingdefs = new();
            allThingdefs.AddRange(accessory);
            //allThingdefs.AddRange(apparels);

            foreach (ItemOnSpawn i in apparels)
            {
                allThingdefs.Add(i.item);
            }

            if (weapon != null) allThingdefs.Add(weapon);
            foreach (ThingDef def in allThingdefs)
            {
                LoadResources_GraphicData(def.graphicData);
            }

            if (voicePackDef != null)
            {
                //SubSoundDef_DynaLoading.shouldResolve = true;

                List<SoundDef> allSounds = new();
                if (voicePackDef.recruitSound != null) allSounds.Add(voicePackDef.recruitSound);
                if (!voicePackDef.draftSounds.NullOrEmpty()) allSounds.AddRange(voicePackDef.draftSounds);
                if (voicePackDef.undraftSound != null) allSounds.Add(voicePackDef.undraftSound);
                if (voicePackDef.diedSound != null) allSounds.Add(voicePackDef.diedSound);
                if (!voicePackDef.selectSounds.NullOrEmpty()) allSounds.AddRange(voicePackDef.selectSounds);
                if (!voicePackDef.abilitySounds.NullOrEmpty()) allSounds.AddRange(voicePackDef.abilitySounds);

                //List<SubSoundDef_DynaLoading> allSubSounds = new();

                /*foreach (SoundDef def in allSounds)
                {
                    foreach (SubSoundDef sub in def.subSounds)
                    {
                        if (sub is SubSoundDef_DynaLoading dyna) allSubSounds.Add(dyna);
                    }
                }

                foreach (SubSoundDef_DynaLoading dyna in allSubSounds)
                {
                    dyna.ResolveReferences();
                }

                SubSoundDef_DynaLoading.shouldResolve = false;*/
            }
        }

        void LoadResources_GraphicData(GraphicData data)
        {
            if (data is not GraphicData_DynamicLoading dd) return;

            dd.ForceLoad(modPackageID);

        }
        #endregion
    }
}