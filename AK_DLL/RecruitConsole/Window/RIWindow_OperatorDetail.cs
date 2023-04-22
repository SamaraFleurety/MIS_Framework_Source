using System;
using Verse;
using RimWorld.Planet;
using System.Text;
using RimWorld;
using UnityEngine;
using HarmonyLib;

namespace AK_DLL
{
    public class RIWindow_OperatorDetail : Dialog_NodeTree
    {
        public static bool isRecruit = true;
        public static readonly Color StackElementBackground = new Color(1f, 1f, 1f, 0.1f);
        public RIWindow_OperatorDetail(DiaNode startNode, bool radioMode) : base(startNode, radioMode, false, null)
        {
        }
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1920f, 1080f);
            }
        }

        public Texture2D blackBack
        {
            get
            {
                return ContentFinder<Texture2D>.Get("UI/Frame/Frame_Skills");
            }
        }
        public OperatorDef Operator_Def
        {
            get { return RIWindowHandler.def; }
        }
        public Thing RecruitConsole
        {
            get { return RIWindowHandler.recruitConsole; }
        }
        
        public override void DoWindowContents(Rect inRect)
        {
            Color temp = GUI.color;       
            try
            {
                Widgets.DrawTextureFitted(new Rect(inRect.x += 350f + Operator_Def.standOffset.x, inRect.y + 40f + Operator_Def.standOffset.y, inRect.width - 870f, inRect.height), ContentFinder<Texture2D>.Get(Operator_Def.stand), Operator_Def.standRatio);
            }
            catch
            {
                Log.Error("MIS. 立绘错误");
            }
            //立绘绘制
            GUI.DrawTexture(new Rect(970f, 0f, 550f, 720f), blackBack);
            //背景绘制
            Rect rect = new Rect(1000f, 20f, 150f, 70f);

            //返回按钮的绘制
            Rect rect_Back = new Rect(1130f, 620f, 100f, 60f);
            if (Widgets.ButtonText(rect_Back, "AK_Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent)
            {
                this.Close();
                RIWindowHandler.OpenRIWindow(RIWindowType.Op_List);
            }

            Widgets.Label(rect, Operator_Def.nickname + "：" + Operator_Def.name);
            //人名绘制
            Rect rect1 = new Rect(rect);
            rect.y += 20f;
            rect.height += 35f;
            Widgets.Label(rect, Operator_Def.description);
            //描述绘制
            rect.height -= 35f;
            rect.y += 100f;
            Widgets.Label(rect, "AK_Terait".Translate());
            rect.y += 25f;
            foreach (TraitAndDegree TraitAndDegree in Operator_Def.traits)
            {
                TraitDegreeData traitDef = TraitAndDegree.def.DataAtDegree(TraitAndDegree.degree);
                if (traitDef == null)
                {
                    Log.ErrorOnce($"MIS. {this.Operator_Def}'s {TraitAndDegree.def.defName} do not have {TraitAndDegree.degree} degree", 1);
                }
                else
                {
                    Rect traitRect = new Rect(rect.x, rect.y, Text.CalcSize(traitDef.label).x + 10f, 25);
                    temp = GUI.color;
                    GUI.color = StackElementBackground;
                    GUI.DrawTexture(traitRect, BaseContent.WhiteTex);
                    GUI.color = temp;
                    Text.Anchor = TextAnchor.MiddleCenter;
                    Widgets.Label(traitRect, traitDef.label.Truncate(traitRect.width));
                    Text.Anchor = TextAnchor.UpperLeft;
                    if (Mouse.IsOver(traitRect)) { Widgets.DrawHighlight(traitRect); }
                    rect.y += 28f;
                }
                /*string label = "寄";
                label = TraitAndDegree.def.DataAtDegree(TraitAndDegree.degree)?.label;
                Widgets.Label(rect, label ?? "寄");*/
            }
            //特性显示绘制
            Rect rect_AbilityImage = new Rect(rect.x, rect.y + 65f, 60f, 60f);
            Rect rect_AbilityText = new Rect(rect.x + 70f, rect.y + 50f, 100f, 60f);

            if (Operator_Def.abilities != null && Operator_Def.abilities.Count > 0)
            {
                foreach (OperatorAbilityDef ability in Operator_Def.abilities)
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
            //^绘制技能(放的那种)

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
            Widgets.DrawTextureFitted(new Rect(rect3.x + Operator_Def.headPortraitOffset.x, rect3.y + Operator_Def.headPortraitOffset.y, rect3.width, rect3.height), ContentFinder<Texture2D>.Get("UI/Frame/Frame_HeadPortrait"), 1f);
            Widgets.DrawTextureFitted(new Rect(rect3.x + 3f + Operator_Def.headPortraitOffset.x, rect3.y + 2f + Operator_Def.headPortraitOffset.y, 145f, 148f), ContentFinder<Texture2D>.Get(Operator_Def.headPortrait), 1f);
            //绘制头像框与头像
            rect3.height = 150f;
            rect3.width = 150f;
            rect3.x += 5f;
            Widgets.DrawTextureFitted(new Rect(rect2.x - 45f, rect2.y + 95f, 180f, 105f), blackBack, 3f);

            foreach (SkillAndFire skillAndFire in Operator_Def.Skills)
            {
                int skillLv;
                if (GameComp_OperatorDocumentation.operatorDocument.ContainsKey(Operator_Def.OperatorID))
                {
                    skillLv = GameComp_OperatorDocumentation.operatorDocument[Operator_Def.OperatorID].skillLevel[skillAndFire.skill];
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
            if (GameComp_OperatorDocumentation.operatorDocument.ContainsKey(Operator_Def.OperatorID))
            {
                doc = GameComp_OperatorDocumentation.operatorDocument[Operator_Def.OperatorID];
            }

            if (Widgets.ButtonText(rect_Back, recruitText))
            {
                if (isRecruit == false)
                {
                    isRecruit = true;
                    AK_ModSettings.secretary = AK_Tool.GetOperatorIDFrom(Operator_Def.defName);
                    this.Close();
                    RIWindowHandler.OpenRIWindow(RIWindowType.MainMenu);
                    return;
                }

                //如果招募曾经招过的干员
                if (doc != null && !doc.currentExist)
                {
                }
                //如果干员未招募过，或已死亡
                if (RecruitConsole.TryGetComp<CompRefuelable>().Fuel >= Operator_Def.ticketCost - 0.01)
                {
                    if (doc == null || !doc.currentExist)
                    {
                        RecruitConsole.TryGetComp<CompRefuelable>().ConsumeFuel(Operator_Def.ticketCost);
                        Operator_Def.Recruit(RecruitConsole.Map);
                        this.Close();

                        /*RIWindow_OperatorList window = new RIWindow_OperatorList(new DiaNode(new TaggedString()), true);
                        window.soundAmbient = SoundDefOf.RadioComms_Ambience;
                        Find.WindowStack.Add(window);*/
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
    }
}