﻿using AK_DLL;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AK_SpineExtention
{
    //保存换肤信息的GC
    public class GC_OpAnimationDocument : GameComponent
    {
        //干员皮肤对应动态立绘动画的缓存信息,每次存读档都必须清理重置，储存方式:,<K,KV>
        public static Dictionary<OperatorDocument, Dictionary<int, GameObject>> cachedOpSkinAnimation = new();

        public static Dictionary<string, GameObject> cachedOpSpine = new();

        public GC_OpAnimationDocument(Game game)
        {
            cachedOpSkinAnimation = new();
        }
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            cachedOpSkinAnimation = new Dictionary<OperatorDocument, Dictionary<int, GameObject>>();
        }
        public override void LoadedGame()
        {
            base.LoadedGame();
            cachedOpSkinAnimation = new Dictionary<OperatorDocument, Dictionary<int, GameObject>>();
        }
        public override void ExposeData()
        {
            base.ExposeData();
        }
        public override void FinalizeInit()
        {
        }
    }
}
