using AK_DLL;
using AKR_Random;
using RimWorld;
using UnityEngine;
using Verse;

namespace LMA_Lib
{
    public static class TypeDef
    {
        public static AssetBundle AzurAsset => AzurDefOf.LMA_Prefab_MainMenu.LoadAssetBundle();
    }

    [DefOf]
    public static class AzurDefOf
    {
        public static JobDef LMA_Job_UseRecruitConsole;
        public static JobDef LMA_Job_UseGachaConsole;
        public static JobDef LMA_Job_StoreSilver;

        public static UIPrefabDef LMA_Prefab_MainMenu;
        public static UIPrefabDef LMA_Prefab_OpList;
        public static UIPrefabDef LMA_Prefab_OpDetail;
        public static UIPrefabDef LMA_Prefab_Gacha;

        public static RandomizerDef LMA_Rander_Operators;

        static AzurDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(AzurDefOf));
        }
    }
}
