using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace AK_DLL
{
    [HotSwappable]
    public class RIWindow_MainMenu : Dialog_NodeTree
    {
        private static readonly int xMargin = 10;
        private static readonly int yMargin = 10;
        private static readonly int classSideLength = 70;
        private static readonly int classMargin;

        #region 没啥用的常量
        //左边6个主要功能按钮
        private static readonly int mainBtnWidth = 320, mainBtnHeight = 480, mainBtnMargin = 25;
        private Texture2D FBtn_Option => ContentFinder<Texture2D>.Get("UI/Frame/MM_Option");
        private Texture2D FBtn_LawMinstry => ContentFinder<Texture2D>.Get("UI/Frame/MM_Quest");
        private Texture2D FBtn_Recruit => ContentFinder<Texture2D>.Get("UI/Frame/MM_Recruit");
        private Texture2D FBtn_Support => ContentFinder<Texture2D>.Get("UI/Frame/MM_Spt");
        private Texture2D FBtn_Store => ContentFinder<Texture2D>.Get("UI/Frame/MM_Store");
        private Texture2D FBtn_Unknown => ContentFinder<Texture2D>.Get("UI/Frame/MM_Unf");
        private Texture2D FBtn_Highlight => ContentFinder<Texture2D>.Get("UI/Frame/MM_Highlight");

        private Texture2D Bg => ContentFinder<Texture2D>.Get("UI/Frame/MM_Bg");

        //界面功能按钮
        public Texture2D Btn_Escape => ContentFinder<Texture2D>.Get("UI/Frame/Btn_Escape");
        public Texture2D Btn_Highlight => ContentFinder<Texture2D>.Get("UI/Frame/Btn_Highlight");
        private int btnLength = 100;

        //货币显示
        private Texture2D Currency_Bg => ContentFinder<Texture2D>.Get("UI/Frame/CurrencyBg");
        private Texture2D Currency_Silver => ContentFinder<Texture2D>.Get("UI/Frame/IC_Silver");
        private Texture2D Currency_Ticket => ContentFinder<Texture2D>.Get("UI/Frame/IC_RTicket");

        //次要功能按钮
        private Texture2D SBtn_ChangeSecretary => ContentFinder<Texture2D>.Get("UI/Frame/Sub_Change");
        #endregion
        public OperatorDef secretaryDef
        {
            get { return AK_Tool.GetDef(AK_ModSettings.secretary); }
        }
        public RIWindow_MainMenu(DiaNode startNode, bool radioMode) : base(startNode, radioMode, false, null)
        {
        }
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(1920f, 1080f);
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            GUI.DrawTexture(new Rect(0, 0, 1920, 1080), this.Bg);

            float crntX = inRect.x + xMargin;
            float crntY = inRect.y + yMargin;
            //右边的助理
            Rect secretaryRect = new Rect(inRect.xMax - 700, inRect.y, 700, inRect.yMax - inRect.yMin);
            Widgets.DrawTextureFitted(secretaryRect, ContentFinder<Texture2D>.Get(secretaryDef.stand), secretaryDef.standRatio);
            if (Widgets.ButtonText(new Rect(inRect.xMax - 300, inRect.yMax - 300, 50, 50), "更换秘书".Translate()))
            {
            }

            Rect rect_Back = new Rect(inRect.xMax - 100, inRect.y, btnLength, btnLength);
            //退出按钮
            if (Widgets.ButtonImageFitted(rect_Back, this.Btn_Escape, Color.white, Color.white) || KeyBindingDefOf.Cancel.KeyDownEvent)
            {
                RIWindow_OperatorDetail.isRecruit = true;
                this.Close();
            }
            rect_Back.DrawHighlightMouseOver(this.Btn_Highlight);

            #region 左边的主要功能
            Rect FeatureBtn = new Rect(inRect.x, inRect.yMin + 10, mainBtnWidth, mainBtnHeight);
            //招募
            if (Widgets.ButtonImage(FeatureBtn, FBtn_Recruit, Color.white, Color.white, false))
            {
                this.Close();
                RIWindowHandler.OpenRIWindow(RIWindow.Series);
            }
            FeatureBtn.DrawHighlightMouseOver(this.FBtn_Highlight);
            FeatureBtn.x += mainBtnWidth + mainBtnMargin;

            //商店
            if (Widgets.ButtonImage(FeatureBtn, FBtn_Store, Color.white, Color.white, false))
            {

            }
            FeatureBtn.DrawHighlightMouseOver(this.FBtn_Highlight);
            FeatureBtn.x += mainBtnWidth + mainBtnMargin;

            //法务部
            if (Widgets.ButtonImage(FeatureBtn, this.FBtn_LawMinstry, Color.white, Color.white, false))
            {

            }
            FeatureBtn.DrawHighlightMouseOver(this.FBtn_Highlight);
            FeatureBtn.x -= mainBtnMargin * 2 + mainBtnWidth * 2;
            FeatureBtn.y += mainBtnHeight + mainBtnMargin;

            //军事支援
            if (Widgets.ButtonImage(FeatureBtn, this.FBtn_Support, Color.white, Color.white, false))
            {

            }
            FeatureBtn.DrawHighlightMouseOver(this.FBtn_Highlight);
            FeatureBtn.x += mainBtnWidth + mainBtnMargin;

            //待定
            if (Widgets.ButtonImage(FeatureBtn, this.FBtn_Unknown, Color.white, Color.white, false))
            {

            }
            FeatureBtn.DrawHighlightMouseOver(this.FBtn_Highlight);
            FeatureBtn.x += mainBtnWidth + mainBtnMargin;

            //设置
            if (Widgets.ButtonImage(FeatureBtn, this.FBtn_Option, Color.white, Color.white, false))
            {

            }
            FeatureBtn.DrawHighlightMouseOver(this.FBtn_Highlight);
            #endregion

            //抬头资源显示
            int currencyRect_width = (int)(inRect.xMax - inRect.xMin - btnLength - 3 * mainBtnWidth - mainBtnMargin * 3) / 2;
            int currencyRect_xMin = (int)(inRect.xMax - btnLength - 2 * currencyRect_width);
            Rect TitleCurrency = new Rect(currencyRect_xMin, inRect.yMin, currencyRect_width, btnLength);
            for (int i = 0; i < 2; ++i)
            {
                GUI.DrawTexture(TitleCurrency, Currency_Bg);
                TitleCurrency.x += currencyRect_width;
                if (Mouse.IsOver(TitleCurrency))
                {

                }
            }
            TitleCurrency.x = currencyRect_xMin;
            TitleCurrency.width = TitleCurrency.height = 100;
            GUI.DrawTexture(TitleCurrency, Currency_Silver);

            Rect subBtn = new Rect(TitleCurrency);
            TitleCurrency.x += currencyRect_width;
            GUI.DrawTexture(TitleCurrency, Currency_Ticket);

            #region 次要功能
            //更换助理
            subBtn.y += btnLength + mainBtnMargin;
            if (Widgets.ButtonImage(subBtn, SBtn_ChangeSecretary))
            {
                this.Close();
                RIWindow_OperatorDetail.isRecruit = false;
                RIWindowHandler.OpenRIWindow(RIWindow.OpList);
            }
#endregion
        }
    }
}
