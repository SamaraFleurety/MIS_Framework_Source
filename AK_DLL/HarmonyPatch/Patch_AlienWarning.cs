using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Steamworks;

namespace AK_DLL
{
    //外星人兼容警告Patch
    [HarmonyPatch(typeof(Page_SelectScenario), "BeginScenarioConfiguration")]
    public class Patch_AlienNewGameDialog
    {
        public static bool DisabledAlienWarning_NewColony = false;
        [HarmonyPostfix]
        public static void NewColonyPostfix()
        {
            if (ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null && ModLister.GetActiveModWithIdentifier("Paluto22.AK.Compatibility") == null && !DisabledAlienWarning_NewColony)
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("AK_AlienModWarnings".Translate(), delegate
                {
                    DisabledAlienWarning_NewColony = true;
                    SteamUtility.OpenWorkshopPage(new PublishedFileId_t(3204702080));
                }
                ));

            }
        }
    }
    [HarmonyPatch(typeof(Dialog_SaveFileList_Load), "DoFileInteraction")]
    public class Patch_AlienLoadGameDialog
    {
        public static bool DisabledAlienWarning_LoadGame = false;
        [HarmonyPostfix]
        public static void LoadGamePostfix()
        {
            if (ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null && ModLister.GetActiveModWithIdentifier("Paluto22.AK.Compatibility") == null && !DisabledAlienWarning_LoadGame)
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("AK_AlienModWarnings".Translate(), delegate
                {
                    DisabledAlienWarning_LoadGame = true;
                    SteamUtility.OpenWorkshopPage(new PublishedFileId_t(3204702080));
                }
                ));
            }
        }
    }
}
