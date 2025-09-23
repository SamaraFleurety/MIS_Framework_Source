using AK_DLL;
using AK_DLL.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LMA_Lib.UGUI
{
    public class AzurUI_OpList : RIWindow_OperatorList
    {
        protected override GameObject OperatorPortraitInstance
        {
            get
            {
                GameObject opRectPrefab = TypeDef.AzurAsset.LoadAsset<GameObject>("OperatorTemplate");
                return GameObject.Instantiate(opRectPrefab, opListPanel);
            }
        }

        protected override GameObject SortButtonInstance
        {
            get
            {
                GameObject sortBtnPrefab = TypeDef.AzurAsset.LoadAsset<GameObject>("btnSortTemplate");
                return GameObject.Instantiate(sortBtnPrefab, sorterColumnLoc);
            }
        }

        protected override GameObject ClassButtonInstance
        {
            get
            {
                GameObject classBtnPrefab = TypeDef.AzurAsset.LoadAsset<GameObject>("Icon_Class");
                return GameObject.Instantiate(classBtnPrefab, classColumn);
            }
        }

        public override void Close(bool closeEV = true)
        {
            base.Close(closeEV);
            previousSortBtn = null;
            previousClassBtn = null;
        }
        public override void ReturnToParent(bool closeEV = true)
        {
            RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_MainMenu, purpose: OpDetailType.Recruit);
            Close(closeEV);
        }

        protected override void OpPortraitBtnOnClickListener(int index)
        {
            RIWindowHandler.OpenRIWindow_OpDetail(AzurDefOf.LMA_Prefab_OpDetail, cachedOperatorList[index]);
            this.Close(false);
        }

        #region 排序按钮
        static GameObject previousSortBtn = null;
        //增添了一个按下会弹出的效果
        protected override GameObject DrawOneSortBtn(int sortID, string label)
        {
            GameObject btn = base.DrawOneSortBtn(sortID, label);

            btn.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = label;
            btn.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                previousSortBtn?.transform.GetChild(1).gameObject.SetActive(false);  //弹出的视觉效果
                previousSortBtn = btn;
                previousSortBtn.transform.GetChild(1).gameObject.SetActive(true);
            });

            return btn;
        }
        #endregion

        #region 职业按钮
        static GameObject previousClassBtn = null;
        //加了个选中会弹起的功能
        protected override GameObject DrawOneClassBtn(int classIndex)
        {
            GameObject btn = base.DrawOneClassBtn(classIndex);

            btn.transform.GetChild(2).gameObject.SetActive(false);
            btn.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                previousClassBtn?.transform.GetChild(2).gameObject.SetActive(false);

                previousClassBtn = btn;
                btn.transform.GetChild(2).gameObject.SetActive(true);
            });

            btn.transform.GetChild(2).GetChild(0).GetComponent<Image>().sprite = btn.transform.GetChild(0).GetComponent<Image>().sprite;

            return btn;
        }

        protected override GameObject DrawOneClassBtn_Functionless(Texture2D icon, string label)
        {
            GameObject classBtnInstance = ClassButtonInstance;

            classBtnInstance.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), new Vector2(0.5f, 0.5f));

            classBtnInstance.SetActive(true);

            return classBtnInstance;
        }
        #endregion
        protected override int MaxOperatorPerPage => 40;
    }
}
