using FS_UGUIFramework.UI;
using UnityEngine;

namespace AK_DLL
{
    //使用UGUI做UI 因为不会写shader，所以渲染UGUI时会禁用UIROOT/IMGUI
    public abstract class RIWindow : UGUIWindow_Base
    {
        public GameObject L2DInstance = null; //干员l2d的模型本身
        public GameObject spineInstance = null;

        internal AssetBundle Bundle => AK_Tool.FSAsset;

        //要是从UGUI转UGUI就不要关ev。UGUI转IMGUI或者全关可以关掉ev。
        public override void Close(bool closeEV = true)
        {
            L2DInstance?.SetActive(false);
            spineInstance?.SetActive(false);
            PatchWindowOnGUI.lastSpineInstance = null;
            base.Close(closeEV);
        }
    }
}
