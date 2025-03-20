using AK_DLL.UI;
using AK_DLL;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using Verse;

namespace LMA_Lib.UGUI
{
    public class AzurUI_MainMenu : RIWindow_MainMenu
    {
        protected override void DrawMainFeature()
        {
            GameObject.Find("MBtn_Recruit").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                //RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_OpList, purpose: OpDetailType.Recruit);
                this.Close(false);
            });
        }

        protected override void DrawSubFeature_ChangeSecretary()
        {
            GameObject.Find("SBtn_ChangeSecretary").GetComponentInChildren<Button>().onClick.AddListener(delegate ()
            {
                RIWindowHandler.OpenRIWindow(AzurDefOf.LMA_Prefab_OpList, purpose: OpDetailType.Secretary);
                this.Close(false);
            });
        }
    }
}
