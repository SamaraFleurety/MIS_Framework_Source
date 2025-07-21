using AK_DLL;
using AK_DLL.UI;
using AKA_Ability;
using FSUI;
using RimWorld;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Verse;

namespace LMA_Lib.UGUI
{
    public class AzurUI_OpDetail : RIWindow_OperatorDetail
    {
        protected override GameObject TraitBtnInstance
        {
            get
            {
                GameObject traitPrefab = TypeDef.AzurAsset.LoadAsset<GameObject>("TraitTemplate");
                GameObject traitInstance = GameObject.Instantiate(traitPrefab, traitPanel);
                return traitInstance;
            }
        }

        protected override GameObject DrawNavBtn_Home()
        {
            GameObject navBtn = GameObject.Find("BtnHome");
            navBtn.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                this.Close(false);
                RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_MainMenu, purpose: OpDetailType.Recruit);
            });
            return navBtn;
        }

        #region 原版技能点
        //此面板修改极大
        protected override void DrawVanillaSkills()
        {
            //柱状图
            GameObject skillBarPanel = GameObject.Find("SkillBarPanel");
            GameObject skillBar;
            for (int i = 0; i < AK_DLL.TypeDef.SortOrderSkill.Count; ++i)
            {
                skillBar = skillBarPanel.transform.GetChild(i).gameObject;
                //技能名字

                skillBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"<color={GetSkillLabelColor(OperatorDef.SortedSkills[i])}>{OperatorDef.SortedSkills[i].skill.label.Translate()}</color>";
                //技能等级 显示与滑动条
                skillBar.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = OperatorDef.SortedSkills[i].level.ToString();
                skillBar.GetComponentInChildren<Slider>().value = OperatorDef.SortedSkills[i].level;
            }

            //雷达图
            GameObject radarPanel = GameObject.Find("SkillRadarPanel");
            //vanillaSkillBtns.Add(radarPanel);
            RadarChart radarChart = radarPanel.GetComponentInChildren<RadarChart>();
            radarChart.data.Add(new GraphData());
            radarChart.data[0].values = new List<float>();
            radarChart.data[0].color = new Color(0, 0.5101833f, 1, 0.627451f);
            for (int i = 0; i < AK_DLL.TypeDef.SortOrderSkill.Count; ++i)
            {
                radarChart.vertexLabelValues[i] = $"<color={base.GetSkillLabelColor(OperatorDef.SortedSkills[i])}>{OperatorDef.SortedSkills[i].skill.label.Translate()}</color>";
                radarChart.data[0].values.Add((float)OperatorDef.SortedSkills[i].level / 20.0f);
            }
        }

        protected override string GetSkillLabelColor(SkillAndFire i)
        {
            int j = (int)i.fireLevel;

            switch (j)
            {
                default:
                    return "\"black\"";
                case 1:
                    return "#d9b341";  //黄
                case 2:
                    return "#f66539";  //红
            }
        }

        protected override void ChangeVanillaSkillChartTo(int val)
        {
        }
        #endregion

        public override void ReturnToParent(bool closeEV = true)
        {
            RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_OpList);
            Close(closeEV);
        }
        public override void ReturnToMainMenu()
        {
            RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_MainMenu, purpose: OpDetailType.Recruit);
            this.Close(closeEV: false);
        }
        public override void DoContent()
        {
            base.DoContent();
            DrawRelationPanel();
        }

        #region 人际关系
        static Transform relationPanel;
        //人际关系按钮
        GameObject RelationBtnInstance
        {
            get
            {
                GameObject traitPrefab = TypeDef.AzurAsset.LoadAsset<GameObject>("RelationDetails");
                GameObject traitInstance = GameObject.Instantiate(traitPrefab, relationPanel);
                return traitInstance;
            }
        }

        void DrawRelationPanel()
        {
            relationPanel = GameObject.Find("RelationsLayout").transform;
            foreach (string relation in OperatorDef.relations.Keys)
            {
                GameObject relationInstance = RelationBtnInstance;
                relationInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = OperatorDef.relations[relation].label;
                relationInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = relation;
            }
        }
        #endregion

        #region op技能
        protected override GameObject OpAbilityBtnInstance => base.OpAbilityBtnInstance;
        //本家技能 此面板ui和舟不一样 
        protected override void DrawOperatorAbility()
        {
            int skillCnt = OperatorDef.AKAbilities.Count;
            //if (skillCnt == 0) return;
            opAbilityPanel = GameObject.Find("OpAbilityLayout").transform;

            int actualSkillCount = Mathf.Max(5, skillCnt);
            opSkills = new List<GameObject>();
            int logicOrder = 0; //在(择一)技能组内，实际的顺序
            for (int i = 0; i < actualSkillCount; ++i)
            {
                GameObject opAbilityInstance = OpAbilityBtnInstance;
                if (i >= OperatorDef.AKAbilities.Count)
                {
                    opAbilityInstance.transform.GetChild(0).gameObject.SetActive(false);
                    continue;
                }

                AKAbilityDef opAbilty = OperatorDef.AKAbilities[i];
                Texture2D icon = opAbilty.Icon;
                opAbilityInstance.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector3.zero);

                //int j = i;
                if (!opAbilty.grouped)
                {
                    //右下角的勾 常驻技能橙色。
                    opAbilityInstance.transform.GetChild(2).gameObject.SetActive(true);
                    //opAbilityInstance.transform.GetChild(1).GetComponent<Image>().sprite = AK_Tool.FSAsset.LoadAsset<Sprite>("InnateAb");
                }
                //可选技能
                else
                {
                    logicOrder++;
                    //opAbilityInstance.transform.GetChild(1).gameObject.SetActive(false);
                    int j = logicOrder;
                    opAbilityInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                    {
                        SwitchGroupedSkillTo(j);
                    });
                    opSkills.Add(opAbilityInstance);
                }

                InitializeEventTrigger(opAbilityInstance.GetComponentInChildren<EventTrigger>(), OperatorDef.AKAbilities[i].description.Translate());

                opAbilityInstance.SetActive(true);
            }
        }

        protected override void SwitchGroupedSkillTo(int val)
        {
            if (AK_ModSettings.debugOverride) Log.Message($"[LMA]try switch skills to {val}");
            if (doc == null || doc.currentExist == false || OperatorDef.AKAbilities.Count == 0 || opSkills.Count <= PreferredAbility) return;
            opSkills[PreferredAbility].transform.GetChild(3).gameObject.SetActive(false);
            PreferredAbility = val;
            opSkills[PreferredAbility].transform.GetChild(3).gameObject.SetActive(true);
        }

        #endregion

        #region 换装按钮
        protected override GameObject FashionBtnInstance
        {
            get
            {
                GameObject fashionIconPrefab = TypeDef.AzurAsset.LoadAsset<GameObject>("FashionIcon");
                return GameObject.Instantiate(fashionIconPrefab, fashionPanel);
            }
        }

        protected override void DrawFashionBtn()
        {
            fashionPanel = GameObject.Find("FashionPanel").transform;
            //GameObject fashionIconPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("FashionIcon");
            GameObject fashionIcon;
            Vector3 v3;

            fashionBtns = new Dictionary<int, GameObject>();

            //精0/精1立绘 切换按钮
            fashionIcon = GameObject.Find("Elite0");
            fashionBtns.Add(0, fashionIcon);
            if (OperatorDef.commonStand != null)
            {
                fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                {
                    ChangeStandTo(0);
                });
            }
            else fashionIcon.SetActive(false);

            //精2立绘按钮。因为历史问题，这是默认立绘，必须有。
            fashionIcon = GameObject.Find("Elite2");
            fashionBtns.Add(1, fashionIcon);
            fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                ChangeStandTo(1);
            });

            //换装按钮。第一个换装（面板上第3个）在数组内是2。
            int logicOrder = 2;
            if (OperatorDef.fashion != null)
            {
                v3 = fashionIcon.transform.localPosition;
                for (int i = 0; i < OperatorDef.fashion.Count; ++i)
                {
                    //逻辑顺序 代表这按钮在面板上实际的位置（即精2按钮之后）
                    fashionIcon = FashionBtnInstance;
                    //fashionIcon.transform.localPosition = new Vector3(v3.x * logicOrder, v3.y);
                    fashionIcon.SetActive(true);
                    fashionIcon.name = "FSUI_FashIc_" + logicOrder;
                    int j = logicOrder;
                    fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                    {
                        //ChangeStandTo(btnOrder(ClickedBtn));
                        ChangeStandTo(j);
                    });
                    fashionBtns.Add(logicOrder, fashionIcon);
                    ++logicOrder;
                }
            }
            //在服装按钮界面实例化l2d换装按钮
            if (ModLister.GetActiveModWithIdentifier("FS.LivelyRim") != null)
            {
                //v3 = fashionIconPrefab.transform.localPosition;
                for (int i = 0; i < OperatorDef.live2dModel.Count; ++i)
                {
                    fashionIcon = FashionBtnInstance;
                    //fashionIcon.transform.localPosition = new Vector3(v3.x * logicOrder, v3.y);
                    int j = i + 1000; //用j来标记选中的哪个l2d。+1000代表选的l2d而不是静态换装。
                    fashionIcon.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
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
                //v3 = fashionIconPrefab.transform.localPosition;
                for (int i = 0; i < OperatorDef.fashionAnimation.Count; ++i)
                {
                    fashionIcon = FashionBtnInstance;
                    //fashionIcon.transform.localPosition = new Vector3(v3.x * logicOrder, v3.y);
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
        #endregion
    }
}
