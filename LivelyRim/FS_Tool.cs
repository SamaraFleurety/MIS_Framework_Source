using System;
using System.Reflection;
using Verse;
using RimWorld;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Live2D.Cubism.Core;
using Live2D.Cubism.Rendering;
using Live2D.Cubism.Rendering.Masking;
using Live2D.Cubism.Framework.Physics;
using Live2D.Cubism.Framework.Json;
using System.IO;
using LitJson;
using System.Collections.Generic;
using System.Linq;
using Live2D.Cubism.Framework.LookAt;
using Live2D.Cubism.FSAddon;

namespace FS_LivelyRim
{
    [StaticConstructorOnStartup]
    public static class FS_Tool
    {
        //存所有mod的路径 <packageID, 路径>
        public static Dictionary<string, string> modPath = new Dictionary<string, string>();

        static string ModID => TypeDef.ModID;

        static AssetBundle l2dResource => TypeDef.l2dResource;

        //执行所有初始化 FIXME：记得写个IO装载原生库
        static FS_Tool ()
        {
            InitializeCubismDll();

            //因为广泛需要读取prefab或者json，需要知道路径。泰南的MODContentInfo竟然tm是个list
            LoadAllModPath();

            TypeDef.Initialize();
        }

        static void LoadAllModPath()
        {
            List<ModContentPack> Mods = LoadedModManager.RunningMods.ToList();
            for (int i = 0; i < Mods.Count; ++i)
            {
                modPath.Add(Mods[i].PackageId, Mods[i].RootDir);
            }
        }

        //因为cubism dll是在游戏中途加载，所以有些仅游戏开始时执行一次的初始化方法无法被执行
        static void InitializeCubismDll()
        {
            MethodInfo method = typeof(CubismModel).GetMethod("RegisterCallbackFunction", BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, new object[0]);

            method = typeof(CubismLogging).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, new object[0]);
        }

        //从mod的ID读获取要的文件的路径
        //实际路径为[MOD根目录]/[subfolder]/path
        private static string ModIDtoPath(string modPackageID, string path, string subfolder = "")
        {
            return modPath[modPackageID.ToLower()] + subfolder + path;
        }

        /// <summary>
        /// 从mod根目录/Asset/开始索引。假设需要读根目录/Aseet/ab，那就入参mod的packageID 和 "Aseet/ab"
        /// </summary>
        /// <param name="modPackageID">写在About.xml里面的PackageID</param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string modPackageID, string path)
        {
            AssetBundle assetBundle = null;
            //string fullPath = modPath[modPackageID.ToLower()] + "/Asset/" + path;
            string fullPath = ModIDtoPath(modPackageID, path, "/Asset/");
            try
            {
                assetBundle = AssetBundle.LoadFromFile(fullPath);
            }
            catch
            {
                Log.Error($"Unable to load assetbundle at {fullPath}");
            }
            return assetBundle;
        }

        //从mod根目录/Json/开始索引 这个是手动加载模型Json用的。
        public static CubismModel3Json LoadCubismJson(string modPackageID, string path)
        {
            CubismModel3Json json = null;
            //string fullPath = modPath[modPackageID.ToLower()] + "/Json/" + path;
            string fullPath = ModIDtoPath(modPackageID, path, "/Json/");
            try
            {
                json = CubismModel3Json.LoadAtPath(fullPath, BuiltinLoadAssetAtPath);
            }
            catch
            {
                Log.Error($"Unable to load Json at {fullPath}");
                return null;
            }
            return json;
        }

        //从mod根目录/Json/开始索引 这个是手动加载模型的Rig的Json用的
        public static string LoadJson(string modPackageID, string path)
        {
            string fullPath = ModIDtoPath(modPackageID, path, "/Json/");
            return File.ReadAllText(fullPath);
        }

        /// <summary>
        /// 从Json手动加载物理rig到模型上。我也不知道为什么原版读ab包时无法处理有嵌套的
        /// </summary>
        /// <param name="Live2DModel"></param>
        /// <param name="modPackageID"></param>
        /// <param name="JsonPath"></param>
        public static void LoadRigtoModel(GameObject Live2DModel, string modPackageID, string JsonPath)
        {
            CubismPhysics3Json physics3Json = LitJson.JsonMapper.ToObject<CubismPhysics3Json>(LoadJson(modPackageID, JsonPath));

            CubismPhysicsController physicsController = Live2DModel.GetComponent<CubismPhysicsController>();

            if (physicsController == null)
            {
                Log.Error("[FS.L2D] model do not have component CubismPhysicsController");
                return;
            }

            physicsController.Initialize(physics3Json.ToRig());

            physicsController.HasUpdateController = true;
        }

        /// <summary>
        /// 从AB包读取模型。如果希望自己完成后续处理就用这个。在使用此方法前需要提前实例化离屏渲染目标。
        /// </summary>
        /// <param name="AB"></param>
        /// <param name="path"></param>
        /// <param name="renderTargetName">模型采用离屏渲染规避泰南的UI。不能为空，需要提前实例化离屏渲染目标，不然别问我为什么看不到模型。</param>
        /// <returns></returns>
        public static GameObject LoadModelfromAB(AssetBundle AB, string path, string renderTargetName = null)
        {
            GameObject l2dPrefab = AB.LoadAsset<GameObject>(path);
            GameObject l2dInstance = GameObject.Instantiate(l2dPrefab);
            if (renderTargetName != null)
            {
                l2dInstance.GetComponent<OffscreenRendering>().renderTargetName = renderTargetName;
            }
            return l2dInstance;
        }

        /// <summary>
        /// 给定所有必要参数，返回实例化的L2D模型
        /// </summary>
        /// <param name="assetBundle"> 装有模型的AB包。需要提前使用LoadAssetBundle方法读取。</param>
        /// <param name="ModID"> 模型MOD的PackageID </param>
        /// <param name="modelPath"> L2D模型在AB包中的路径和名字 </param>
        /// <param name="rigJsonPath"> [可选]L2D模型在Json文件夹中的路径和名字 </param>
        /// <param name="renderTargetName"> [可选，带默认值] 希望把L2D模型渲染在哪个游戏物体上。自定义游戏物体必须带有UnityEngine.UI.Image组件，且Spirit需要留空。
        ///                                                  默认会实例化一个符合条件的内置的prefab。</param>
        /// <param name="eyeFollowMouse">[默认为是]模型是否带有眼睛和头的追踪功能</param>
        /// <param name="eyeFollowTarget">[可选，带默认值]如果有头眼追踪功能，追踪的物体。默认是跟随鼠标，并且有速度限制。</param>
        /// <returns>返回完成以上处理的游戏物体，如果有其他需求可自行更改。</returns>
        public static GameObject InstantiateLive2DModel(AssetBundle assetBundle, string ModID, string modelPath, string rigJsonPath = null, string renderTargetName = null, bool eyeFollowMouse = true, GameObject eyeFollowTarget = null)
        {
            GameObject renderTarget = GameObject.Find("L2DRenderTarget");
            if (renderTarget == null && renderTargetName == null)
            {
                GameObject renderTargetPrefab = l2dResource.LoadAsset<GameObject>("L2DCanvas");
                GameObject renderTargetInstance = GameObject.Instantiate(renderTargetPrefab);
            }

            GameObject l2dIns = LoadModelfromAB(assetBundle, modelPath, renderTargetName);

            if (rigJsonPath != null)
            {
                try
                {
                    LoadRigtoModel(l2dIns, ModID, rigJsonPath);
                }
                catch
                {
                    Log.Error("[FS.L2D] error when loading rig to model.");
                }
            }


            if (eyeFollowMouse)
            {
                if (eyeFollowTarget != null)
                {
                    l2dIns.GetComponent<CubismLookController>().Target = eyeFollowTarget;
                }
                else
                {
                    GameObject eyeTargetInstance = GameObject.Find("EyeTarget");
                    if (eyeTargetInstance == null)
                    {
                        GameObject eyeTargetPrefab = l2dResource.LoadAsset<GameObject>("EyeTarget");
                        eyeTargetInstance = GameObject.Instantiate(eyeTargetPrefab);
                    }
                    l2dIns.GetComponent<CubismLookController>().Target = eyeTargetInstance;
                }
            }

            return l2dIns;
        }

       /* public static void DrawLive()
        {

            if (true)
            {


                GameObject l2dins = LoadModelfromAB(TypeDef.ricepicotest, "rice_pro_t03Motion");
                l2dins.GetComponent<OffscreenRendering>().renderTarget = GameObject.Find("AG");

                LoadRigtoModel(l2dins, ModID, "rice_pro_t03.physics3.json");


                GameObject eyeTargetPrefab = TypeDef.ricepicotest.LoadAsset<GameObject>("EyeTarget");
                GameObject eyeTargetIns = GameObject.Instantiate(eyeTargetPrefab);
                l2dins.GetComponent<CubismLookController>().Target = eyeTargetIns;
            }
            //l2dins.SetActive(true);
            //Log.Message($"{l2dins.GetComponentInChildren<CubismPart>() == null}");

        }*/
        public static object BuiltinLoadAssetAtPath(Type assetType, string absolutePath)
        {
            Debug.Log(absolutePath);
            if (assetType == typeof(byte[]))
            {
                return File.ReadAllBytes(absolutePath);
            }
            else if (assetType == typeof(string))
            {
                return File.ReadAllText(absolutePath);
            }
            else if (assetType == typeof(Texture2D))
            {
                var texture = new Texture2D(1, 1);
                texture.LoadImage(File.ReadAllBytes(absolutePath));
                return texture;
            }
            throw new NotSupportedException();
        }

    }
}
