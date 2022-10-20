using System;
using RimWorld;
using Verse;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using RimWorld.Planet;

namespace AK_DLL
{
    public class OperatorDef : Def
    {
        //string defName
        //string label
        //string description
        [MustTranslate]
        public string name;//名字
        [MustTranslate]
        public string surname;//姓氏
        [MustTranslate]
        public string nickname;//昵称

        public BackstoryDef childHood;//童年背景故事
        public BackstoryDef adultHood;//成年背景故事

        public List<OperatorAbilityDef> abilities;//技能

        public int age = 16;//年龄
        public bool isMale = false;//性别

        public VoicePackDef voicePackDef;

        public Dictionary<string, PawnRelationDef> relations;

        public List<TraitAndDegree> traits;//干员特性
        public ThingDef weapon;//干员武器                                                  
        public List<ThingDef> apparels;//干员衣服
        public BodyTypeDef bodyTypeDef;//干员的体型
        private List<SkillAndFire> skills;//技能列表
        public Color skinColor /*= PawnSkinColors.GetSkinColor(0.5f)*/;//皮肤颜色
        public Color hairColor /*= PawnSkinColors.GetSkinColor(1f)*/;//头发颜色
        public HairDef hair;//头发类型
        public BeardDef beard;//胡须

        public OperatorType operatorType;//干员类型
        public string stand;//立绘
        public Vector2 standOffset;
        public string headPortrait;//头像
        public Vector2 headPortraitOffset;

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

        public void Recruit(Map map)
        {
            IntVec3 intVec;
            if (map != null)
            {
                RCellFinder.TryFindRandomPawnEntryCell(out intVec, map, 0.2f, false, null);
                if (intVec != null)
                {
                    Pawn operator_Pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);
                    operator_Pawn.ageTracker.AgeBiologicalTicks = this.age * 3600000L;
                    operator_Pawn.ageTracker.AgeChronologicalTicks = this.age * 3600000L;
                    operator_Pawn.health.hediffSet.Clear();

                    foreach (Hediff hediff_Pawn in operator_Pawn.health.hediffSet.hediffs)
                    {
                        if (hediff_Pawn.def.isBad)
                        {
                            operator_Pawn.health.RemoveHediff(hediff_Pawn);
                        }
                    }

                    operator_Pawn.story.traits.allTraits.Clear();
                    operator_Pawn.inventoryStock.stockEntries.Clear();
                    //调整

                    operator_Pawn.story.bodyType = this.bodyTypeDef;
                    operator_Pawn.story.headType = DefDatabase<HeadTypeDef>.GetNamed("Female_AverageNormal");
                    operator_Pawn.story.skinColorOverride = this.skinColor;
                    operator_Pawn.story.HairColor = this.hairColor;
                    operator_Pawn.story.hairDef = this.hair == null ? HairDefOf.Bald : this.hair;
                    operator_Pawn.style.beardDef = this.beard == null ? BeardDefOf.NoBeard : this.beard;
                    //发型与体型设置
                    operator_Pawn.Name = new NameTriple(this.name, this.nickname, this.surname);//“名”“简”“姓”
                    //名字更改

                    foreach (TraitAndDegree TraitAndDegree in this.traits)
                    {
                        operator_Pawn.story.traits.GainTrait(new Trait(TraitAndDegree.def, TraitAndDegree.degree));
                    }
                    //特性更改

                    


                    ThingWithComps weapon = (ThingWithComps)ThingMaker.MakeThing(this.weapon);
                    weapon.GetComp<CompBiocodable>().CodeFor(operator_Pawn);
                    /*CompOperatorWeapon compOperatorWeapon = new CompOperatorWeapon();
                    compOperatorWeapon.operatorDef = this;
                    weapon.AllComps.Add(compOperatorWeapon);
                    ((OperatorWeapon)weapon).operator_Pawn = operator_Pawn; */ //已经整合进成长系统的指针
                    operator_Pawn.equipment.AddEquipment(weapon);
                    //装备武器

                    operator_Pawn.skills.skills.Clear();
                    foreach (SkillAndFire skillDef in this.skills)
                    {
                        SkillRecord skill = new SkillRecord(operator_Pawn, skillDef.skill);
                        skill.passion = skillDef.fireLevel;
                        skill.Level = skillDef.level;
                        operator_Pawn.skills.skills.Add(skill);
                    }
                    //技能属性更改
                    if (GameComp_OperatorDocumentation.operatorDocument.ContainsKey(AK_Tool.GetOperatorNameFromDefName(this.defName)))
                    {
                        Dictionary<SkillDef, int> skills = GameComp_OperatorDocumentation.operatorDocument[AK_Tool.GetOperatorNameFromDefName(this.defName)].skillLevel;
                        foreach (SkillRecord i in operator_Pawn.skills.skills)
                        {
                            i.Level = skills[i.def];
                        }
                    }
                    //从干员文档更新属性

                    operator_Pawn.needs.food.CurLevel = operator_Pawn.needs.food.MaxLevel;
                    //避免Bug更改

                    operator_Pawn.gender = (this.isMale) ? Gender.Male : Gender.Female;
                    //性别更改

                    operator_Pawn.relations.ClearAllRelations();
                    Recruit_AddRelations(operator_Pawn);
                    //关系归零

                    //operator_Pawn.story.CrownType = CrownType.Average;

                    GenSpawn.Spawn(operator_Pawn, intVec, map);
                    Hediff_Operator hediff = HediffMaker.MakeHediff(HediffDef.Named("AK_Operator"), operator_Pawn, operator_Pawn.health.hediffSet.GetBrain()) as Hediff_Operator;

                    operator_Pawn.health.AddHediff(hediff, null, null, null);
                    //增加语音hediff

                    CameraJumper.TryJump(new GlobalTargetInfo(intVec, map));

                    /*foreach (Backstory back in BackstoryDatabase.allBackstories.Values)
                    {
                        if (this.childHood != null && back.title == this.childHood.title)
                        {
                            operator_Pawn.story.Childhood = back;
                        }
                        if (this.adultHood != null && back.title == this.adultHood.title)
                        {
                            operator_Pawn.story.Adulthood = back;
                        }
                    }*/

                    operator_Pawn.story.Childhood = this.childHood;
                    //童年背景设置
                    if (this.age < 20)
                    {
                        operator_Pawn.story.Adulthood = null;
                    }
                    else
                    {
                        operator_Pawn.story.Adulthood = this.adultHood;
                    }
                    //成年背景设置

                    //播放语音
                    this.voicePackDef.recruitSound.PlaySound();

                    //档案系统
                    GameComp_OperatorDocumentation.AddPawn(this.getDefName(), this, operator_Pawn, weapon);
                    hediff.document = GameComp_OperatorDocumentation.operatorDocument[this.getDefName()];
                    hediff.document.voicePack = this.voicePackDef;

                    operator_Pawn.apparel.DestroyAll();
                    foreach (ThingDef apparelDef in this.apparels)
                    {
                        Apparel apparel = (Apparel)ThingMaker.MakeThing(apparelDef);
                        operator_Pawn.apparel.Wear(apparel, true, true);
                        foreach (ThingComp i in apparel.AllComps)
                        {
                            if (i.props is CompProperties_Ability && (i.props as CompProperties_Ability).abilityDef.grouped)
                            {
                                hediff.document.groupedAbilities.Add(i.props as CompProperties_Ability);
                            }
                        }
                    }
                    //装备衣物

                    //禁用非第一个的可选技能
                    for (int i = 1; i < hediff.document.groupedAbilities.Count; ++i)
                    {
                        hediff.document.groupedAbilities[i].enabled = false;
                    }

                    //向数据库记录已生成的干员
                    //if (Operator_Recruited.RecruitedOperators != null)
                    //{
                    //    Operator_Recruited.RecruitedOperators.Add(operator_Def);//全局数据添加干员
                    //}
                    //else
                    //{
                    //    if (!Operator_Recruited.RecruitedOperators.Contains(operator_Def)) 
                    //    {
                    //        Operator_Recruited.RecruitedOperators.Add(operator_Def);
                    //    }
                    //}
                    //if (recruitedOperators != null)
                    //{
                    //    recruitedOperators.Clear();//清除招募台自己的数据
                    //    recruitedOperators.AddRange(Operator_Recruited.RecruitedOperators);//令招募台的数据与全局数据相等
                    //}
                    //else
                    //{
                    //    recruitedOperators = new List<OperatorDef>();
                    //    recruitedOperators.Clear();
                    //    recruitedOperators.AddRange(Operator_Recruited.RecruitedOperators);
                    //}
                }
            }
        }

        private void Recruit_AddRelations(Pawn p)
        {
            if (this.relations == null || this.relations.Count == 0) return;
            foreach (KeyValuePair<string, PawnRelationDef> node in relations)
            {
                if (GameComp_OperatorDocumentation.operatorDocument.ContainsKey(node.Key))
                {
                    p.relations.AddDirectRelation(node.Value, GameComp_OperatorDocumentation.operatorDocument[node.Key].pawn);
                }
            }
        }
        public string getDefName()
        {
            return AK_Tool.GetOperatorNameFromDefName(this.defName);
        }
        public void AutoFill()
        {
            //默认名字
            AutoFill_Name();

            //默认体型
            //if (this.bodyTypeDef == null) this.bodyTypeDef = BodyTypeDefOf.Thin;

            //默认武器
            AutoFill_Weapon();

            //默认衣服
            AutoFill_ApparelAbility();

            //默认发型，没有也无所谓，生成时有另一个默认值
            if (this.hair == null)
            {
                this.hair = DefDatabase<HairDef>.GetNamedSilentFail(AK_Tool.GetThingsDefName(this.defName, "Hair"));
            }

            //默认头像和立绘
            AutoFill_StandPortrait();

            //默认语音
            AutoFill_VoicePack();

        }


        #region AutoFillSubMethods
        private void AutoFill_Name()
        {
            if (this.name == null && this.nickname == null)
            {
                Log.Error($"{this.defName}缺乏任何形式的名字。");
                this.nickname = this.name = AK_Tool.GetOperatorNameFromDefName(this.defName);
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
                this.weapon = DefDatabase<ThingDef>.GetNamedSilentFail(AK_Tool.GetThingsDefName(this.defName, "Weapon"));
                if (this.weapon == null)
                {
                    this.weapon = DefDatabase<ThingDef>.GetNamed("AK_Weapon_Dusk");
                    Log.Error($"缺乏{this.nickname}的武器，随便发一把。");
                }
            }
        }
        private void AutoFill_VoicePack()
        {
            if (this.voicePackDef == null)
            {
                this.voicePackDef = DefDatabase<VoicePackDef>.GetNamedSilentFail($"AK_VoicePack_" + this.getDefName());
                if (this.voicePackDef == null)
                {
                    this.voicePackDef = new VoicePackDef(AK_Tool.GetOperatorNameFromDefName(this.defName));
                    return;
                }
            }
            this.voicePackDef.AutoFillVoicePack(this.getDefName());
        }
        private void AutoFill_StandPortrait()
        {
            if (this.headPortrait == null)
            {
                string portraitPath = "UI/Image/" + AK_Tool.operatorTypeStiring[(int)this.operatorType] + "/" + AK_Tool.GetOperatorNameFromDefName(this.defName) + "Portrait";
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
                string standPath = "UI/Image/" + AK_Tool.operatorTypeStiring[(int)this.operatorType] + "/" + AK_Tool.GetOperatorNameFromDefName(this.defName) + "Stand";
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
        }
        private void AutoFill_ApparelAbility()
        {
            string tempString = AK_Tool.GetThingsDefName(this.defName, "Apparel");
            ThingDef tempThing = DefDatabase<ThingDef>.GetNamedSilentFail(tempString);
            if (this.apparels == null) this.apparels = new List<ThingDef>();
            //从干员def绑定技能
            if (tempThing != null)
            {
                //添加干员同名衣服
                if (this.apparels.Contains(tempThing) == false) this.apparels.Add(tempThing);

                #region 自动添加技能
                if (tempThing.comps == null) tempThing.comps = new List<CompProperties>();
                CompProperties_Ability comp;
                //保存有的技能
                HashSet<OperatorAbilityDef> abilityHash = new HashSet<OperatorAbilityDef>();
                //检测衣服上每个组件 把已绑技能加入哈希
                foreach (CompProperties k in tempThing.comps)
                {
                    if (k is CompProperties_Ability)
                    {
                        if (abilityHash.Contains((k as CompProperties_Ability).abilityDef) == false) abilityHash.Add((k as CompProperties_Ability).abilityDef);
                        else Log.Error($"detected duplicate operator ability {(k as CompProperties_Ability).abilityDef.defName}");
                    }
                }
                //读取干员Def里面的技能并绑定进衣服
                if (this.abilities != null)
                {
                    foreach (OperatorAbilityDef i in this.abilities)
                    {
                        comp = new CompProperties_Ability(i);
                        if (abilityHash.Contains(i) == false) tempThing.comps.Add(comp);
                    }
                }
                //自动绑定合规范的技能
                OperatorAbilityDef j = DefDatabase<OperatorAbilityDef>.GetNamedSilentFail(tempString = AK_Tool.GetThingsDefName(this.defName, "Ability"));
                if (j != null && abilityHash.Contains(j) == false)
                {
                    comp = new CompProperties_Ability(j);
                    tempThing.comps.Add(comp);
                }
                for (int i = 0; i < 10 && (j = DefDatabase<OperatorAbilityDef>.GetNamedSilentFail(tempString + AK_Tool.romanNumber[i])) != null; ++i)
                {

                    if (abilityHash.Contains(j) == false)
                    {
                        comp = new CompProperties_Ability(j);
                        tempThing.comps.Add(comp);
                    }
                }
                abilityHash.Clear();
            }
            #endregion

            foreach (string thingType in AK_Tool.apparelType)
            {
                tempThing = DefDatabase<ThingDef>.GetNamedSilentFail(AK_Tool.GetThingsDefName(this.defName, thingType));
                if (tempThing != null && !this.apparels.Contains(tempThing)) this.apparels.Add(tempThing);
            }


        }
        #endregion

    }
}

