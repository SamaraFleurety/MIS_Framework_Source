using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;
using AKR_Random;
using AK_DLL;
using System.IO;
using AKR_Random.RewardSet;

namespace AKBG_MainmenuBackground
{
    public class AKBG_ModSettings : ModSettings
    {
        public const string Include_Folder_LoadBG = "UI/BG/Load/";
        public const string Include_Folder_MenuBG = "UI/BG/Menu/";
        public const string Include_Folder_Shared = "UI/BG/Shared/";

        public static AKBG_ModSettings instance;

        public static bool enableLoadingBG = false;
        //key是modid, val是图片相对路径
        public static Dictionary<string, HashSet<string>> loadingBGs = new();


        //往下都不再是设置参数
        public static Dictionary<string, HashSet<string>> allPossibleLoadingBGs = null;
        //这里面随出来的结果是绝对路径
        static RandomizerNode_Subnodes randRoot_load = null;
        static string loadingBG_Path; //当前选中的图片的绝对路径
        public static Texture2D LoadingBG_Tex
        {
            get
            {
                InitAllNodes();

                loadingBG_Path = randRoot_load.TryIssueGachaResult().First() as string;
                return Utilities_Unity.GetDynamicLoadTexture(loadingBG_Path, true);
            }
        }

        public static bool enableMainmenuBG = false;
        public static Dictionary<string, HashSet<string>> mainmenuBGs = new();
        public static bool randomMainmenuBG = false;
        public static int randomMainmenuBGInterval = 10; //随机间隔，单位是现实中的秒

        //往下都不再是设置参数
        public static Dictionary<string, HashSet<string>> allPossibleMenuBGs = null;
        static RandomizerNode_Subnodes randRoot_menu = null;
        public static string mainmenuBG_Path = null;
        public static Texture2D MainmenuBG_Tex
        {
            get
            {
                InitAllNodes();

                mainmenuBG_Path ??= randRoot_load.TryIssueGachaResult().First() as string;
                return Utilities_Unity.GetDynamicLoadTexture(mainmenuBG_Path);
            }
        }


        static bool init = false;
        static void InitAllNodes()
        {
            if (init) return;

            bool changed = false;

            InitNodes(ref randRoot_load, loadingBGs, ref changed);
            InitNodes(ref randRoot_menu, mainmenuBGs, ref changed);

            if (changed)
            {
                instance.Write();
            }
            init = true;    
        }

        static void InitNodes(ref RandomizerNode_Subnodes root, Dictionary<string, HashSet<string>> data, ref bool changed)
        {
            List<string> temp = data.Keys.ToList();
            List<string> tempInner;

            root = new();

            foreach (string modID in temp)
            {
                //mod列表变化
                if (!Utilities_Unity.modPath.ContainsKey(modID))
                {
                    changed = true;
                    data.Remove(modID);
                    continue;
                }

                tempInner = data[modID].ToList();

                RandomizerNode_Rewards modNode = new();
                foreach (string path in tempInner)
                {
                    string pathFull = Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(path, modID);
                    if (!File.Exists(pathFull))  //文件不存在
                    {
                        changed = true;
                        data[modID].Remove(path);
                        continue;
                    }
                    else
                    {
                        Rewards_Generic<string> randRes_Path = new()
                        {
                            rewardSingle = pathFull,
                            weight = 1
                        };
                        modNode.rewards.Add(randRes_Path);
                    }
                }

                //不要加入权重为0的空节点
                if (data[modID].Count > 0)
                {
                    modNode.weight = data[modID].Count;
                }
                else
                {
                    changed = true;
                    data.Remove(modID);
                }
            }
        }

        static void SearchAllPossibleBG()
        {
            SearchPossibleBG(ref allPossibleLoadingBGs, BGType.Loading);
            SearchPossibleBG(ref allPossibleMenuBGs, BGType.Mainmenu);
        }

        //自动加载所有可能的背景图片的路径，用于ui
        static void SearchPossibleBG(ref Dictionary<string, HashSet<string>> dict, BGType type) 
        {
            dict = new Dictionary<string, HashSet<string>>();

            foreach (BackgroundDef def in DefDatabase<BackgroundDef>.AllDefs)
            {
                if (!ValidBGDef(type, def)) continue;
                string modid = def.modID;
                modid ??= def.modContentPack.PackageId;

                if (!dict.ContainsKey(modid)) dict.Add(modid, new HashSet<string>());

                string fullPath = Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(def.texturePath, modid);
                if (File.Exists(fullPath))
                {
                    dict[modid].Add(Utilities_Unity.StandardizePath(def.texturePath));
                }
                else
                {
                    Log.Error($"[AKBG] {def.label} 的路径 {fullPath} 不存在对应图片。");
                }
            }

            //自动往文件夹里面装载图片
            foreach (string modid in Utilities_Unity.modPath.Keys)
            {
                string folderPath = type == BGType.Mainmenu ? Include_Folder_MenuBG : Include_Folder_LoadBG;
                DirectoryInfo dirExclusive = new DirectoryInfo(Utilities_Unity.modPath[modid] + folderPath);
                DirectoryInfo dirShared = new DirectoryInfo(Utilities_Unity.modPath[modid] + folderPath);

                List<FileInfo> allFileInfo = new List<FileInfo>();
                if (dirExclusive.Exists)
                {
                    allFileInfo.AddRange(dirExclusive.GetFiles("*.png", SearchOption.AllDirectories));
                }
                if (dirShared.Exists)
                {
                    allFileInfo.AddRange(dirShared.GetFiles("*.png", SearchOption.AllDirectories));
                }

                if (allFileInfo.Count <= 0) continue;

                if (!dict.ContainsKey(modid)) dict.Add(modid, new HashSet<string>());

                foreach (FileInfo file in allFileInfo)
                {
                    string relativePath = Utilities_Unity.DynaLoad_PathFullToRelative<Texture2D>(file.FullName);
                    if (!dict[modid].Contains(relativePath)) dict[modid].Add(relativePath); //这里面一律存的相对路径
                }

            }
        }

        static bool ValidBGDef(BGType type, BackgroundDef def)
        {
            if (type == BGType.Loading && def.loadingBG)
            {
                return true;
            }
            else if (type == BGType.Mainmenu && def.mainmenuBG) return true;

            return false;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look(ref enableLoadingBG, "loadBG", true);
            List<string> key1 = new();
            List<HashSet<string>> val1 = new();
            Scribe_Collections.Look(ref loadingBGs, "loadBGPath", LookMode.Value, LookMode.Deep, ref key1, ref val1);

            Scribe_Values.Look(ref enableMainmenuBG, "mmBG", true);
            List<string> key2 = new();
            List<HashSet<string>> val2 = new();
            Scribe_Collections.Look(ref mainmenuBGs, "mmBGPath", LookMode.Value, LookMode.Deep, ref key2, ref val2);
        }
    }

    public class AKBG_Mod : Mod
    {
        public static AKBG_ModSettings Settings => AKBG_ModSettings.instance;
        public AKBG_Mod(ModContentPack content) : base(content)
        {
            AKBG_ModSettings.instance = GetSettings<AKBG_ModSettings>();
        }
    }
}
