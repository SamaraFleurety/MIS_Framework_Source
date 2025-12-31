using AKA_Ability;
using FSUI;
using RimWorld;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Verse;

namespace AK_DLL.UI
{
    public class RIWindow_OperatorDetail : RIWindow
    {
        public static OpDetailType windowPurpose = OpDetailType.Recruit;

        protected OperatorDocument doc;

        private static string _recruitText;

        private bool canRecruit;

        //0~999是静态立绘，1000-1999是l2d，2000-2999是spine
        private int preferredSkin;  //当前选中皮肤。同时存储于干员文档（如果有）来实现主界面左下角显示立绘，和mod设置的秘书选择。

        private static int preferredVanillaSkillChart = OperatorStandType.Elite2;

        protected Dictionary<int, GameObject> fashionBtns;

        private List<GameObject> vanillaSkillBtns;  //0,1: 条形图; 2,3: 雷达图

        protected List<GameObject> opSkills = new();  //只有可选技能被加进来。

        protected GameObject floatingBubbleInstance;

        public GameObject OpStand; //干员静态立绘的渲染目标
        public GameObject OpL2DRenderTarget;   //干员动态立绘的渲染目标（不是模型本身）
        private const string OpL2DRenderTargetName = "L2DRenderTarget"; //干员动态立绘的渲染目标的名字

        #region 快捷属性
        public static OperatorDef OperatorDef => RIWindowHandler.def;

        public static Thing RecruitConsole => RIWindowHandler.RecruitConsole;

        private static GameObject ClickedBtn => EventSystem.current.currentSelectedGameObject;

        /*private GameObject ClickedBtnParent
        {
            get
            {
                return ClickedBtn.transform.parent.gameObject;
            }
        }*/

        private int BtnOrder(GameObject clickedBtn)
        {
            return int.Parse(clickedBtn.name.Substring(RIWindow_OperatorList.orderInName));
        }

        protected int PreferredAbility
        {
            get => doc.preferedAbility;
            set => doc.preferedAbility = value;
        }

        #endregion

        public override void DoContent()
        {
            DrawNavBtn();
            //Initialize();
            DrawFashionBtn();
            ChangeStandTo(preferredSkin, true);

            DrawOperatorAbility();
            if (doc != null) SwitchGroupedSkillTo(doc.preferedAbility);

            DrawWeapon();
            DrawTrait();

            DrawVanillaSkills();
            ChangeVanillaSkillChartTo(preferredVanillaSkillChart);

            DrawDescription();

            DrawDebugPanel();
        }

        public override void Initialize()
        {
            base.Initialize();
            if (GC_OperatorDocumentation.opDocArchive.ContainsKey(OperatorDef.OperatorID))
            {
                doc = GC_OperatorDocumentation.opDocArchive[OperatorDef.OperatorID];
                preferredSkin = doc.preferedSkin;
            }
            canRecruit = false;
            if (RecruitConsole.TryGetComp<CompRefuelable>().Fuel >= OperatorDef.ticketCost - 0.01)
            {
                if (doc is not { currentExist: true })
                {
                    canRecruit = true;
                    _recruitText = "可以招募"; //残留
                }
                else
                {
                    _recruitText = "AK_CanntRecruitOperator".Translate();
                }
            }
            else
            {
                _recruitText = "AK_NoTicket".Translate();
            }
            floatingBubbleInstance = GameObject.Find("FloatingInfPanel");
            floatingBubbleInstance.SetActive(false);

            OpStand = GameObject.Find("OpStand");
            OpL2DRenderTarget = GameObject.Find(OpL2DRenderTargetName);
        }
        #region 绘制UI

        //换装按钮会被记录于 this.fashionbtns

        #region 左边界面
        private void DrawDescription()
        {
            GameObject OpDescPanel = GameObject.Find("OpDescPanel");
            OpDescPanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = OperatorDef.nickname.Translate();
            OpDescPanel.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = OperatorDef.description.Translate();

            //职业图标
            GameObject opClassIcon = OpDescPanel.transform.GetChild(1).gameObject;
            if (OperatorDef.operatorType.Icon == null)
            {
                TextMeshProUGUI TMP = opClassIcon.GetComponentInChildren<TextMeshProUGUI>();
                TMP.gameObject.SetActive(true);
                TMP.text = OperatorDef.operatorType.label.Translate();
            }
            else
            {
                Texture2D classImage = OperatorDef.operatorType.Icon;
                opClassIcon.GetComponent<Image>().sprite = Sprite.Create(classImage, new Rect(0, 0, classImage.width, classImage.height), new Vector2(0.5f, 0.5f));
            }
        }

        //FIXME：自动给武器打标签
        private void DrawWeapon()
        {
            GameObject weaponIconObj = GameObject.Find("WeaponIcon");
            if (OperatorDef.weapon == null)
            {
                weaponIconObj.SetActive(false);
                return;
            }
            GameObject weaponPanel = GameObject.Find("WeaponPanel");
            Texture2D weaponIcon = ContentFinder<Texture2D>.Get(OperatorDef.weapon.graphicData.texPath);
            weaponIconObj.GetComponent<Image>().sprite = Sprite.Create(weaponIcon, new Rect(0, 0, weaponIcon.width, weaponIcon.height), Vector2.zero);

            EventTrigger.Entry entry = weaponIconObj.GetComponent<EventTrigger>().triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);

            entry.callback.AddListener(data =>
            {
                DrawFloatingBubble(OperatorDef.weapon.description.Translate());
            });
        }

        protected static Transform fashionPanel;
        protected virtual GameObject FashionBtnInstance
        {
            get
            {
                GameObject fashionIconPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("FashionIcon");
                return Object.Instantiate(fashionIconPrefab, fashionPanel);
            }
        }
        protected virtual void DrawFashionBtn()
        {
            fashionPanel = GameObject.Find("FashionPanel").transform;  //切换换装按钮的面板。因为做的时候不会用Grid，所以需要手动设置按钮位置，乐
            GameObject fashionIconPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("FashionIcon");
            Vector3 v3;

            fashionBtns = new Dictionary<int, GameObject>();

            //精0/精1立绘 切换按钮
            GameObject fashionIcon = GameObject.Find("Elite0");
            fashionBtns.Add(OperatorStandType.Elite0, fashionIcon);
            if ((!OperatorDef.dynaLoadStaticStands && OperatorDef.commonStand != null) || (OperatorDef.dynaLoadStaticStands && OperatorDef.staticStands.ContainsKey(OperatorStandType.Elite0)))
            {
                fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate
                {
                    ChangeStandTo(OperatorStandType.Elite0);
                });
            }
            else fashionIcon.SetActive(false);

            //精2立绘按钮。因为历史问题，这是默认立绘，必须有。
            fashionIcon = GameObject.Find("Elite2");
            fashionBtns.Add(OperatorStandType.Elite2, fashionIcon);
            fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                ChangeStandTo(OperatorStandType.Elite2);
            });

            //换装按钮。第一个换装（面板上第3个）在数组内是2。
            int logicOrder = 2;
            if (OperatorDef.dynaLoadStaticStands)
            {
                v3 = fashionIcon.transform.localPosition;
                foreach (int key in OperatorDef.staticStands.Keys)
                {
                    if (key <= 1) continue;
                    //逻辑顺序 代表这按钮在面板上实际的位置（即精2按钮之后）
                    fashionIcon = FashionBtnInstance;
                    fashionIcon.transform.localPosition = new Vector3(v3.x * logicOrder, v3.y);
                    fashionIcon.SetActive(true);
                    fashionIcon.name = "FSUI_FashIc_" + logicOrder;
                    int j = logicOrder;
                    fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate
                    {
                        //ChangeStandTo(btnOrder(ClickedBtn));
                        ChangeStandTo(j);
                    });
                    fashionBtns.Add(logicOrder, fashionIcon);
                    ++logicOrder;
                }
            }
            else
            {
                if (OperatorDef.fashion != null)
                {
                    v3 = fashionIcon.transform.localPosition;
                    for (int i = 0; i < OperatorDef.fashion.Count; ++i)
                    {
                        //逻辑顺序 代表这按钮在面板上实际的位置（即精2按钮之后）
                        fashionIcon = FashionBtnInstance;
                        fashionIcon.transform.localPosition = new Vector3(v3.x * logicOrder, v3.y);
                        fashionIcon.SetActive(true);
                        fashionIcon.name = "FSUI_FashIc_" + logicOrder;
                        int j = logicOrder;
                        fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate
                        {
                            //ChangeStandTo(btnOrder(ClickedBtn));
                            ChangeStandTo(j);
                        });
                        fashionBtns.Add(logicOrder, fashionIcon);
                        ++logicOrder;
                    }
                }
            }
            //在服装按钮界面实例化l2d换装按钮
            if (ModLister.GetActiveModWithIdentifier("FS.LivelyRim") != null)
            {
                v3 = fashionIconPrefab.transform.localPosition;
                for (int i = 0; i < OperatorDef.live2dModel.Count; ++i)
                {
                    fashionIcon = Object.Instantiate(fashionIconPrefab, fashionPanel);
                    fashionIcon.transform.localPosition = new Vector3(v3.x * logicOrder, v3.y);
                    int j = i + 1000; //用j来标记选中的哪个l2d。+1000代表选的l2d而不是静态换装。
                    fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate
                    {
                        ChangeStandTo(j);
                    });
                    fashionBtns.Add(j, fashionIcon);
                    ++logicOrder;
                }
            }
            //spine2d动态换装
            if (ModLister.GetActiveModWithIdentifier("Paluto22.SpriteEvo") != null)
            {
                v3 = fashionIconPrefab.transform.localPosition;
                for (int i = 0; i < OperatorDef.fashionAnimation.Count; ++i)
                {
                    fashionIcon = Object.Instantiate(fashionIconPrefab, fashionPanel);
                    fashionIcon.transform.localPosition = new Vector3(v3.x * logicOrder, v3.y);
                    int k = i + 2000;
                    fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate
                    {
                        ChangeStandTo(k);
                    });
                    fashionBtns.Add(k, fashionIcon);
                    ++logicOrder;
                }
            }
        }

        //原版的驯兽等技能的面板
        protected virtual void DrawVanillaSkills()
        {
            //柱状图按钮
            GameObject skillTypeBtn = GameObject.Find("BtnBarChart");
            vanillaSkillBtns = new List<GameObject>
            {
                skillTypeBtn
            };
            skillTypeBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                preferredVanillaSkillChart = 0;
                ChangeVanillaSkillChartTo(0);
            });

            //柱状图
            GameObject skillBarPanel = GameObject.Find("SkillBarPanel");
            GameObject skillBar;
            for (int i = 0; i < TypeDef.SortOrderSkill.Count; ++i)
            {
                skillBar = skillBarPanel.transform.GetChild(i).gameObject;
                //技能名字

                skillBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color={GetSkillLabelColor(OperatorDef.SortedSkills[i])}>{OperatorDef.SortedSkills[i].skill.label.Translate()}</color>";
                //技能等级 显示与滑动条
                skillBar.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = OperatorDef.SortedSkills[i].level.ToString();
                skillBar.GetComponentInChildren<Slider>().value = OperatorDef.SortedSkills[i].level;
            }
            vanillaSkillBtns.Add(skillBarPanel);

            //雷达图按钮
            skillTypeBtn = GameObject.Find("BtnRadarChart");
            vanillaSkillBtns.Add(skillTypeBtn);

            skillTypeBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                preferredVanillaSkillChart = 2;
                ChangeVanillaSkillChartTo(2);
            });

            //雷达图
            GameObject radarPanel = GameObject.Find("SkillRadarPanel");
            vanillaSkillBtns.Add(radarPanel);
            RadarChart radarChart = radarPanel.GetComponentInChildren<RadarChart>();
            radarChart.data.Add(new GraphData());
            radarChart.data[0].values = new List<float>();
            radarChart.data[0].color = new Color(0.9686275f, 0.5882353f, 0.03137255f);
            for (int i = 0; i < TypeDef.SortOrderSkill.Count; ++i)
            {
                radarChart.vertexLabelValues[i] = $"<color={GetSkillLabelColor(OperatorDef.SortedSkills[i])}>{OperatorDef.SortedSkills[i].skill.label.Translate()}</color>";
                radarChart.data[0].values.Add(OperatorDef.SortedSkills[i].level / 20.0f);
            }
        }

        protected virtual string GetSkillLabelColor(SkillAndFire i)
        {
            int j = (int)i.fireLevel;

            return j switch
            {
                1 => "\"yellow\"",
                //break;
                2 => "\"red\"",
                //break;
                _ => "\"white\"",
            };
        }

        //0和2是按钮，1和3是图表本身
        protected virtual void ChangeVanillaSkillChartTo(int val)
        {
            if (val == 0)
            {
                vanillaSkillBtns[0].transform.GetChild(0).gameObject.SetActive(true);
                vanillaSkillBtns[1].SetActive(true);
                vanillaSkillBtns[2].transform.GetChild(0).gameObject.SetActive(false);
                vanillaSkillBtns[3].SetActive(false);
            }
            else
            {
                vanillaSkillBtns[0].transform.GetChild(0).gameObject.SetActive(false);
                vanillaSkillBtns[1].SetActive(false);
                vanillaSkillBtns[2].transform.GetChild(0).gameObject.SetActive(true);
                vanillaSkillBtns[3].SetActive(true);
            }
        }

        void DrawDebugPanel()
        {
            bool devMode = Prefs.DevMode;
            if (!devMode)
            {
                GameObject.Find("DebugToolPanel").SetActive(false);
            }
            else
            {
                //这里面的if devmode不是废话 有神必脚本会允许键鼠直接调用这些控制方法
                GameObject.Find("_DPlus").GetComponent<Button>().onClick.AddListener(delegate
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localScale;
                    loc.localScale = new Vector3(v3.z + 0.1f, v3.z + 0.1f, v3.z + 0.1f);
                    if (devMode) Log.Message($"MIS. {OperatorDef.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DMinus").GetComponent<Button>().onClick.AddListener(delegate
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localScale;
                    loc.localScale = new Vector3(v3.z - 0.1f, v3.z - 0.1f, v3.z - 0.1f);
                    if (devMode) Log.Message($"MIS. {OperatorDef.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DUP").GetComponent<Button>().onClick.AddListener(delegate
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localPosition;
                    loc.localPosition = new Vector3(v3.x, v3.y + 10f, v3.z);
                    if (devMode) Log.Message($"MIS. {OperatorDef.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DDown").GetComponent<Button>().onClick.AddListener(delegate
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localPosition;
                    loc.localPosition = new Vector3(v3.x, v3.y - 10f, v3.z);
                    if (devMode) Log.Message($"MIS. {OperatorDef.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DLeft").GetComponent<Button>().onClick.AddListener(delegate
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localPosition;
                    loc.localPosition = new Vector3(v3.x - 10f, v3.y, v3.z);
                    if (devMode) Log.Message($"MIS. {OperatorDef.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
                GameObject.Find("_DRight").GetComponent<Button>().onClick.AddListener(delegate
                {
                    Transform loc = GameObject.Find("OpStand").transform;
                    Vector3 v3 = loc.localPosition;
                    loc.localPosition = new Vector3(v3.x + 10f, v3.y, v3.z);
                    if (devMode) Log.Message($"MIS. {OperatorDef.nickname} 的 {preferredSkin}号皮肤为 (偏移)({loc.localPosition.x}, {loc.localPosition.y}, (缩放倍率){loc.localScale.x})");
                });
            }
        }

        #endregion

        #region 右边界面

        protected virtual GameObject DrawNavBtn_Home()
        {
            GameObject navBtn = GameObject.Find("BtnHome");
            navBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                this.Close(false);
                RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                //RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccMainMenu, purpose: OpDetailType.Recruit);
                ReturnToMainMenu();
            });
            return navBtn;
        }
        //确认招募和取消也是导航键
        protected virtual void DrawNavBtn()
        {
            GameObject navBtn;
            //Home
            DrawNavBtn_Home();
            //取消
            navBtn = GameObject.Find("BtnCancel");
            navBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                this.ReturnToParent(false);
            });
            //确认招募
            navBtn = GameObject.Find("BtnConfirm");
            Button button = navBtn.GetComponentInChildren<Button>();
            if (windowPurpose == OpDetailType.Recruit)
            {
                GameObject.Find("TexChangeSec").SetActive(false);
                //FIXME: 更换贴图
                button.onClick.AddListener(delegate
                {
                    //如果招募曾经招过的干员
                    if (doc != null && !doc.currentExist)
                    {
                    }
                    //如果干员未招募过，或已死亡
                    if (canRecruit)
                    {
                        this.Close();
                        RecruitConsole.TryGetComp<CompRefuelable>().ConsumeFuel(OperatorDef.ticketCost);
                        Pawn resultPawn = OperatorDef.Recruit(RecruitConsole.Map);
                        OperatorDocument document = resultPawn.GetDoc();
                        document.preferedSkin = this.preferredSkin;
                        if (RIWindowHandler.continuousRecruit) //连续招募
                        {
                            ReturnToParent(closeEV: false);
                            //RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccOpList /*RIWindowType.Op_List*/);
                        }
                    }
                });
            }
            // 更换助理按钮
            else if (windowPurpose == OpDetailType.Secretary)
            {
                //fixme
                button.onClick.AddListener(delegate
                {
                    windowPurpose = OpDetailType.Recruit;
                    AK_ModSettings.secretary = OperatorDef.defName;
                    AK_ModSettings.secretarySkin = preferredSkin;
                    if (preferredSkin < 1000)
                    {
                        AK_ModSettings.secretaryLoc = TypeDef.defaultSecLoc;
                    }
                    else AK_ModSettings.secretaryLoc = TypeDef.defaultSecLocLive;

                    //手动保存设置
                    AK_Mod.settings.Write();

                    ReturnToMainMenu();
                    /*this.Close(false);
                    RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccMainMenu);*/
                });
            }
            else if (windowPurpose == OpDetailType.Fashion)
            {

            }
        }

        protected static Transform opAbilityPanel;
        protected virtual GameObject OpAbilityBtnInstance
        {
            get
            {
                GameObject opAbilityPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("OpAbilityIcon");
                return Object.Instantiate(opAbilityPrefab, opAbilityPanel);
            }
        }
        //FIXME:切换技能逻辑不对 需要大修
        protected virtual void DrawOperatorAbility()
        {
            int skillCnt = OperatorDef.AKAbilities.Count;
            if (skillCnt == 0) return;
            else if (skillCnt >= 10)
            {
                Log.Error("目前不支持单个干员有十个及以上技能");
                return;
            }
            opAbilityPanel = GameObject.Find("OpAbilityPanel").transform;

            opSkills = new List<GameObject>();
            int logicOrder = 0; //在技能组内，实际的顺序
            for (int i = 0; i < skillCnt; ++i)
            {
                AKAbilityDef opAbilty = OperatorDef.AKAbilities[i];
                GameObject opAbilityInstance = OpAbilityBtnInstance;
                Texture2D icon = opAbilty.Icon;
                opAbilityInstance.GetComponent<Image>().sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector3.zero);

                opAbilityInstance.name = $"FSUI_OpAb_{i}_{logicOrder}";

                if (!opAbilty.grouped)
                {
                    //右下角的勾 常驻技能橙色。
                    opAbilityInstance.transform.GetChild(1).GetComponent<Image>().sprite = AK_Tool.FSAsset.LoadAsset<Sprite>("InnateAb");
                }
                //可选技能
                else
                {
                    opSkills.Add(opAbilityInstance);
                    logicOrder++;
                    opAbilityInstance.transform.GetChild(1).gameObject.SetActive(false);
                    opAbilityInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate
                    {
                        SwitchGroupedSkillTo(BtnOrder(ClickedBtn));
                    });
                }

                InitializeEventTrigger(opAbilityInstance.GetComponentInChildren<EventTrigger>(), OperatorDef.AKAbilities[i].description.Translate());

                opAbilityInstance.SetActive(true);
            }
        }

        protected virtual void SwitchGroupedSkillTo(int val)
        {
            if (AK_ModSettings.debugOverride) Log.Message($"try switch skills to {val}");
            if (doc == null || doc.currentExist == false || OperatorDef.AKAbilities.Count == 0 || opSkills.Count <= PreferredAbility) return;
            opSkills[PreferredAbility].transform.GetChild(1).gameObject.SetActive(false);
            PreferredAbility = val;
            opSkills[PreferredAbility].transform.GetChild(1).gameObject.SetActive(true);
        }

        protected virtual GameObject TraitBtnInstance
        {
            get
            {
                GameObject traitPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("TraitTemplate");
                GameObject traitInstance = Object.Instantiate(traitPrefab, traitPanel);
                return traitInstance;
            }
        }
        protected static Transform traitPanel;
        protected virtual void DrawTrait()
        {
            if (OperatorDef.traits == null || OperatorDef.traits.Count == 0) return;
            //GameObject traitPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("TraitTemplate");
            GameObject traitInstance;
            traitPanel = GameObject.Find("ActualTraitsPanel").transform;

            for (int i = 0; i < OperatorDef.traits.Count; ++i)
            {
                //traitInstance = GameObject.Instantiate(traitPrefab, traitPanel);
                traitInstance = TraitBtnInstance;
                TraitDegreeData traitDef = OperatorDef.traits[i].def.DataAtDegree(OperatorDef.traits[i].degree);
                traitInstance.GetComponentInChildren<TextMeshProUGUI>().text = traitDef.label.Translate();
                traitInstance.name = "FSUI_Traits_" + i;
                InitializeEventTrigger(traitInstance.GetComponent<EventTrigger>(), AK_Tool.DescriptionManualResolve(OperatorDef.traits[i].def.DataAtDegree(OperatorDef.traits[i].degree).description, OperatorDef.nickname, OperatorDef.isMale ? Gender.Male : Gender.Female));
            }
        }
        #endregion
        private void DrawStand()
        {
            //禁用掉之前的立绘
            OpStand.SetActive(false);
            OpL2DRenderTarget.SetActive(false);
            L2DInstance?.SetActive(false);
            spineInstance?.SetActive(false);

            //静态立绘
            if (preferredSkin < 1000)
            {
                OpStand.SetActive(true);
                AK_Tool.DrawStaticOperatorStand(OperatorDef, preferredSkin, OpStand, OpStaticStandOffset());
            }
            //l2d
            else if (preferredSkin < 2000)
            {
                OpL2DRenderTarget.SetActive(true);
                RIWindow_MainMenu.SetLive2dDefaultCanvas(false); //就用ui的 canvas就可以
                //FS_Tool.SetDefaultCanvas(false); //ui有canvas
                //L2DInstance = FS_Utilities.DrawModel(DisplayModelAt.RIWDetail, RIWindowHandler.def.Live2DModelDef(Def.live2dModel[preferredSkin - 1000]), OpL2D);
                L2DInstance = RIWindow_MainMenu.DrawLive2DModel(/*Def, */4/*DisplayModelAt.RIWDetail*/, OperatorDef.live2dModel[preferredSkin - 1000], OpL2DRenderTarget);
            }
            else
            {
                OpL2DRenderTarget.SetActive(true);

                Image compImage = OpL2DRenderTarget.GetComponent<Image>();
                //compImage.material ??= AK_Tool.FSAsset.LoadAsset<Material>("OffScreenCameraMaterial");

                spineInstance = RIWindow_MainMenu.DrawSpine2DModel(OperatorDef.fashionAnimation[preferredSkin - 2000]);

                /*Camera camera = spineInstance.GetComponentInChildren<Camera>();
                if (camera.targetTexture.width != 1920 || camera.targetTexture.height != 1080)
                {
                    camera.targetTexture = new RenderTexture(1920, 1080, 24);
                }
                compImage.material.mainTexture = camera.targetTexture;*/
                compImage.material.mainTexture = RIWindow_MainMenu.GetOrSetSpineRenderTexture(spineInstance);
            }
        }

        //记得x是x，y是环世界一般意义的z，z是缩放！！！
        protected virtual Vector3 OpStaticStandOffset()
        {
            return Vector3.zero;
        }

        protected void ChangeStandTo(int val, bool forceChange = false, StandType standType = StandType.Static)
        {
            if (!forceChange && val == preferredSkin) return;

            if (doc != null)
            {
                doc.preferedSkin = val;
                /*if (val < 1000)
                {
                    doc.preferedSkin = val;
                }*/
            } 

            //禁用之前的换装按钮
            GameObject fBtn = fashionBtns[preferredSkin];
            fBtn.transform.GetChild(0).gameObject.SetActive(true);
            fBtn.transform.GetChild(1).gameObject.SetActive(false);

            //启用新的换装按钮
            preferredSkin = val;
            fBtn = fashionBtns[preferredSkin];
            fBtn.transform.GetChild(0).gameObject.SetActive(false);
            fBtn.transform.GetChild(1).gameObject.SetActive(true);
            DrawStand(); //实际刷新立绘立绘
        }


        //对于不存在于界面预制体内的obj，不能在unity里面设定关掉悬浮窗。
        protected void InitializeEventTrigger(EventTrigger ev, string text)
        {
            EventTrigger.Entry entry = ev.triggers.Find(e => e.eventID == EventTriggerType.PointerEnter);

            entry.callback.AddListener(data =>
            {
                DrawFloatingBubble(text);
            });

            entry = ev.triggers.Find(e => e.eventID == EventTriggerType.PointerExit);


            entry.callback.AddListener(data =>
            {
                floatingBubbleInstance.SetActive(false);
            });

        }

        //鼠标指上去的悬浮窗 
        protected virtual void DrawFloatingBubble(string text)
        {
            floatingBubbleInstance.GetComponentInChildren<TextMeshProUGUI>().text = text;
            Vector3 mousePosition = Input.mousePosition;
            Vector2 pivot;
            pivot = mousePosition.x < Screen.currentResolution.width / 2 ? new Vector2(0, 1) : new Vector2(1, 1);

            ((RectTransform)floatingBubbleInstance.transform).pivot = pivot;
            floatingBubbleInstance.transform.position = Input.mousePosition;

            //自动计算大小
            floatingBubbleInstance.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)floatingBubbleInstance.transform);
        }

        public override void ReturnToParent(bool closeEV = true)
        {
            RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccOpList/* RIWindowType.Op_List*/);
            base.ReturnToParent(closeEV);
        }

        public virtual void ReturnToMainMenu()
        {
            RIWindowHandler.OpenRIWindow(AKDefOf.AK_Prefab_yccMainMenu, purpose: OpDetailType.Recruit);
            this.Close(closeEV: false);
        }

        #endregion
    }
}