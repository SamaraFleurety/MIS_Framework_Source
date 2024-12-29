using HarmonyLib;
using System.Reflection;
using UnityEngine.SceneManagement;
using UnityExplorer;
using Verse;

namespace AKUL_UnityExplorerLoader
{
    public class ModSettings : Mod
    {
        static bool loaded = false;
        public ModSettings(ModContentPack content) : base(content)
        {
            SceneManager.sceneLoaded += SceneLoaded;
        }

        public static void SceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (loaded) return;
            loaded = true;
            var har = new Harmony("AKUL");
            har.PatchAll(Assembly.GetExecutingAssembly());

            ExplorerStandalone.CreateInstance();

            return;
        }
    }
}
