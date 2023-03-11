using System;
using Verse;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using static AK_DLL.AK_Tool;

namespace AK_DLL
{
    public class Window_Recruit : Dialog_NodeTree
    {
        private static readonly int btnHeight = 20;
        private static readonly int btnWidth = 80;
        private static readonly int xMargin = 10;
        private static readonly int yMargin = 10;
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
            try
            {
                if (operatorType == -1)
                {
                    Log.WarningOnce("MIS.No operator classes found.", 1);
                    return;
                }
                float crntY = inRect.y + yMargin;
                float crntX = inRect.x + xMargin;
                Rect rect_Back = new Rect(inRect.xMax - xMargin - btnWidth, inRect.y + yMargin , btnWidth, btnHeight);
                //退出按钮
                if (Widgets.ButtonText(rect_Back, "AK_Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent)
                {
                    this.Close();
                }
                //绘制选干员的tab
                Rect btnRect = new Rect(crntX, crntY, btnWidth, btnHeight);
                foreach (KeyValuePair<int, string> i in AK_Tool.operatorClasses)
                {
                    /*if (Widgets.ButtonText(new Rect(inRect.x + xOffset, inRect.y + 15f, 80f, 20f), i.Value, true, true, operatorType != i.Key)) operatorType = i.Key;
                    xOffset += 100;*/
                    if (Widgets.ButtonText(btnRect, i.Value.Translate(), true, true, operatorType != i.Key)) operatorType = i.Key;
                    btnRect.x += btnWidth + xMargin;
                    if (btnRect.x + btnWidth + xMargin > rect_Back.x) //为了不和返回按钮重合
                    {
                        btnRect.y += btnHeight + yMargin;
                        btnRect.x = crntX;
                    }
                }
                btnRect.x = crntX;
                btnRect.y += btnHeight + yMargin;
                btnRect.size = inRect.size;
                GUI.DrawTexture(new Rect(btnRect.position, new Vector2(inRect.width - 2 * xMargin, 2f)), BaseContent.WhiteTex);
                btnRect.y += yMargin;
                if (AK_Tool.operatorDefs == null)
                {
                    throw new NullReferenceException("MIS. 干员库字典是空的.");
                }
                if (AK_Tool.operatorDefs[(int)operatorType] == null)
                {
                    throw new NullReferenceException($"MIS.{operatorType}号干员库是null.");
                }
                DrawOperator(btnRect, AK_Tool.operatorDefs[operatorType]);
                //统一绘制干员            
            }
            catch (Exception)
            {
                this.Close();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("OperatorClasses:");
                foreach (KeyValuePair<int, string> i in operatorClasses)
                {
                    sb.AppendLine($"Key:{i.Key},Value:{i.Value}");
                }
                sb.AppendLine("OperatorDefs:");
                foreach (var i in operatorDefs.Values)
                {
                    foreach (KeyValuePair<string, OperatorDef> j in i)
                        sb.AppendLine($"Key:{j.Key},Value:{j.Value}");
                }
                sb.AppendLine($"Current operatorType:{operatorType}");
                Log.Error($"{sb}");
                operatorType = operatorClasses.FirstOrDefault().Key;
            }
            //干员列表已经改放在ModSettings




        }

        public void DrawOperator(Rect inRect, Dictionary<string, OperatorDef> operators)
        {
            Rect rect_operatorFrame = new Rect(inRect.position, new Vector2(100f, 100f));
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