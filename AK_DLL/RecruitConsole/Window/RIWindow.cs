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

        internal AssetBundle bundle => AK_Tool.FSAsset;

        public virtual void Initialize()
        {
        }

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
