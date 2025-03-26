using AK_DLL;
using RimWorld;
using UnityEngine;
using Verse;

namespace LMA_Lib
{
    public static class TypeDef
    {
        public static AssetBundle AzurAsset => Utilities_Unity.LoadAssetBundle(AzurDefOf.LMA_Prefab_MainMenu);
    }

    [DefOf]
    public static class AzurDefOf
    {
        public static JobDef LMA_Job_UseRecruitConsole;

        public static UIPrefabDef LMA_Prefab_MainMenu;
        public static UIPrefabDef LMA_Prefab_OpList;
        public static UIPrefabDef LMA_Prefab_OpDetail;
        public static UIPrefabDef LMA_Prefab_Gacha;
    }
}
