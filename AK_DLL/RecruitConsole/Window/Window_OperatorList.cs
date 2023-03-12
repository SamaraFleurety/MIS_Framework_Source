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
        private static readonly int listWH = 100;
        private static readonly int btnHeight = 20;
        private static readonly int btnWidth = 80;
        private static readonly int xMargin = 10;
        private static readonly int yMargin = 10;
        private static readonly Vector2 labelSize = new Vector2(100f, 50f);
        private static readonly Vector2 btnSize = new Vector2(80f, 20f);
        private static readonly Vector2 frameSize = new Vector2(100f, 100f);
        private static readonly Vector2 iconSize = new Vector2(50f, 50f);
        private static Vector2 _scrollPosition = Vector2.zero;
        private static float? _cachedViewRectHeight;
        public static float ViewRectHeight
        {
            get
            {
                return _cachedViewRectHeight ?? -1;
            }
            set
            {
                _cachedViewRectHeight = value;
            }
        }
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
            Rect rect_Back = new Rect(inRect.xMax - xMargin - btnWidth, inRect.y + yMargin, btnWidth, btnHeight);
            //退出按钮
            if (/*Widgets.ButtonText(rect_Back, "AK_Back".Translate()) || */KeyBindingDefOf.Cancel.KeyDownEvent)
            {
                this.Close();
            }
            try
            {

                if (operatorType == -1)
                {
                    Log.WarningOnce("MIS.No operator classes found.", 1);
                    return;
                }
                float crntY = inRect.y + yMargin;
                float crntX = inRect.x + xMargin;

                Rect ruleRect = new Rect(inRect.position, new Vector2(inRect.width - listWH, listWH));
                GUI.DrawTexture(ruleRect, BaseContent.BlackTex);
                Rect listRect = new Rect(ruleRect.xMax, inRect.y, listWH, inRect.height);
                //绘制OperatorClass
                DrawListBtn(listRect, new Vector2(btnWidth, btnHeight));
                //出参ruleRect的x,y应当瞄准它的左下角以方便接下来DrawOperators的定位
                DrawruleRect(inRect, ref ruleRect);
                ruleRect.y += yMargin;
                //考虑使用Verse::Widgets.DrawLineHorizontal(float x, float y, float length)代替
                GUI.DrawTexture(new Rect(ruleRect.position, new Vector2(ruleRect.width - 2 * xMargin, 2f)), BaseContent.WhiteTex);
                //GUI.DrawTexture(listRect, BaseContent.WhiteTex);
                ruleRect.y += yMargin;
                if (operatorDefs == null)
                {
                    throw new NullReferenceException("MIS. 干员库字典是空的.");
                }
                if (operatorDefs[(int)operatorType] == null)
                {
                    throw new NullReferenceException($"MIS.{operatorType}号干员库是null.");
                }
                DrawOperator(ruleRect, operatorDefs[operatorType]);
                //统一绘制干员            
                Text.Anchor = default;
            }
            catch (Exception)
            {
                this.Close();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("OperatorClasses:");
                foreach (KeyValuePair<int, OperatorClassDef> i in operatorClasses)
                {
                    sb.AppendLine($"Key:{i.Key},Value:{i.Value.defName}");
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

            //在给定的baseRect内绘制大小为size的按钮，这些按钮应当在一个内层的viewRect之内
            //Widgets.BeginScrollView(baseRect, ref scrollPosition, viewRect)
            //baseRect: 实际显示大小
            //viewRect: 把所有要显示的铺平后显示的大小，大于baseRect时出现滚动条
            //并且似乎之后的坐标均应变为相对(baseRect.x, baseRect.y)的偏移量
            //scrollPosition 必须在函数外声明（
            static void DrawListBtn(Rect baseRect, Vector2 size)
            {
                Rect viewRect = new Rect(baseRect);
                viewRect.width = baseRect.width - 2 * xMargin;
                /*if (ViewRectHeight == -1)
                {
                    ViewRectHeight = 0;
                    foreach (var cls in operatorClasses.Values)
                    {
                        ViewRectHeight += cls.tex != null ? 50f : btnHeight;
                    }
                }*/
                //Log.ErrorOnce($"ViewRectHeight:{ViewRectHeight}", 114514);
                viewRect.height = iconSize.y * operatorClasses.Count;
                Rect btnRect = new Rect(viewRect.position + new Vector2(xMargin, yMargin), size);
                //测试Widgets.BeginScrollView
                baseRect.height -= 500f;
                //GUI.DrawTexture(baseRect, BaseContent.WhiteTex);
                Widgets.BeginScrollView(baseRect, ref _scrollPosition, viewRect);
                int startRow = (int)Math.Floor((decimal)(_scrollPosition.y / iconSize.y));
                startRow = (startRow < 0) ? 0 : startRow;
                int endRow = startRow + (int)(Math.Ceiling((decimal)(baseRect.height / iconSize.y)));
                endRow = (endRow > operatorClasses.Count) ? operatorClasses.Count : endRow;
                for (int i = startRow; i < endRow; i++)
                {
                    Rect row = new Rect(viewRect.x, i * iconSize.y, iconSize.x,iconSize.y);
                    //Text.WordWrap = false;
                    if (operatorClasses[i].tex != null)
                    {
                        //Widgets.DrawTextureFitted(new Rect(row.position, new Vector2(btnHeight, btnHeight)), operatorClasses[i].tex, 1f);
                        //Widgets.LabelFit(new Rect(row.position + new Vector2(btnHeight, 0f), new Vector2(btnWidth - btnHeight,btnHeight)), operatorClasses[i].label.Translate());
                        TooltipHandler.TipRegion(row, operatorClasses[i].LabelCap);
                        Widgets.DrawTextureFitted(row, operatorClasses[i].tex, 1f);
                    }
                    else
                    {
                        Widgets.LabelFit(row, operatorClasses[i].label.Translate());
                    }
                    Widgets.DrawHighlightIfMouseover(row);
                    if (Widgets.ButtonInvisible(row)) operatorType = i;
                    row.y += row.height;
                    //Text.WordWrap = true;
                }
                Widgets.EndScrollView();
            }
        }

        static void DrawruleRect(Rect baseRect, ref Rect ruleRect)
        {
            ruleRect.y = ruleRect.yMax;
        }
        //干员列表已经改放在ModSettings

        public void DrawOperator(Rect baseRect, Dictionary<string, OperatorDef> operators)
        {
            //设定文字居中
            Text.Anchor = TextAnchor.MiddleCenter;
            Rect rect_operatorFrame = new Rect(baseRect.position, frameSize);
            foreach (OperatorDef operator_Def in operators.Values)
            {
                Widgets.LabelFit(new Rect(rect_operatorFrame.position + new Vector2(0f, frameSize.y), labelSize), operator_Def.nickname);
                Widgets.DrawTextureFitted(new Rect(rect_operatorFrame.x, rect_operatorFrame.y, rect_operatorFrame.width + 2f, rect_operatorFrame.height + 2f), ContentFinder<Texture2D>.Get("UI/Frame/Frame_HeadPortrait"), 1f);
                if (Widgets.ButtonImage(new Rect(rect_operatorFrame.x + 3f, rect_operatorFrame.y + 5f, 97f, 95f), operator_Def._headPortrait))
                {
                    this.Close();
                    Window_Operator window_Operator = new Window_Operator(new DiaNode(new TaggedString(operator_Def.name)), true);
                    window_Operator.operator_Def = operator_Def;
                    window_Operator.RecruitConsole = Recruit;
                    Find.WindowStack.Add(window_Operator);
                }
                rect_operatorFrame.x += frameSize.x + xMargin;
                if (rect_operatorFrame.x + frameSize.x + xMargin > baseRect.xMax)
                {
                    rect_operatorFrame.x = baseRect.x;
                    rect_operatorFrame.y += frameSize.y + labelSize.y + yMargin;
                }
            }
            //干员绘制
        }
        public Thing Recruit;
        //private static OperatorType operatorType = OperatorType.Caster;
        public static int operatorType = -1;
    }
}