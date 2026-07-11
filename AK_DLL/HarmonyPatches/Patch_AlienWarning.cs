#if V16
using HarmonyLib;
using RimWorld;
using Steamworks;
using System;
using Verse;

namespace AK_DLL
{
    //外星人兼容警告Patch

    [HarmonyPatch]
    public class Patch_AlienWarning
    {
        private static bool _disabledAlienWarningNewColony;
        private static bool _disabledAlienWarningLoadGame;

        [HarmonyPatch(typeof(Page_SelectScenario), nameof(Page_SelectScenario.BeginScenarioConfiguration))]
        [HarmonyPostfix]
        public static void NewColonyPostfix()
        {
            if (ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null && ModLister.GetActiveModWithIdentifier("Paluto22.AK.Compatibility") == null && !_disabledAlienWarningNewColony)
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("AK_AlienModWarnings".Translate(), delegate
                {
                    _disabledAlienWarningNewColony = true;
                    SteamUtility.OpenWorkshopPage(new PublishedFileId_t(3204702080));
                },
                delegate
                {
                    _disabledAlienWarningNewColony = true;
                }
                ));
            }
        }

        [HarmonyPatch(typeof(Dialog_SaveFileList_Load), "DoFileInteraction")]
        [HarmonyPostfix]
        public static void LoadGamePostfix()
        {
            if (ModLister.GetActiveModWithIdentifier("erdelf.HumanoidAlienRaces") != null && ModLister.GetActiveModWithIdentifier("Paluto22.AK.Compatibility") == null && !_disabledAlienWarningLoadGame)
            {
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("AK_AlienModWarnings".Translate(), delegate
                {
                    _disabledAlienWarningLoadGame = true;
                    SteamUtility.OpenWorkshopPage(new PublishedFileId_t(3204702080));
                },
                delegate
                {
                    _disabledAlienWarningLoadGame = true;
                }
                ));
            }
        }
    }
}
#endif