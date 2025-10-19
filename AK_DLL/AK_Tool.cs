using AK_DLL.UI;
using AK_TypeDef;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public static class AK_Tool
    {
        public static bool disableIMGUI = false;

        public static AssetBundle FSAsset => Utilities_Unity.LoadAssetBundle(AKDefOf.AK_Prefab_yccMainMenu);
        public static AssetBundle PAAsset;
        public static AssetBundle l2dAsset;
        private static GameObject EVSystem;
        public static GameObject EVSystemInstance;

        public static bool Live2DActivated => ModLister.GetActiveModWithIdentifier("FS.LivelyRim") != null;

        private static List<ModContentPack> Mods => LoadedModManager.RunningMods.ToList();

        static AK_Tool()
        {
            try
            {

                if (AK_ModSettings.debugOverride) Log.Message("[AK] loading");
                RIWindowHandler.LoadOperatorSeries();
                if (AK_ModSettings.debugOverride) Log.Message($"AK loaded {RIWindowHandler.operatorSeries.Count} series");
                if (AK_ModSettings.debugOverride) Log.Message("AK loading classes");
                RIWindowHandler.LoadOperatorClasses();
                if (AK_ModSettings.debugOverride) Log.Message("AK loading operators");
                RIWindowHandler.AutoFillOperators();

                InitializeUI();

                Log.Message($"MIS.初始化完成");
            }
            catch (Exception ex)
            {
                Log.Error($"MIS. Critical Error: Initialization fail : {ex.Message} + {ex.StackTrace}");
            }
        }

        private static void InitializeUI()
        {
            for (int i = 0; i < Mods.Count; ++i)
            {
                if (/*Mods[i].Name == "M.I.S. - Framework" ||*/ Mods[i].PackageId == TypeDef.ModID.ToLower())
                {
                    Log.Message(Mods[i].RootDir);
                    //FSAsset = AssetBundle.LoadFromFile(Mods[i].RootDir + "/Asset/fsassets");
                    PAAsset = AssetBundle.LoadFromFile(Mods[i].RootDir + "/Asset/paassets");
                    break;
                }
            }
            if (!FSAsset || !PAAsset)
            {
                Log.Error("MIS. Critical Error: Missing Assets");
            }
            else if (Prefs.DevMode)
            {
                Log.Message("MIS. Assets Initialized");
            }

            EVSystem = FSAsset.LoadAsset<GameObject>("EventSystem");

        }

        #region 字符串处理，文档/干员ID/def转换
        public static string GetPrefixFrom(string XMLdefName)
        {
            string[] temp = XMLdefName.Split('_');
            if (temp.Length <= 1) return null;
            return temp[0];
        }
        public static string GetOperatorIDFrom(string XMLdefName)
        {
            string[] temp = XMLdefName.Split('_');
            if (temp.Length <= 1) return null;
            return temp[temp.Length - 1];
        }

        //要求defName格式：前缀_随便啥_人物名；返回物品defName格式：前缀_{string thingType}_人物名
        public static string GetThingdefNameFrom(string XMLdefName, string thingType)
        {
            string[] temp = XMLdefName.Split('_');
            if (temp.Length <= 1) return null;

            return temp[0] + "_" + thingType + "_" + temp[temp.Length - 1];
        }

        public static string GetThingdefNameFrom(string operatorID, string prefix, string thingType)
        {
            return prefix + "_" + thingType + "_" + operatorID;
        }

        public static OperatorDef GetDef(OperatorClassDef classDef, string operatorID)
        {
            return RIWindowHandler.operatorDefs[classDef.sortingOrder][operatorID];
        }
        public static OperatorDef GetDef(int operatorClass, string operatorID)
        {
            return RIWindowHandler.operatorDefs[operatorClass][operatorID];
        }
        public static OperatorDef GetDef(string operatorID)
        {
            foreach (Dictionary<string, OperatorDef> i in RIWindowHandler.operatorDefs.Values)
            {
                if (i.ContainsKey(operatorID)) return i[operatorID];
            }
            return null;
        }

        public static OperatorDocument GetDoc(this OperatorDef def)
        {
            if (GC_OperatorDocumentation.opDocArchive == null) return null;
            GC_OperatorDocumentation.opDocArchive.TryGetValue(def.OperatorID, out OperatorDocument doc);
            return doc;
        }

        public static OperatorDocument GetDoc(string ID)
        {
            if (GC_OperatorDocumentation.opDocArchive == null) return null;
            GC_OperatorDocumentation.opDocArchive.TryGetValue(ID, out OperatorDocument doc);
            return doc;
        }
        //只有OpDocContainer才有Doc
        public static OperatorDocument GetDoc(this Pawn p)
        {
            if (OperatorDef.currentlyGenerating) return null;
            if (p == null) return null;

            OperatorDocument doc = p.TryGetDoc<OpDocContainer>()?.va?.Document;

            if (doc != null) return doc;

            if (p.abilities == null) return null;
            if (!p.IsColonist) return null;
            if (GC_OperatorDocumentation.cachedOperators.ContainsKey(p))
            {
                return GC_OperatorDocumentation.cachedOperators[p];
            }
            else if (GC_OperatorDocumentation.cachedNonOperators.Contains(p))
            {
                return null;
            }
            VAbility_Operator va = null;
            foreach (Ability i in p.abilities.abilities)
            {
                if (i is VAbility_Operator vab)
                {
                    //Log.Message($"g1 at {p.Name}, {i.GetType()}, {i.def.defName}");
                    va = vab;
                    GC_OperatorDocumentation.cachedOperators.Add(p, va.Document);
                    p.AddDoc(new OpDocContainer() { parent = p, va = vab });
                    break;
                }
            }
            if (va == null)
            {
                GC_OperatorDocumentation.cachedNonOperators.Add(p);
                return null;
            }
            doc = va.Document;
            return doc;
        }
        #endregion

        #region IMGUI渲染
        /*public static void DrawHighlightMouseOver(this Rect rect, Texture2D highlight)
        {
            if (Mouse.IsOver(rect))
            {
                GUI.DrawTexture(rect, highlight);
            }
        }*/
        #endregion

        #region UGUI绘制立绘/L2D
        //普通的图片立绘
        public static void DrawStaticOperatorStand(OperatorDef def, int preferredSkin, GameObject OpStand, Vector3? offset = null)
        {
            Transform containerLoc = OpStand.transform;
            containerLoc.localPosition = Vector3.zero;
            Image opStand = OpStand.GetComponent<Image>();
            Texture2D tex = def.PreferredStand(preferredSkin);

            /*if (preferredSkin == 0) tex = ContentFinder<Texture2D>.Get(def.commonStand);
            else if (preferredSkin == 1) tex = ContentFinder<Texture2D>.Get(def.stand);
            else tex = ContentFinder<Texture2D>.Get(def.fashion[preferredSkin - 2]);*/

            opStand.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);

            if (offset == null && def.standOffsets != null && def.standOffsets.ContainsKey(preferredSkin))
            {
                offset = def.standOffsets[preferredSkin];
            }

            if (offset is Vector3 rOffset)
            {
                containerLoc.localPosition = new Vector3(rOffset.x, rOffset.y);
                containerLoc.localScale = new Vector3(rOffset.z, rOffset.z, rOffset.z);
            }

        }

        //我真的不知道为什么这个ev system过一会自己就会变null。
        public static void SetEV(bool value)
        {
            if (value)
            {
                if (EVSystemInstance == null) EVSystemInstance = GameObject.Instantiate(EVSystem);
                EVSystemInstance.SetActive(true);
            }
            else EVSystemInstance.SetActive(false);
        }
        #endregion

        #region 算法（自己造轮子）
        //模式1是精确搜索，2是找第一个更大值，3是找第一个更小值
        public static int QuickSearch(int[] arr, int leftPtr, int rightPtr, int target, int mode)
        {
            int middle;
            if (rightPtr - leftPtr < 0) return -1;
            if (target > arr[rightPtr])
            {
                if (mode == 3) return rightPtr;
                else return -1;
            }
            else if (target < arr[leftPtr])
            {
                if (mode == 2) return leftPtr;
                else return -1;
            }
            while (rightPtr >= leftPtr)
            {
                middle = (leftPtr + rightPtr) / 2;
                if (arr[middle] == target)
                {
                    return middle;
                }
                else if (leftPtr == rightPtr) break;
                else if (arr[middle] < target)
                {
                    leftPtr = middle + 1;
                }
                else
                {
                    rightPtr = middle;
                }
            }
            if (mode == 2) return rightPtr;
            else if (mode == 3) return rightPtr - 1;
            else return -1;
        }

        public static int WeightArrayRand(int[] arr)
        {
            int rd = UnityEngine.Random.Range(1, arr.Last()); //值域是[min, max]

            return QuickSearch(arr, 0, arr.Length - 1, rd, 2);
        }

        public static string DescriptionManualResolve(string s, string name, Gender gender)
        {
            s = Regex.Replace(s, @"\{PAWN_nameDef\}|\[PAWN_nameDef\]", name);
            s = Regex.Replace(s, @"\{PAWN_pronoun\}|\[PAWN_pronoun\]", GenderUtility.GetPronoun(gender));
            s = Regex.Replace(s, @"\{PAWN_objective\}|\[PAWN_objective\]", GenderUtility.GetObjective(gender));
            s = Regex.Replace(s, @"\{PAWN_possessive\}|\[PAWN_possessive\]", GenderUtility.GetPossessive(gender));
            return s;
        }
        #endregion
    }
}
