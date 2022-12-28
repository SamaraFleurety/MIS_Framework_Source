using System;
using Verse;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;

namespace AK_DLL
{
    public class Window_Recruit : Dialog_NodeTree
    {
        public Window_Recruit(DiaNode startNode, bool radioMode) : base(startNode, radioMode, false, null)
        {

        }
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1280f, 720f);
            }
        }
        public override void DoWindowContents(Rect inRect)
        {
            //干员列表已经改放在ModSettings

            GUI.DrawTexture(new Rect(inRect.x, inRect.y + 37f, 1240f, 2f), BaseContent.WhiteTex);

            Rect rect_Back = new Rect(1125f, -1f, 100f, 30f);
            if (Widgets.ButtonText(rect_Back, "AK_Back".Translate()))
            {
                this.Close();
            }
            if (operatorType == -1)
            {
                Log.WarningOnce("MIS.No operator classes found.", 1);
                return;
            }
            //退出按钮
            int xOffset = 0;
            foreach (KeyValuePair<int, string> i in AK_Tool.operatorClasses)
            {
                if (Widgets.ButtonText(new Rect(inRect.x + xOffset, inRect.y + 15f, 60f, 20f), i.Value.Translate(), true, true, operatorType != i.Key)) operatorType = i.Key;
                xOffset += 80;
            }

            /*if (Widgets.ButtonText(new Rect(inRect.x, inRect.y + 15f, 60f, 20f), "Type_Caster".Translate(), true, true, operatorType != OperatorType.Caster))
            {
                operatorType = OperatorType.Caster;
            }
            else if (Widgets.ButtonText(new Rect(inRect.x + 100f, inRect.y + 15f, 60f, 20f), "Type_Defender".Translate(), true, true, operatorType != OperatorType.Defender))
            {
                operatorType = OperatorType.Defender;
            }
            else if (Widgets.ButtonText(new Rect(inRect.x + 200f, inRect.y + 15f, 60f, 20f), "Type_Guard".Translate(), true, true, operatorType != OperatorType.Guard))
            {
                operatorType = OperatorType.Guard;
            }
            else if (Widgets.ButtonText(new Rect(inRect.x + 300f, inRect.y + 15f, 60f, 20f), "Type_Vanguard".Translate(), true, true, operatorType != OperatorType.Vanguard))
            {
                operatorType = OperatorType.Vanguard;
            }
            else if (Widgets.ButtonText(new Rect(inRect.x + 400f, inRect.y + 15f, 60f, 20f), "Type_Specialist".Translate(), true, true, operatorType != OperatorType.Specialist))
            {
                operatorType = OperatorType.Specialist;
            }
            else if (Widgets.ButtonText(new Rect(inRect.x + 500f, inRect.y + 15f, 60f, 20f), "Type_Supporter".Translate(), true, true, operatorType != OperatorType.Supporter))
            {
                operatorType = OperatorType.Supporter;
            }
            else if (Widgets.ButtonText(new Rect(inRect.x + 600f, inRect.y + 15f, 60f, 20f), "Type_Medic".Translate(), true, true, operatorType != OperatorType.Medic))
            {
                operatorType = OperatorType.Medic;
            }
            else if (Widgets.ButtonText(new Rect(inRect.x + 700f, inRect.y + 15f, 60f, 20f), "Type_Sinoer".Translate(), true, true, operatorType != OperatorType.Sniper))
            {
                operatorType = OperatorType.Sniper;
            }*/
            //绘制选干员的tab

            if (AK_Tool.operatorDefs == null)
            {
                Log.Error("MIS. 干员库字典是空的.");
                return;
            }
            if (AK_Tool.operatorDefs[(int)operatorType] == null)
            {
                Log.Error($"MIS.{operatorType}号干员库是null.");
                return;
            }
            DrawOperator(inRect, AK_Tool.operatorDefs[operatorType]);
            //统一绘制干员            


        }

        public void DrawOperator(Rect inRect, Dictionary<string, OperatorDef> operators)
        {
            Rect rect_operatorFrame = new Rect(inRect.x += 5f, inRect.y += 45f, 100f, 100f);
            foreach (OperatorDef operator_Def in operators.Values)
            {
                Texture2D operatorTex = ContentFinder<Texture2D>.Get(operator_Def.headPortrait);
                Widgets.LabelFit(new Rect(rect_operatorFrame.x + 20f, rect_operatorFrame.y + 110f, 100f, 50f), operator_Def.nickname);
                Widgets.DrawTextureFitted(new Rect(rect_operatorFrame.x, rect_operatorFrame.y, rect_operatorFrame.width + 2f, rect_operatorFrame.height + 2f), ContentFinder<Texture2D>.Get("UI/Frame/Frame_HeadPortrait"), 1f);
                if (Widgets.ButtonImage(new Rect(rect_operatorFrame.x + 3f, rect_operatorFrame.y + 5f, 97f, 95f), operatorTex))
                {
                    this.Close();
                    Window_Operator window_Operator = new Window_Operator(new DiaNode(new TaggedString(operator_Def.name)), true);
                    window_Operator.operator_Def = operator_Def;
                    window_Operator.RecruitConsole = Recruit;
                    Find.WindowStack.Add(window_Operator);
                }
                rect_operatorFrame.x += 140f;
                if (rect_operatorFrame.x > 1200f)
                {
                    rect_operatorFrame.x = inRect.x;
                    rect_operatorFrame.y += 150f;
                }
            }
            //干员绘制
        }
        public Thing Recruit;
        //private static OperatorType operatorType = OperatorType.Caster;
        public static int operatorType = -1;
    }
}