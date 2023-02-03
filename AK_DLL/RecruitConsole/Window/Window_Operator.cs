using System;
using Verse;
using RimWorld.Planet;
using System.Text;
using RimWorld;
using UnityEngine;
using HarmonyLib;

namespace AK_DLL
{
    public class Window_Operator : Dialog_NodeTree
    {
        public Window_Operator(DiaNode startNode, bool radioMode) : base(startNode, radioMode, false, null)
        {
        }
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1280f, 720f);
            }
        }

        public Texture2D blackBack
        {
            get
            {
                return ContentFinder<Texture2D>.Get("UI/Frame/Frame_Skills");
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            try
            {
                Widgets.DrawTextureFitted(new Rect(inRect.x += 350f + operator_Def.standOffset.x, inRect.y + 40f + operator_Def.standOffset.y, inRect.width - 870f, inRect.height), ContentFinder<Texture2D>.Get(operator_Def.stand), operator_Def.standRatio);
            }
            catch
            {
                Log.Error("立绘错误");
            }
            //立绘绘制
            GUI.DrawTexture(new Rect(970f, 0f, 550f, 720f), blackBack);
            //背景绘制
            Rect rect = new Rect(1000f, 20f, 150f, 70f);
            Rect rect_Back = new Rect(1130f, 620f, 100f, 60f);
            if (Widgets.ButtonText(rect_Back, "AK_Back".Translate()))
            {
                this.Close();
                Window_Recruit window = new Window_Recruit(new DiaNode(new TaggedString()), true);
                window.soundAmbient = SoundDefOf.RadioComms_Ambience;
                window.Recruit = RecruitConsole;
                Find.WindowStack.Add(window);
            }
            //返回按钮的绘制

            Widgets.Label(rect, operator_Def.nickname + "：" + operator_Def.name);
            //人名绘制
            Rect rect1 = new Rect(rect);
            rect.y += 20f;
            rect.height += 35f;
            Widgets.Label(rect, operator_Def.description);
            //描述绘制
            rect.height -= 35f;
            rect.y += 100f;
            Widgets.Label(rect, "AK_Terait".Translate());
            foreach (TraitAndDegree TraitAndDegree in operator_Def.traits)
            {
                rect.y += 25f;
                string label = "寄";
                label = TraitAndDegree.def.DataAtDegree(TraitAndDegree.degree)?.label;
                Widgets.Label(rect, label ?? "寄");
            }
            //特性显示绘制
            Rect rect_AbilityImage = new Rect(rect.x, rect.y + 65f, 60f, 60f);
            Rect rect_AbilityText = new Rect(rect.x + 70f, rect.y + 50f, 100f, 60f);

            if (operator_Def.abilities != null && operator_Def.abilities.Count > 0)
            {
                foreach (OperatorAbilityDef ability in operator_Def.abilities)
                {
                    Texture2D abilityImage = ContentFinder<Texture2D>.Get(ability.icon);
                    Widgets.DrawTextureFitted(rect_AbilityImage, abilityImage, 1f);
                    StringBuilder text = new StringBuilder();
                    text.AppendLine(ability.label);
                    text.AppendLine(ability.description);
                    Widgets.Label(rect_AbilityText, text.ToString().Trim());
                    rect_AbilityImage.y += 65f;
                    rect_AbilityText.y += 65f;
                }
            }
            //绘制技能(放的那种)

            rect1.x = 80f;
            rect1.y = 350f;
            rect1.width -= 60f;
            rect1.height -= 30f;
            Texture2D smallFire = ContentFinder<Texture2D>.Get("UI/Icons/PassionMinor");
            Texture2D bigFire = ContentFinder<Texture2D>.Get("UI/Icons/PassionMajor");
            //获取兴趣贴图

            Rect rect2 = new Rect(rect1);
            rect2.width += 100f;
            rect2.x += 70f;
            Rect rect3 = new Rect(rect1);
            rect3.height = 152f;
            rect3.width = 152f;
            rect3.y -= 250f;
            Widgets.DrawTextureFitted(new Rect(rect3.x + operator_Def.headPortraitOffset.x, rect3.y + operator_Def.headPortraitOffset.y, rect3.width, rect3.height), ContentFinder<Texture2D>.Get("UI/Frame/Frame_HeadPortrait"), 1f);
            Widgets.DrawTextureFitted(new Rect(rect3.x + 3f + operator_Def.headPortraitOffset.x, rect3.y + 2f + operator_Def.headPortraitOffset.y, 145f, 148f), ContentFinder<Texture2D>.Get(operator_Def.headPortrait), 1f);
            //绘制头像框与头像
            rect3.height = 150f;
            rect3.width = 150f;
            rect3.x += 5f;
            Widgets.DrawTextureFitted(new Rect(rect2.x - 45f, rect2.y + 95f, 180f, 105f), blackBack, 3f);

            foreach (SkillAndFire skillAndFire in operator_Def.Skills)
            {
                int skillLv;
                if (GameComp_OperatorDocumentation.operatorDocument.ContainsKey(operator_Def.OperatorID))
                {
                    skillLv = GameComp_OperatorDocumentation.operatorDocument[operator_Def.OperatorID].skillLevel[skillAndFire.skill];
                }
                else
                {
                    skillLv = skillAndFire.level;
                }
                float verticalOffset = 25f * TypeDef.statType[skillAndFire.skill.defName];
                Widgets.FillableBar(new Rect(rect2.x, rect2.y + verticalOffset, 170f, 20f), skillLv / 20f, SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0.3f)), ContentFinder<Texture2D>.Get("UI/Frame/Null"), false);
                Widgets.Label(new Rect(rect1.x - 35f, rect1.y + verticalOffset, 150f, rect1.height), skillAndFire.skill.label);
                Widgets.Label(new Rect(rect1.x + 50f, rect1.y + verticalOffset, 100f, rect1.height), skillLv.ToString());
                Rect rect4 = new Rect(rect1.x + 25f, rect1.y + 4f + verticalOffset, 10f, 10f);
                if (skillAndFire.fireLevel == Passion.Minor)
                {
                    Widgets.DrawTextureFitted(rect4, smallFire, 2.5f);
                }
                if (skillAndFire.fireLevel == Passion.Major)
                {
                    Widgets.DrawTextureFitted(rect4, bigFire, 2.5f);
                }
            }
            //技能绘制

            rect_Back.x -= 145f;
            OperatorDocument doc = null;
            if (GameComp_OperatorDocumentation.operatorDocument.ContainsKey(operator_Def.OperatorID))
            {
                doc = GameComp_OperatorDocumentation.operatorDocument[operator_Def.OperatorID];
            }

            if (Widgets.ButtonText(rect_Back, recruitText))
            {
                //如果招募曾经招过的干员
                if (doc != null && !doc.currentExist)
                {
                }
                //如果干员未招募过，或已死亡
                if (RecruitConsole.TryGetComp<CompRefuelable>().Fuel >= operator_Def.ticketCost - 0.01)
                {
                    if (doc == null || !doc.currentExist)
                    {
                        RecruitConsole.TryGetComp<CompRefuelable>().ConsumeFuel(operator_Def.ticketCost);
                        operator_Def.Recruit(RecruitConsole.Map);
                        this.Close();

                        Window_Recruit window = new Window_Recruit(new DiaNode(new TaggedString()), true);
                        window.soundAmbient = SoundDefOf.RadioComms_Ambience;
                        window.Recruit = RecruitConsole;
                        Find.WindowStack.Add(window);
                    }
                    else
                    {
                        recruitText = "AK_CanntRecruitOperator".Translate();
                    }
                }
                else
                {
                    recruitText = "AK_NoTicket".Translate();
                }
                
            }
            //招募
            //切换技能
            if (false && doc != null && doc.currentExist)
            {
                rect_Back.x -= 145f;
                if (Widgets.ButtonText(rect_Back, switchSkillText))
                {
                    doc.groupedAbilities[doc.preferedAbility].enabled = false;
                    doc.preferedAbility = (doc.preferedAbility + 1) % doc.groupedAbilities.Count;
                    doc.groupedAbilities[doc.preferedAbility].enabled = true;
                    Log.Message($"切换技能至{doc.groupedAbilities[doc.preferedAbility].AbilityDef.defName}");
                }
            }  
        }

        private string switchSkillText = "AK_SwitchSkill".Translate();
        public string recruitText = "AK_RecruitOperator".Translate();
        public OperatorDef operator_Def;
        public Thing RecruitConsole;
    }
}