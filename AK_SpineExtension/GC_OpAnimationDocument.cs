using AK_DLL;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_SpineExtention
{
    //保存换肤信息的GC
    public class GC_OpAnimationDocument : GameComponent
    {
        //干员皮肤对应动态立绘动画的缓存信息,每次存读档都必须清理重置，储存方式:,<K,KV>
        public static Dictionary<OperatorDocument, Dictionary<int, GameObject>> cachedOpSkinAnimation;

        public GC_OpAnimationDocument(Game game)
        {
            cachedOpSkinAnimation = new();
        }
        public override void StartedNewGame()
        {
            base.StartedNewGame();
            if (ModLister.GetActiveModWithIdentifier("MIS.Arknights") != null)
            {
                Find.LetterStack.ReceiveLetter(LetterMaker.MakeLetter(Translator.Translate("AK_StartLabel"), Translator.Translate("AK_StartDesc"), LetterDefOf.NeutralEvent, null, null));
            }
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
