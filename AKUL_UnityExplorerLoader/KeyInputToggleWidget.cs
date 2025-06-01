using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityExplorer.UI;
using UniverseLib.UI;
using Text = UnityEngine.UI.Text;

namespace AKUL_UnityExplorerLoader
{
    internal class KeyInputToggleWidget
    {
        public KeyInputToggleWidget(GameObject parent)
        {
            Instance = this;

            ConstructUI(parent);
        }

        public static KeyInputToggleWidget Instance;

        protected GameObject lockBtn;
        protected Text lockLabel;
        protected Toggle LockedToggle;
        public static bool Locked { get; protected set; } = true;

        protected void OnToggleBlock(bool value)
        {
            Locked = value;
        }

        protected void ConstructUI(GameObject parent)
        {
            lockBtn = UIFactory.CreateToggle(parent, "InputToggleButton", out LockedToggle, out lockLabel);
            LockedToggle.isOn = true;
            lockLabel.alignment = TextAnchor.UpperLeft;
            lockLabel.text = "Block Keys";
            UIFactory.SetLayoutElement(lockBtn, minHeight: 25, minWidth: 100);
            LockedToggle.onValueChanged.AddListener(OnToggleBlock);
        }

        public static void AddButton()
        {
            var navbar = GameObject.Find("MainNavbar");
            var layout = navbar.GetComponent<HorizontalLayoutGroup>();
            Instance = new KeyInputToggleWidget(navbar);
            Instance.lockBtn.transform.SetSiblingIndex(layout.transform.childCount - 2);
        }
    }

    [HarmonyPatch(typeof(UIManager), "InitUI")]
    internal class UIManager_Patch
    {
        [HarmonyPostfix]
        public static void UIManager_InitUI_Postfix()
        {
            KeyInputToggleWidget.AddButton();
        }
    }
}