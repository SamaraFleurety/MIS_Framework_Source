using AK_DLL;
using AK_DLL.UI;
using FS_UGUIFramework.UI;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace LMA_Lib.UGUI
{
    public class AzurUI_Gacha : RIWindow
    {
        private const int MaximumUpButtons = 5;

        private RecruitConsole_Azur recruitConsole;
        private Pawn gambler;

        private readonly List<GameObject> upButtons = new();
        private IReadOnlyList<OperatorDef> upOperators;
        private Image periodUpStand;
        private TextMeshProUGUI periodUpName;
        private Button gachaButton;

        public override void Initialize()
        {
            base.Initialize();

            GC_AzurManager manager = GC_AzurManager.Instance;
            recruitConsole = RIWindowHandler.RecruitConsole as RecruitConsole_Azur;
            gambler = UGUIHandler.pawn as Pawn;
            upOperators = manager.GetUpOperators(AzurDefOf.LMA_Rander_Operators);

            GameObject layoutPanel = GameObject.Find("Panel_Layout");

            upButtons.Clear();
            for (int index = 0; index < MaximumUpButtons; index++)
            {
                upButtons.Add(layoutPanel.transform.GetChild(index).gameObject);
            }

            periodUpStand = GameObject.Find("Image_PeriodUP OpStand").GetComponent<Image>();
            periodUpStand.rectTransform.sizeDelta = new Vector2(1280f, 1280f);
            periodUpName = GameObject.Find("UP OPName").GetComponentInChildren<TextMeshProUGUI>();

            //剩余时间
            TextMeshProUGUI remainingTime = GameObject.Find("UP Remaining Time").GetComponentInChildren<TextMeshProUGUI>();
            DateTime upPeriodEnd = GachaGenerator.GetPeriodStart(DateTime.Now).AddDays(7);
            remainingTime.gameObject.AddComponent<Mono_TextUpdater>().Bind(delegate
            {
                DateTime currentTime = DateTime.Now;
                TimeSpan remaining = currentTime >= upPeriodEnd ? TimeSpan.Zero : manager.GetWeeklyUpRemainingTime(currentTime);

                string formattedTime = $"{(int)remaining.TotalDays:D2}:{remaining.Hours:D2}:{remaining.Minutes:D2}";
                return "LMA_GachaUpRemaining".Translate(formattedTime).ToString();
            }, upPeriodEnd);

            gachaButton = GameObject.Find("Btn Gacha").GetComponentInChildren<Button>();

            GameObject gachaPanel = GameObject.Find("Panel_Gacha");
            gachaPanel.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text = "LMA_GachaUpChance".Translate();
        }

        public override void DoContent()
        {
            base.DoContent();
            BindHomeButton();
            BindGachaButton();
            DrawUpButtons();

            if (upOperators.Count > 0)
            {
                SelectUpOperator(0);
            }
        }

        public override void ReturnToParent(bool closeEV)
        {
            RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_MainMenu, purpose: OpDetailType.Recruit);
            Close(closeEV);
        }

        private void BindHomeButton()
        {
            Button homeButton = GameObject.Find("Btn_Home").GetComponentInChildren<Button>();
            homeButton.onClick.RemoveAllListeners();
            homeButton.onClick.AddListener(delegate
            {
                ReturnToParent(false);
            });

            Button homeEscape = GameObject.Find("Btn_Escape").GetComponentInChildren<Button>();
            homeEscape.onClick.AddListener(delegate
            {
                RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                this.Close();
            });
        }

        private void BindGachaButton()
        {
            RefreshGachaButtonState();
            gachaButton.onClick.RemoveAllListeners();
            gachaButton.onClick.AddListener(delegate
            {
                if (recruitConsole.CompRefuelable.Fuel < RecruitConsole_Azur.SingleRecruitCount - 0.01f)
                {
                    RefreshGachaButtonState();
                    return;
                }

                recruitConsole.DrawOperators(gambler, RecruitConsole_Azur.SingleRecruitCount);
                RefreshGachaButtonState();
            });
        }

        private void RefreshGachaButtonState()
        {
            gachaButton.interactable = recruitConsole.CompRefuelable.Fuel >= RecruitConsole_Azur.SingleRecruitCount - 0.01f;
        }

        private void DrawUpButtons()
        {
            for (int index = 0; index < upButtons.Count; index++)
            {
                GameObject upButton = upButtons[index];
                bool active = index < upOperators.Count;
                upButton.SetActive(active);
                if (!active) continue;

                OperatorDef operatorDef = upOperators[index];
                Texture2D portrait = operatorDef.PreferredStand(-1);
                upButton.transform.Find("Selected/UpOpPortrait").GetComponent<Image>().sprite = portrait.Image2Spirit();
                upButton.transform.Find("Unselected/UpOpPortrait").GetComponent<Image>().sprite = portrait.Image2Spirit();

                Button button = upButton.GetComponentInChildren<Button>();
                button.onClick.RemoveAllListeners();
                int selectedIndex = index;
                button.onClick.AddListener(delegate
                {
                    SelectUpOperator(selectedIndex);
                });
            }
        }

        private void SelectUpOperator(int selectedIndex)
        {
            if (selectedIndex < 0 || selectedIndex >= upOperators.Count)
            {
                //throw new ArgumentOutOfRangeException(nameof(selectedIndex), selectedIndex, upOperators.Count.ToString());
                return;
            }

            OperatorDef operatorDef = upOperators[selectedIndex];
            periodUpStand.sprite = operatorDef.PreferredStand(0).Image2Spirit();
            periodUpName.text = operatorDef.nickname.Translate();

            for (int index = 0; index < upOperators.Count; index++)
            {
                SetButtonSelected(upButtons[index], index == selectedIndex);
            }
        }

        private static void SetButtonSelected(GameObject button, bool selected)
        {
            Transform selectedRoot = button.transform.Find("Selected");
            Transform unselectedRoot = button.transform.Find("Unselected");

            selectedRoot.gameObject.SetActive(selected);
            unselectedRoot.gameObject.SetActive(!selected);
        }
    }
}
