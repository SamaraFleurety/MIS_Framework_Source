using System.Collections.Generic;
using System.Linq;
using Verse;
using UnityEngine;
using AKR_Random;
using AK_DLL;
using System.IO;
using AKR_Random.RewardSet;
using System.Collections.Generic.RedBlack;

namespace AKBG_MainmenuBackground
{
    public class BackgroundMod_Tableview_Setting : IExposable
    {
        public class ModContentPack_BG  //写到这玩意存读档
        {
            string modid;

            Dictionary<string, TexturePathProperties> allBGs = new();
            //为可调顺序播放做的优化
            TexturePathProperties firstBG = null;
            TexturePathProperties lastBG = null;

            public ModContentPack_BG(string modid)
            {
                this.modid = modid;
            }

            public bool BGIDExist(string BGUniqueID)
            {
                return allBGs.ContainsKey(BGUniqueID);
            }
            public void InsertAtTail(TexturePathProperties prop)
            {
                if (BGIDExist(prop.GetUniqueLoadID())) return;

                allBGs.Add(prop.GetUniqueLoadID(), prop);
                if (firstBG == null)  //显然不可能first和last仅其一是null
                {
                    firstBG = lastBG = prop;
                    return;
                }
                else
                {
                    var lastNode_Old = lastBG;
                    lastBG.next = prop;
                    prop.prev = lastNode_Old;
                }
            }

            public void RemoveNode(string id)
            {
                if (!BGIDExist(id)) return;

                TexturePathProperties nodeToRemove = allBGs[id];

                //仅有这一个
                if (firstBG == lastBG && firstBG == nodeToRemove)
                {
                    firstBG = lastBG = null;
                }
                else if (firstBG == nodeToRemove) //这是第一个
                {
                    var next = nodeToRemove.next;
                    firstBG = next;
                    next.prev = null;
                }
                else if (lastBG == nodeToRemove) //芝士最后一个
                {
                    var prev = nodeToRemove.prev;
                    lastBG = prev;
                    prev.next = null; 
                }
                else  //两面包夹芝士
                {
                    var next = nodeToRemove.next;
                    var prev = nodeToRemove.prev;
                    prev.next = next;
                    next.prev = prev;
                }
                allBGs.Remove(id);
            }

            public void SwapNode(TexturePathProperties nodeA, TexturePathProperties nodeB)
            {
                TexturePathProperties temp;
                temp = nodeA.prev;
                nodeA.prev = nodeB.prev;
                nodeB.prev = temp;

                temp = nodeB.next;
                nodeB.next = nodeA.next;
                nodeA.next = temp;
            }
        }

        public class TexturePathProperties : IExposable, ILoadReferenceable
        {
            string modid;

            string path;  //也是id 同mod肯定唯一
            bool enabled = true;
            int weight = 1;

            //伪链表实现
            public TexturePathProperties prev = null;
            public TexturePathProperties next = null;

            public void ExposeData()
            {
                Scribe_Values.Look(ref modid, "modid");
                Scribe_Values.Look(ref path, "path");
                Scribe_Values.Look(ref enabled, "enabled", true);
                Scribe_Values.Look(ref weight, "weight", 1);

                Scribe_References.Look(ref prev, "prev");
                Scribe_References.Look(ref next, "next");
            }

            public string GetUniqueLoadID()
            {
                return modid + path;
            }
        }

        #region 字段
        [DefaultValue(false)]
        [Description("是否启动")]
        public bool enable;

        [DefaultValue(true)]
        [Description("是否随机播放")]
        public bool randomPlay;

        [DefaultValue(0)]
        [Description("图片播放间隔,单位秒")]
        public float randomPlayInterval;

        //顺序播放间隔
        public float sequentialPlayInterval;

        //[Description("存储的用于播放的图片")]
        //public List<Texture2D> TextureList_Play;

        //外部key是modid；内部key是unique id(真有个函数叫这个名), 内部val是图片参数
        //启用的背景图片必须以路径形式写入存档，因为加载游戏的时候压根没加载def
        public RedBlackTree<string, RedBlackTree<string, TexturePathProperties>> enabledBGs = new();

        #region 保存
        //不是常见的存读档流程。如果不熟悉此机制勿改。
        List<TexturePathProperties> allBGListForSave = new();

        void ExposeData_PreSave()
        {
            allBGListForSave = new();
            foreach (var modid in allPossibleBGs.Keys)
            {
                foreach (var innerPath in allPossibleBGs[modid].Keys)
                {
                    allBGListForSave.Add(allPossibleBGs[modid][innerPath]);
                }
            }
        }

        void ExposeData_PostLoad()
        {
        }
        public void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving) ExposeData_PreSave();

            Scribe_Values.Look(ref enable, "enable");
            Scribe_Values.Look(ref randomPlay, "rand");
            Scribe_Values.Look(ref randomPlayInterval, "randInterval");

            if (Scribe.mode == LoadSaveMode.PostLoadInit) ExposeData_PostLoad();
        }

        #endregion

        //往下数据都不保存
        //只可能在mod设置里调用, 每次开游戏不会变化
        Dictionary<string, Dictionary<string, TexturePathProperties>> allPossibleBGs = null;
        public Dictionary<string, Dictionary<string, TexturePathProperties>> AllPossibleBGs
        {
            get
            {
                if (allPossibleBGs == null) { SearchPossibleBG(); }

                return allPossibleBGs;
            }
        }

        public const string Include_Folder_LoadBG = "UI/BG/Load/";
        public const string Include_Folder_MenuBG = "UI/BG/Menu/";
        public const string Include_Folder_Shared = "UI/BG/Shared/";

        BGType settingType;

        bool init = false;

        //上面enableBGs那个参数实际转成随机器
        RandomizerNode_Subnodes randerRoot = null; // == randomizer
        static string crtBGPath = null; //crt == current
        #endregion

        public Texture2D CrtBG_Tex
        {
            get
            {
                InitRandomizer();

                crtBGPath ??= randerRoot.TryIssueGachaResult().First() as string;
                return Utilities_Unity.GetDynamicLoadTexture(crtBGPath);
            }
        }
        /*public BackgroundMod_Tableview_Setting(bool IsApply = false, bool IsRandomPlay = true, float SecondsPerImage = 0, List<Texture2D> TextureList = null)
        {
            this.IsApply = IsApply;
            this.IsRandomPlay = IsRandomPlay;
            this.SecondsPerImage = SecondsPerImage;
            this.TextureList_Play = TextureList ?? new List<Texture2D>();
        }*/

        public BackgroundMod_Tableview_Setting(BGType settingType)
        {
            this.settingType = settingType;
        }

        //初始化随机器的节点
        //这个函数会把enableBGs转化成随机器，并且会自动筛选不合格的路径。但这个函数不可能增加enableBGs
        private void InitRandomizer(/*ref RandomizerNode_Subnodes root, Dictionary<string, HashSet<string>> data, ref bool changed*/)
        {
            if (init) return;
            init = true;

            bool changed = false;
            List<string> temp = enabledBGs.Keys.ToList();
            List<string> tempInner;

            randerRoot = new();

            foreach (string modID in temp)
            {
                //mod列表变化
                if (!Utilities_Unity.modPath.ContainsKey(modID))
                {
                    changed = true;
                    enabledBGs.Remove(modID);
                    continue;
                }

                tempInner = enabledBGs[modID].ToList();

                RandomizerNode_Rewards modNode = new();
                foreach (string path in tempInner)
                {
                    string pathFull = Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(path, modID);
                    if (!File.Exists(pathFull))  //文件不存在
                    {
                        changed = true;
                        enabledBGs[modID].Remove(path);
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
                if (enabledBGs[modID].Count > 0)
                {
                    modNode.weight = enabledBGs[modID].Count;
                }
                else
                {
                    changed = true;
                    enabledBGs.Remove(modID);
                }
            }

            //如果enableBGs变化了(仅可能变少),写入mod设置
            if (changed)
            {

            }
        }

        //自动加载所有可能的背景图片的路径，用于ui
        //只可能在mod设置里调用，这时候def皆已经完全读取
        void SearchPossibleBG(/*ref Dictionary<string, HashSet<string>> dict, BGType type*/)
        {
            if (allPossibleBGs != null)
            {
                return;
            }
            BGType type = this.settingType;

            allPossibleBGs = new();

            foreach (var )

                foreach (BackgroundDef def in DefDatabase<BackgroundDef>.AllDefs)
                {
                    if (!ValidBGDef(type, def)) continue;
                    string modid = def.modID;
                    modid ??= def.modContentPack.PackageId;

                    if (!allPossibleBGs.ContainsKey(modid)) allPossibleBGs.Add(modid, new());

                    string fullPath = Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(def.texturePath, modid);
                    if (File.Exists(fullPath))
                    {
                        allPossibleBGs[modid].Add(Utilities_Unity.StandardizePath(def.texturePath));
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

                if (!allPossibleBGs.ContainsKey(modid)) allPossibleBGs.Add(modid, new HashSet<string>());

                foreach (FileInfo file in allFileInfo)
                {
                    string relativePath = Utilities_Unity.DynaLoad_PathFullToRelative<Texture2D>(file.FullName);
                    if (!allPossibleBGs[modid].Contains(relativePath)) allPossibleBGs[modid].Add(relativePath); //这里面一律存的相对路径
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
        }

        //仅在mod设置里，选择完毕并且按下保存配置时调用
        public void ApplyEnabledBGList()
        {
            enabledBGs = new();

            foreach (string modid in allPossibleBGs.Keys)
            {
                int cnt = 0;
                enabledBGs.Add(modid, new());

                if (cnt == 0) enabledBGs.Remove(modid);
            }
        }

    }

    public class AKBG_ModSettings : ModSettings
    {

        public static AKBG_ModSettings instance;

        /*public static bool enableLoadingBG = false;
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

        /*public static void SearchAllPossibleBG()
        {
            SearchPossibleBG(ref allPossibleLoadingBGs, BGType.Loading);
            SearchPossibleBG(ref allPossibleMenuBGs, BGType.Mainmenu);
        }

        //自动加载所有可能的背景图片的路径，用于ui
        //只可能在mod设置里调用，这时候def皆已经完全读取
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
        }*/

        public static BackgroundMod_Tableview_Setting settingsLoadingBG = new BackgroundMod_Tableview_Setting(BGType.Loading);
        public static BackgroundMod_Tableview_Setting settingsMainMenuBG = new(BGType.Mainmenu);

        public override void ExposeData()
        {
            Scribe_Deep.Look(ref settingsLoadingBG, "setld", BGType.Loading);
            Scribe_Deep.Look(ref settingsMainMenuBG, "setmm", BGType.Mainmenu);
            /*Scribe_Values.Look(ref enableLoadingBG, "loadBG", true);
            List<string> key1 = new();
            List<HashSet<string>> val1 = new();
            Scribe_Collections.Look(ref loadingBGs, "loadBGPath", LookMode.Value, LookMode.Deep, ref key1, ref val1);

            Scribe_Values.Look(ref enableMainmenuBG, "mmBG", true);
            List<string> key2 = new();
            List<HashSet<string>> val2 = new();
            Scribe_Collections.Look(ref mainmenuBGs, "mmBGPath", LookMode.Value, LookMode.Deep, ref key2, ref val2);*/
        }
    }

    public class AKBG_Mod : Mod
    {
        public static AKBG_ModSettings Settings => AKBG_ModSettings.instance;
        public AKBG_Mod(ModContentPack content) : base(content)
        {
            AKBG_ModSettings.instance = GetSettings<AKBG_ModSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard list = new Listing_Standard();
            list.Begin(inRect);
            if (list.ButtonTextLabeled("text".Translate(), "Open".Translate()))
            {
                Find.WindowStack.Add(new Window_BackgroundMod());
            }
            list.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "AKBG";
        }
    }
}
