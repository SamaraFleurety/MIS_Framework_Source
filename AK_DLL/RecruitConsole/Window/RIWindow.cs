﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    //使用UGUI做UI 因为不会写shader，所以渲染UGUI时会禁用UIROOT/IMGUI
    public abstract class RIWindow
    {
        public GameObject UIPrefab;
        public GameObject UIInstance;
        internal AssetBundle bundle => AK_Tool.FSAsset;

        public virtual void DrawUI(string path)
        {
            UIPrefab = bundle.LoadAsset<GameObject>(path);

            AK_Tool.disableIMGUI = false;
            UIInstance = GameObject.Instantiate(UIPrefab);
            UIInstance.SetActive(true);
            this.DoContent();
            AK_Tool.setEV(true);
        }
        public abstract void DoContent();

        public virtual void Close()
        {
            UIInstance.SetActive(false);
            AK_Tool.setEV(false);
            AK_Tool.disableIMGUI = false;
        }
    }
}