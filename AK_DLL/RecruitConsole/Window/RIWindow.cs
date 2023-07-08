using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using FSUI;

namespace AK_DLL
{
    //使用UGUI做UI 因为不会写shader，所以渲染UGUI时会禁用UIROOT/IMGUI
    public abstract class RIWindow
    {
        public GameObject UIPrefab;
        public GameObject UIInstance;

        public GameObject L2DInstance = null; //干员l2d的模型本身

        internal AssetBundle bundle => AK_Tool.FSAsset;

        //设计是处理UI内元素，但你要拿去和外部交互我也管不着
        public virtual void Initialize()
        {
        }

        //设计顺序是DrawUI(读取UI的Prefab)->Initialize初始化UI中元素->DoContent实际绘制UI
        public virtual void DrawUI(string path)
        {
            UIPrefab = bundle.LoadAsset<GameObject>(path);

            AK_Tool.disableIMGUI = true;
            if (AK_ModSettings.debugOverride) AK_Tool.disableIMGUI = false;
            UIInstance = GameObject.Instantiate(UIPrefab);
            UIInstance.SetActive(true);
            this.DoContent();
            AK_Tool.setEV(true);
        }
        public virtual void DoContent()
        {
            Initialize();
        }

        //要是从UGUI转UGUI就不要关ev。UGUI转IMGUI或者全关可以关掉ev。
        public virtual void Close(bool closeEV = true)
        {
            this.UIInstance.SetActive(false);
            GameObject.Destroy(UIInstance);
            if (L2DInstance != null)
            {
                L2DInstance.SetActive(false);
                L2DInstance = null;
            }
            if (closeEV)
            {
                AK_Tool.setEV(false);
                AK_Tool.disableIMGUI = false;
            }
        }

        public virtual void ReturnToParent(bool closeEV = true)
        {
            this.Close(closeEV);
        }
    }
}
