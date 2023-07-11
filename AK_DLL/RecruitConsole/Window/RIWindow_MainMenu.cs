using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FS_LivelyRim;

namespace AK_DLL
{
    /* legacy
     * [HotSwappable]
    public class RIWindow_MainMenu : Dialog_NodeTree
    {

        #region 常量，属性
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
        public static int btnLength = 100;

        //货币显示
        private Texture2D Currency_Bg => ContentFinder<Texture2D>.Get("UI/Frame/CurrencyBg");
        private Texture2D Currency_Silver => ContentFinder<Texture2D>.Get("UI/Frame/IC_Silver");
        private Texture2D Currency_Ticket => ContentFinder<Texture2D>.Get("UI/Frame/IC_RTicket");

        //次要功能按钮
        private Texture2D SBtn_ChangeSecretary => ContentFinder<Texture2D>.Get("UI/Frame/Sub_Change");

        private Texture2D SBtn_Locked => ContentFinder<Texture2D>.Get("UI/Frame/Locked");
        private Texture2D SBtn_Unlocked => ContentFinder<Texture2D>.Get("UI/Frame/Lock - Unlocked");
        private Texture2D SBtn_ScaleUp => ContentFinder<Texture2D>.Get("UI/Frame/Lens - Plus");
        private Texture2D SBtn_ScaleDown => ContentFinder<Texture2D>.Get("UI/Frame/Lens - Minus");
        private Texture2D SBtn_ArrowBase => ContentFinder<Texture2D>.Get("UI/Frame/Arrow-None-Button-White");

        private Texture2D Transp => ContentFinder<Texture2D>.Get("UI/Frame/transp");

        //
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
        public OperatorDef secretaryDef
        {
            get { return AK_Tool.GetDef(AK_ModSettings.secretary); }
        }
        private Vector3 SecLoc
        {
            get { return AK_ModSettings.secretaryLoc; }
            set { AK_ModSettings.secretaryLoc = value; }
        }
        #endregion

        static bool lockedSec = true;

        public override void DoWindowContents(Rect inRect)
        {
            GUI.DrawTexture(new Rect(0, 0, 1920, 1080), this.Bg);

            //右边的助理
            Rect secretaryRect = new Rect(inRect.xMax - 700, inRect.y, 700, inRect.yMax - inRect.yMin);
            Widgets.DrawTextureFitted(secretaryRect, ContentFinder<Texture2D>.Get(secretaryDef.stand), secretaryDef.standRatio);

            Rect rect_Back = new Rect(inRect.xMax - btnLength, inRect.y, btnLength, btnLength);
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
                RIWindowHandler.OpenRIWindow(RIWindowType.Op_Series);
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
                RIWindowHandler.OpenRIWindow(RIWindowType.Op_List);
            }

            //调整助理位置
            subBtn.y += btnLength + mainBtnMargin;
            if (lockedSec)
            {
                if (Widgets.ButtonImage(subBtn, SBtn_Locked))
                {
                    lockedSec = false;
                }
            }
            else
            {
                Vector3 secLoc = SecLoc;
                if (Widgets.ButtonImage(subBtn, SBtn_Unlocked))
                {
                    lockedSec = true;
                }
                subBtn.y += btnLength + mainBtnMargin;
                if (Widgets.ButtonImage(subBtn, SBtn_ScaleUp))
                {
                    secLoc.z += 0.1f;
                    SecLoc = secLoc;
                }
                subBtn.y += btnLength + mainBtnMargin;
                if (Widgets.ButtonImage(subBtn, SBtn_ScaleDown))
                {
                    secLoc.z -= 0.1f;
                    SecLoc = secLoc;
                }
                subBtn.y += btnLength + mainBtnMargin;
                Widgets.DrawTextureFitted(subBtn, SBtn_ArrowBase, 1);
                Rect subBtnArrow = new Rect(subBtn);
                if (Widgets.ButtonImage(subBtnArrow, FBtn_Highlight))
                {

                }
            }

            #endregion
        }
    }*/

    public class RIWindow_MainMenu : RIWindow
    {
        //调整秘书位置的按钮
        List<GameObject> adjustSecBtns;

        public GameObject OpStand; //干员静态立绘的渲染目标
        public GameObject OpL2D;   //干员动态立绘的渲染目标（不是模型本身）

        #region 快捷函数
        private GameObject ClickedBtn
        {
            get
            {
                return EventSystem.current.currentSelectedGameObject;
            }
        }

        private OperatorDef SecretaryDef => AK_Tool.GetDef(AK_ModSettings.secretary);

        private Vector3 SecretaryLoc
        {
            get { return AK_ModSettings.secretaryLoc; }
            set { AK_ModSettings.secretaryLoc = value; }
        }

        private int SecretaryLocSensetive
        {
            get { return AK_ModSettings.secLocSensitive * 10; }
        }
        #endregion
        public override void Initialize()
        {
            base.Initialize();

            OpStand = GameObject.Find("OpStand");
            OpL2D = GameObject.Find("L2DRenderTarget");
        }

        public override void DoContent()
        {
            base.DoContent();

            DrawNavBtn();
            DrawResoureHeader();

            DrawMainFeature();
            DrawSubFeature();

            DrawStand();
        }

        private void DrawNavBtn()
        {
            GameObject.Find("BtnBack").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                this.ReturnToParent();
            });
        }

        //左边6个主要功能
        private void DrawMainFeature()
        {
            GameObject.Find("MBtn_Recruit").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                RIWindow_OperatorDetail.isRecruit = true;
                RIWindowHandler.OpenRIWindow(RIWindowType.Op_List);
                this.Close(false);
            });

            //fixme:没做
            GameObject.Find("MBtn_Config").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
            });
        }



        private void DrawSubFeature()
        {
            GameObject temp;
            GameObject.Find("SBtn_ChangeSecretary").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                RIWindow_OperatorDetail.isRecruit = false;
                RIWindowHandler.OpenRIWindow(RIWindowType.Op_List);
                this.Close(false);
            });

            //点击已解锁的按钮变成未解锁。 解锁时，只需要启用锁定图标，不需要禁用未锁定图标。
            GameObject.Find("SBtn_UnlockedSec").GetComponent<Button>().onClick.AddListener(delegate
            {
                GameObject.Find("SBtn_UnlockedSec").transform.GetChild(0).gameObject.SetActive(true);
                SetSecretaryOffsetBtnsActive(false);
            });

            GameObject.Find("SBtn_LockedSec").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                SetSecretaryOffsetBtnsActive(true);
                ClickedBtn.SetActive(false);
            });

            temp = GameObject.Find("SBtn_Sec_ScaleUp");
            adjustSecBtns = new List<GameObject>();
            adjustSecBtns.Add(temp);
            temp.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                Vector3 v3 = SecretaryLoc;
                v3.z += 0.05f;
                SecretaryLoc = v3;
                DrawStand();
            });

            temp = GameObject.Find("SBtn_Sec_ScaleDown");
            adjustSecBtns.Add(temp);
            temp.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                Vector3 v3 = SecretaryLoc;
                v3.z -= 0.05f;
                SecretaryLoc = v3;
                DrawStand();
            });

            temp = GameObject.Find("SBtn_Sec_Offset");
            adjustSecBtns.Add(temp);
            //对应上下左右
            temp.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
            {
                Vector3 v3 = SecretaryLoc;
                v3.y += SecretaryLocSensetive;
                SecretaryLoc = v3;
                DrawStand();
            });
            temp.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate
           {
               Vector3 v3 = SecretaryLoc;
               v3.y -= SecretaryLocSensetive;
               SecretaryLoc = v3;
               DrawStand();
           });
            temp.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate
            {
                Vector3 v3 = SecretaryLoc;
                v3.x -= SecretaryLocSensetive;
                SecretaryLoc = v3;
                DrawStand();
            });
            temp.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate
            {
                Vector3 v3 = SecretaryLoc;
                v3.x += SecretaryLocSensetive;
                SecretaryLoc = v3;
                DrawStand();
            });

            SetSecretaryOffsetBtnsActive(false);

        }

        //看锁定秘书立绘与否 启用禁用变更位置的按钮
        private void SetSecretaryOffsetBtnsActive(bool val)
        {
            foreach (GameObject i in adjustSecBtns)
            {
                i.SetActive(val);
            }
        }

        private void DrawStand()
        {
            OpStand.SetActive(false);
            OpL2D.SetActive(false);

            if (SecretaryDef == null)
            {
                Log.Error($"MIS. missing operator named {AK_ModSettings.secretary}");
                return;
            }

            int preferredSkin = AK_ModSettings.secretarySkin;

            if (preferredSkin < 1000)
            {
                OpStand.SetActive(true);
                GameObject opStandObj = GameObject.Find("OpStand");
                AK_Tool.DrawStaticOperatorStand(SecretaryDef, preferredSkin, opStandObj, SecretaryLoc);

                /*Image opStand = opStandObj.GetComponent<Image>();
                Vector3 opStandLoc = SecretaryLoc;
                Texture2D tex;

                if (preferredSkin == 0) tex = ContentFinder<Texture2D>.Get(SecretaryDef.commonStand, false);
                else if (preferredSkin == 1) tex = ContentFinder<Texture2D>.Get(SecretaryDef.stand, false);
                else tex = ContentFinder<Texture2D>.Get(SecretaryDef.fashion[preferredSkin - 2], false);

                opStand.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                opStandObj.transform.localPosition = new Vector3(opStandLoc.x, opStandLoc.y);
                opStandObj.transform.localScale = new Vector3(opStandLoc.z, opStandLoc.z, opStandLoc.z);*/
            }
            else
            {
                OpL2D.SetActive(true);
                L2DInstance =  AK_Tool.DrawLive2DOperatorStand(SecretaryDef, preferredSkin, "L2DRenderTarget");
            }
        }
        //FIXME 没做
        private void DrawResoureHeader()
        {

        }

        public override void ReturnToParent(bool closeEV = true)
        {
            RIWindow_OperatorDetail.isRecruit = true;
            base.ReturnToParent(closeEV);
        }
    }
}