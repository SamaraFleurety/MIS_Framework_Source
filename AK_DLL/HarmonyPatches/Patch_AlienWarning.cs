using HarmonyLib;
using RimWorld;
using Steamworks;
using Verse;

namespace AK_DLL
{
    //外星人兼容警告Patch
    [HarmonyPatch(typeof(Page_SelectScenario), "BeginScenarioConfiguration")]
    public class Patch_AlienNewGameDialog
    {
        private static bool _disabledAlienWarningNewColony;

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
    }

    [HarmonyPatch(typeof(Dialog_SaveFileList_Load), "DoFileInteraction")]
    public class Patch_AlienLoadGameDialog
    {
        private static bool _disabledAlienWarningLoadGame;

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
