using AK_DLL;
using AK_DLL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LMA_Lib.UGUI
{
    public class AzurUI_MainMenu : RIWindow_MainMenu
    {
        public override void Initialize()
        {
            base.Initialize();
            GameObject.Find("Silver").GetComponentInChildren<TextMeshProUGUI>().text = GC_AzurManager.Instance.storedSilver.ToString();
        }

        protected override void DrawMainFeature()
        {
            GameObject.Find("MBtn_Recruit").GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                //RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_OpList, purpose: OpDetailType.Recruit);
                Close(false);
            });

            GameObject.Find("MBtn_Gacha").GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_Gacha, purpose: OpDetailType.Recruit);
                Close(false);
            });
        }

        protected override void DrawSubFeature_ChangeSecretary()
        {
            GameObject.Find("SBtn_ChangeSecretary").GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_OpList, purpose: OpDetailType.Secretary);
                Close(false);
            });
        }
    }
}
