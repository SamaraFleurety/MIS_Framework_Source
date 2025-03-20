using System;
using System.Collections.Generic;
using Verse;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using FS_LivelyRim;
using System.Reflection;
using SpriteEvo;
using RimWorld;

namespace AK_DLL.UI
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
        protected List<GameObject> adjustSecBtns;

        public GameObject OpStand; //干员静态立绘的渲染目标
        public GameObject OpL2D;   //干员动态立绘的渲染目标（不是模型本身，是名为L2DRenderTarget的游戏物体）

        #region 快捷函数
        /*private GameObject ClickedBtn
        {
            get
            {
                return EventSystem.current.currentSelectedGameObject;
            }
        }*/

        protected OperatorDef SecretaryDef => DefDatabase<OperatorDef>.GetNamed(AK_ModSettings.secretary);

        protected Vector3 SecretaryLoc
        {
            get { return AK_ModSettings.secretaryLoc; }
            set { AK_ModSettings.secretaryLoc = value; }
        }

        protected int SecretaryLocSensetive
        {
            get { return AK_ModSettings.secLocSensitive * 10; }
        }
        #endregion
        public override void Initialize()
        {
            base.Initialize();

            if (AK_Tool.Live2DActivated) SetLive2dDefaultCanvas(false);
            OpStand = GameObject.Find("OpStand");
            OpL2D = GameObject.Find("L2DRenderTarget");
        }

        public static void SetLive2dDefaultCanvas(bool active = true)
        {
            MethodInfo method = typeof(FS_Tool).GetMethod("SetDefaultCanvas", BindingFlags.Static | BindingFlags.Public);
            method.Invoke(null, new object[] { active });

            //FS_Tool.SetDefaultCanvas(active);
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

        protected virtual void DrawNavBtn()
        {
            GameObject.Find("BtnBack").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                this.ReturnToParent();
            });
        }

        //左边6个主要功能
        protected virtual void DrawMainFeature()
        {
            GameObject.Find("MBtn_Recruit").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                //RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccOpList, purpose : OpDetailType.Recruit);
                this.Close(false);
            });

            return;
            //fixme:没做
            GameObject.Find("MBtn_Config").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
            });
        }

        protected virtual void DrawSubFeature_ChangeSecretary()
        {
            GameObject.Find("SBtn_ChangeSecretary").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccOpList, purpose: OpDetailType.Secretary);
                this.Close(false);
            });
        }

        protected virtual void DrawSubFeature()
        {
            GameObject temp;

            DrawSubFeature_ChangeSecretary();

            //点击已解锁的按钮变成未解锁。 解锁时，只需要启用锁定图标，不需要禁用未锁定图标。
            /*GameObject.Find("SBtn_UnlockedSec").GetComponent<Button>().onClick.AddListener(delegate
            {
                GameObject.Find("SBtn_UnlockedSec").transform.GetChild(0).gameObject.SetActive(true);
                SetSecretaryOffsetBtnsActive(false);
            });

            GameObject.Find("SBtn_LockedSec").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                SetSecretaryOffsetBtnsActive(true);
                ClickedBtn.SetActive(false);
            });*/

            temp = GameObject.Find("SBtn_Sec_ScaleUp");
            adjustSecBtns = new List<GameObject>();
            adjustSecBtns.Add(temp);
            temp.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                Vector3 v3 = SecretaryLoc;
                v3.z += 0.05f;
                SecretaryLoc = v3;
                DrawStand();
            });

            temp = GameObject.Find("SBtn_Sec_ScaleDown");
            adjustSecBtns.Add(temp);
            temp.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
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


        }

        //看锁定秘书立绘与否 启用禁用变更位置的按钮
        private void SetSecretaryOffsetBtnsActive(bool val)
        {
            foreach (GameObject i in adjustSecBtns)
            {
                i.SetActive(val);
            }
        }

        protected virtual void DrawStand()
        {
            OpStand.SetActive(false);
            OpL2D.SetActive(false);
            L2DInstance?.SetActive(false);
            spineInstance?.SetActive(false);

            //因为添加移除mod后导致的，缺失秘书
            if (SecretaryDef == null)
            {
                Log.Error($"[MIS] missing operator named {AK_ModSettings.secretary}");
                List<OperatorDef> allOpDefs = DefDatabase<OperatorDef>.AllDefsListForReading;
                if (allOpDefs.NullOrEmpty()) return;
                AK_ModSettings.secretary = allOpDefs.RandomElement().defName;
                AK_ModSettings.secretarySkin = 1;
                Log.Message($"[MIS] setting secretary as {SecretaryDef}");
                AK_Mod.settings.Write();
            }


            int preferredSkin = AK_ModSettings.secretarySkin;
            if (preferredSkin >= 2000 && ModLister.GetActiveModWithIdentifier("Paluto22.SpriteEvo") == null) //之前是spine，但现在不可能
            {
                AK_ModSettings.secretarySkin = 1;
                preferredSkin = 1;
                AK_Mod.settings.Write();
            }
            else if (preferredSkin >= 1000 && ModLister.GetActiveModWithIdentifier("FS.LivelyRim") == null) //现在不能播l2d
            {
                AK_ModSettings.secretarySkin = 1;
                preferredSkin = 1;
                AK_Mod.settings.Write();
            }

            if (preferredSkin < 1000)
            {
                OpStand.SetActive(true);
                GameObject opStandObj = GameObject.Find("OpStand");
                AK_Tool.DrawStaticOperatorStand(SecretaryDef, preferredSkin, opStandObj, SecretaryLoc);
            }
            else if (preferredSkin < 2000)
            {
                OpL2D.SetActive(true);
                //L2DInstance =  AK_Tool.DrawLive2DOperatorStand(SecretaryDef, preferredSkin, OpL2D);
                //L2DInstance = FS_Utilities.DrawModel(DisplayModelAt.RIWMain, RIWindowHandler.def.Live2DModelDef(SecretaryDef.live2dModel[preferredSkin - 1000]), OpL2D);
                L2DInstance = DrawLive2DModel(/*SecretaryDef, */3/*DisplayModelAt.RIWMain*/, SecretaryDef.live2dModel[preferredSkin - 1000], OpL2D);
                L2DInstance.transform.position = SecretaryLoc;
            }
            else
            {
                OpL2D.SetActive(true);
                
                Image compImage = OpL2D.GetComponent<Image>();
                //compImage.material ??= AK_Tool.FSAsset.LoadAsset<Material>("OffScreenCameraMaterial");

                spineInstance = DrawSpine2DModel(SecretaryDef.fashionAnimation[preferredSkin - 2000]);

                /*Camera camera = spineInstance.GetComponentInChildren<Camera>();
                camera.targetTexture.width = 1920;
                camera.targetTexture.height = 1080;*/
                compImage.material.mainTexture = GetOrSetSpineRenderTexture(spineInstance);
            }
        }

        public static GameObject DrawLive2DModel(int drawAt, string l2dDefname, GameObject renderTarget = null)
        {
            MethodInfo method = typeof(FS_Utilities).GetMethod("DrawLive2DModel", BindingFlags.Public | BindingFlags.Static);
            return (GameObject)method.Invoke(null, new object[] { drawAt, l2dDefname, renderTarget });
            //return FS_Utilities.DrawModel(drawAt, RIWindowHandler.def.Live2DModelDef(l2dDefname), renderTarget);
        }

        public static GameObject DrawSpine2DModel(string spineDefname)
        {
            MethodInfo method = typeof(SkeletonAnimationUtility).GetMethod("InstantiateSpineByDefname", BindingFlags.Public | BindingFlags.Static);
            return (GameObject)method.Invoke(null, new object[] { spineDefname, spineDefname, 2, true, true, true, null });
        }

        public static RenderTexture GetOrSetSpineRenderTexture(GameObject spine, int width = 1920, int height = 1080)
        {
            Camera camera = spine.GetComponentInChildren<Camera>();
            if (camera.targetTexture.width != width || camera.targetTexture.height != height)
            {
                camera.targetTexture = new RenderTexture(width, height, 24);
            }
             return camera.targetTexture;
        }

        //FIXME 没做
        protected virtual void DrawResoureHeader()
        {

        }

        public override void ReturnToParent(bool closeEV = true)
        {
            RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
            base.ReturnToParent(closeEV);
        }
    }
}