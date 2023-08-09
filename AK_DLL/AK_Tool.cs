using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Live2D.Cubism.Core;
using System.Reflection;
using Live2D.Cubism.Framework.Json;
using UnityEngine.UI;
using FS_LivelyRim;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public static class AK_Tool
    {
        public static bool disableIMGUI = false;

        public static AssetBundle FSAsset;
        public static AssetBundle l2dAsset;
        private static GameObject EVSystem;
        public static GameObject EVSystemInstance;

        private static List<ModContentPack> Mods => LoadedModManager.RunningMods.ToList();
        /*public static void PrintfHairColor(this Pawn p)
        {
			Log.Message($"pawnHC: {p.story.HairColor.r}, {p.story.HairColor.g}, {p.story.HairColor.b},{p.story.HairColor.a}");
		}

		public static void PrinftSkinColor(this Pawn p)
        {
			Log.Message($"pawnSC: {p.story.SkinColor.r}, {p.story.SkinColor.g}, {p.story.SkinColor.b},{p.story.SkinColor.a}");
		}*/

        static AK_Tool()
        {
            try
            {
                RIWindowHandler.LoadOperatorSeries();
                RIWindowHandler.LoadOperatorClasses();
                RIWindowHandler.AutoFillOperators();

                InitializeUI();
                Log.Message($"MIS.初始化完成");
            }
            catch
            {
                Log.Error("MIS. Critical Error: Initialization fail");
            }
        }


        //技能残留 早晚给删咯
        public static List<IntVec3> GetSector(OperatorAbilityDef ability, Pawn caster)
        {
            List<IntVec3> result = new List<IntVec3>();
            foreach (IntVec3 intVec3 in GenRadial.RadialCellsAround(caster.Position, ability.sectorRadius, false))
            {
                float mouseAngle = caster.Position.ToIntVec2.ToVector2().AngleTo(Event.current.mousePosition);
                float curAngle = caster.Position.ToVector3().AngleToFlat(intVec3.ToVector3());
                if (intVec3.InBounds(caster.Map) && curAngle > ability.minAngle + mouseAngle && curAngle < ability.maxAngle + mouseAngle)
                {
                    result.Add(intVec3);
                }
            }
            return result;
        }

        private static void InitializeUI()
        {
            for (int i = 0; i < Mods.Count; ++i)
            {
                if (Mods[i].Name == "M.I.S. - Framework")
                {
                    Log.Message(Mods[i].RootDir);
                    FSAsset = AssetBundle.LoadFromFile(Mods[i].RootDir + "/Asset/fsassets");
                    break;
                }
            }
            //FSAsset = AssetBundle.LoadFromFile(@"S:/Program Files (x86)/Steam/steamapps/common/RimWorld/Mods/Framework/Asset/fsassets");
            if (FSAsset == null)
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
        public static OperatorDocument GetDoc(this Pawn p)
        {
            if (p.health.hediffSet.GetFirstHediff<Hediff_Operator>() == null) return null;
            return p.health.hediffSet.GetFirstHediff<Hediff_Operator>().document;
        }
        #endregion

        #region IMGUI渲染
        public static void DrawBottomLeftPortrait()
        {
            if (AK_ModSettings.displayBottomLeftPortrait == false) return;
            if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count == 0) return;
            Pawn p = Find.Selector.SelectedPawns.First();
            if (p == null) return;
            OperatorDocument doc = AK_Tool.GetDoc(p);
            if (doc == null) return;
            Widgets.DrawTextureFitted(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 408, 408), ContentFinder<Texture2D>.Get(AK_Tool.GetDoc(p).operatorDef.stand), (float)AK_ModSettings.ratio * 0.05f);
        }
        public static void DrawHighlightMouseOver(this Rect rect, Texture2D highlight)
        {
            if (Mouse.IsOver(rect))
            {
                GUI.DrawTexture(rect, highlight);
            }
        }
        #endregion

        #region UGUI绘制立绘/L2D
        public static void DrawStaticOperatorStand(OperatorDef def, int preferredSkin, GameObject OpStand, Vector3? offset = null)
        {
            Transform containerLoc = OpStand.transform;
            containerLoc.localPosition = Vector3.zero;
            Image opStand = OpStand.GetComponent<Image>();
            Texture2D tex;

            if (preferredSkin == 0) tex = ContentFinder<Texture2D>.Get(def.commonStand);
            else if (preferredSkin == 1) tex = ContentFinder<Texture2D>.Get(def.stand);
            else tex = ContentFinder<Texture2D>.Get(def.fashion[preferredSkin - 2]);

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

        public static GameObject DrawLive2DOperatorStand(OperatorDef def, int l2dOrder, GameObject renderTarget, Vector3? offset = null)
        {
            if (ModLister.GetActiveModWithIdentifier("FS.LivelyRim") == null)
            {
                Log.Error("MIS. loading a live2d but FS.LivelyRim not found");
                return null;
            }
            GameObject L2DInstance;
            if (l2dOrder >= 1000) l2dOrder -= 1000;
            if (l2dOrder > def.live2dModel.Count)
            {
                Log.Error("MIS. l2d skin out of array");
                return null;
            }
            if (TypeDef.cachedLive2DModels.ContainsKey(def.live2dModel[l2dOrder].modelName))
            {
                L2DInstance = TypeDef.cachedLive2DModels[def.live2dModel[l2dOrder].modelName];
                FS_Tool.SetModelActive(L2DInstance, renderTarget);
            }
            else
            {
                LiveModelDef l2dDef = def.live2dModel[l2dOrder];
                AssetBundle ab = FS_Tool.LoadAssetBundle(l2dDef.modID, l2dDef.assetBundle);
                L2DInstance = FS_Tool.InstantiateLive2DModel(ab, l2dDef.modID, l2dDef.modelName, rigJsonPath: l2dDef.rigJsonPath, renderTargetName: renderTarget, eyeFollowMouse : l2dDef.eyeFollowMouse);
                if (true) //给mod设置预留
                {
                    TypeDef.cachedLive2DModels.Add(l2dDef.modelName, L2DInstance);
                }
            }
            return L2DInstance;
        }

        //我真的不知道为什么这个ev system过一会自己就会变null。
        public static void setEV(bool value)
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
        public static int quickSearch(int[] arr, int leftPtr, int rightPtr, int target, int mode)
        {
            int middle;
            middle = 0;
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

        public static int weightArrayRand(int[] arr)
        {
            int rd = UnityEngine.Random.Range(1, arr.Last());

            return quickSearch(arr, 0, arr.Length - 1, rd, 2);
        }
#endregion
    }
}
