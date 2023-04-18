using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using Verse;
using Verse.AI;
using System.Reflection;

/*namespace AK_DLL
{

    [HarmonyPatch(typeof(ModAssetBundlesHandler), "ReloadAll")]
    public static class PatchReloadAll
	{
        [HarmonyPrefix]
        public static bool prefix(ModAssetBundlesHandler __instance)
        {
			Log.Error("hhhhh");
			System.Reflection.FieldInfo fieldInfo = __instance.GetType().GetField("mod", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			ModContentPack mod;
			mod = (ModContentPack)fieldInfo.GetValue(__instance);

			Log.Error(mod.PackageId);
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(mod.RootDir, "AssetBundles"));
			Log.Error(mod.RootDir);
			if (!directoryInfo.Exists)
			{
				return false;
			}
			FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
			foreach (FileInfo fileInfo in files)
			{
				Log.Error(fileInfo.FullName);
				if (fileInfo.Extension.NullOrEmpty())
				{
					AssetBundle assetBundle = AssetBundle.LoadFromFile(fileInfo.FullName);
					if (assetBundle != null)
					{
						__instance.loadedAssetBundles.Add(assetBundle);
					}
					else
					{
						Log.Error("Could not load asset bundle at " + fileInfo.FullName);
					}
				}
			}
			return false;
		}
    }
}*/