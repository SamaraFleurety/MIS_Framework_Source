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


//fixme:静态存个canvas得了
namespace FS_LivelyRim
{
    [StaticConstructorOnStartup]
    public static class FS_Tool
    {
        //存所有mod的路径 <packageID, 路径>
        public static Dictionary<string, string> modPath = new Dictionary<string, string>();

        static string ModID => TypeDef.ModID; //本mod的id

        static AssetBundle l2dResource => TypeDef.l2dResource;

        //public static GameObject l2dPrefab;
        public static GameObject defaultModelInstance;
        public static LiveModelDef l2dDef;

        public static GameObject defaultCanvas;
        public static GameObject defaultRenderTarget;

        public static RenderTexture OffScreenCameraRenderTarget => OffscreenRendering.OffCamera.targetTexture;

        //执行所有初始化 原生库只有x64
        static FS_Tool ()
        {
            //因为广泛需要读取prefab或者json，需要知道路径。泰南的MODContentInfo竟然tm是个list
            LoadAllModPath();

            CheckCubismCoreLib();

            InitializeCubismDll();

            TypeDef.Initialize();

            SetDefaultCanvas(false);
        }

        #region IO
        static void LoadAllModPath()
        {
            List<ModContentPack> Mods = LoadedModManager.RunningMods.ToList();
            for (int i = 0; i < Mods.Count; ++i)
            {
                modPath.Add(Mods[i].PackageId, Mods[i].RootDir);
            }
        }
        
        //自动装载原生库。很可能不兼容x86/mac/linux
        static void CheckCubismCoreLib()
        {
            if (FS_ModSettings.autofillCubismCoreLib)
            {
                string targetPath = Application.dataPath + "/Plugins/x86_64/Live2DCubismCore.dll";
                string oriPath = ModIDtoPath(ModID, "Live2DCubismCore.dll", "/CoreLib/Windows/x86_64/");
                if (!File.Exists(targetPath))
                {
                    File.Copy(oriPath, targetPath);
                }
                FS_ModSettings.autofillCubismCoreLib = false;
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
        //实际路径为[MOD根目录][subfolder]path
        //subfolder前后应该包括'/'
        private static string ModIDtoPath(string modPackageID, string path, string subfolder = "")
        {
            return modPath[modPackageID.ToLower()] + subfolder + path;
        }

        /// <summary>
        /// 从mod根目录/Asset/开始索引。假设需要读根目录/Aseet/ab，那就入参mod的packageID 和 "Aseet/ab"
        /// </summary>
        /// <param name="modPackageID">写在About.xml里面的PackageID</param>
        /// <param name="path"></param>
        /// <param name="AssetBundleID">如果path和ab包实际名字不一致，需要输入想使用的名字</param>>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string modPackageID, string path, string AssetBundleID = null)
        {
            AssetBundle assetBundle = null;

            if (AssetBundleID == null) AssetBundleID = path;

            if (TypeDef.cachedAssetBundle.ContainsKey(AssetBundleID)) return TypeDef.cachedAssetBundle[AssetBundleID];

            string fullPath = ModIDtoPath(modPackageID, path, "/Asset/");
            Log.Message(fullPath);
            try
            {
                assetBundle = AssetBundle.LoadFromFile(fullPath);
                if (assetBundle != null)
                {
                    //记录这次加载的ab包
                    TypeDef.cachedAssetBundle.Add(AssetBundleID, assetBundle);
                }
                else
                {
                    Log.Error($"FS.L2D. Unable to load assetbundle at {fullPath}");
                    return null;
                }
            }
            catch
            {
                Log.Error($"FS.L2D. Unable to load assetbundle at {fullPath}");
                return null;
            }
            return assetBundle;
        }

        /// <summary>
        /// 对于已经加载的ab包，直接通过id读
        /// </summary>
        /// <param name="AssetBundleID"></param>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string AssetBundleID)
        {
            if (TypeDef.cachedAssetBundle.ContainsKey(AssetBundleID)) return TypeDef.cachedAssetBundle[AssetBundleID];
            else
            {
                Log.Error($"FS.L2D. Unable to load assetbundle named {AssetBundleID}");
                return null;
            }
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

        #endregion

        #region 内部加载模型相关

        /// <summary>
        /// 从AB包读取模型，不会做后续处理。如果希望自己完成后续处理就用这个。在使用此方法前需要提前实例化离屏渲染目标。
        /// </summary>
        /// <param name="AB"></param>
        /// <param name="path"></param>
        /// <param name="renderTarget">模型采用离屏渲染规避泰南的UI。需要提前实例化离屏渲染目标，不然看不到模型。渲染目标需要是1920*1080,带有空Image组件的游戏物体。此处留空会默认使用defaultCanvas</param>
        /// <returns></returns>
        public static GameObject LoadModelfromAB(AssetBundle AB, string path, GameObject renderTarget = null)
        {
            if (AB == null)
            {
                Log.Error("[FS.L2D] loading model from empty asset bundle");
                return null;
            }
            GameObject l2dPrefab = AB.LoadAsset<GameObject>(path);
            if (l2dPrefab == null)
            {
                Log.Error($"[FS.L2D] fail to load l2d model named {path}");
                return null;
            }

            if (renderTarget == null) renderTarget = defaultRenderTarget;

            OffscreenRendering offscreenRenderingComp = l2dPrefab.GetComponent<OffscreenRendering>();
            if (offscreenRenderingComp == null)
            {
                Log.Error("[FS.L2D] model lacks off screen rendering component");
                return null;
            }
            offscreenRenderingComp.renderTarget = renderTarget;

            GameObject l2dInstance = GameObject.Instantiate(l2dPrefab);
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
        public static GameObject InstantiateLive2DModel(AssetBundle assetBundle, string ModID, string modelPath, string rigJsonPath = null, GameObject renderTargetName = null, bool eyeFollowMouse = true, GameObject eyeFollowTarget = null)
        {
            //不允许传入空ab包
            if (assetBundle == null)
            {
                Log.Error("[FS.L2D]Error: received null ab");
                return null;
            }

            //GameObject renderTarget = GameObject.Find("L2DRenderTarget");
            if (renderTargetName == null)
            {
                renderTargetName = SetDefaultCanvas(true);
            }

            GameObject l2dIns = LoadModelfromAB(assetBundle, modelPath, renderTargetName);

            //没能成功从ab包加载模型
            if (l2dIns == null)
            {
                Log.Error($"[FS.L2D]failed to load model with path {modelPath}");
                return null;
            }

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

        /// <summary>
        /// 从LiveModelDef，快捷地加载并返回一个实例化模型。
        /// </summary>
        /// <param name="def">l2d的def</param>
        /// <param name="setAsDefault">设置为本框架渲染主菜单l2d和商人界面l2d时默认使用的模型</param>
        /// <param name="modelID">保存在TypeDef中的模型缓存，方便二次调用。传入null就是不想保存。</param>
        /// <returns></returns>
        public static GameObject InstantiateLive2DModel(LiveModelDef def, GameObject renderTarget = null, bool setAsDefault = false, string modelID = null)
        {
            GameObject l2dInstance;
            if (modelID != null && TypeDef.cachedL2DModel.ContainsKey(modelID))
            {
                l2dInstance = TypeDef.cachedL2DModel[modelID];
                if (l2dInstance != null) return l2dInstance;
            }

            AssetBundle ab = FS_Tool.LoadAssetBundle(def.modID, def.assetBundle);
            l2dInstance = FS_Tool.InstantiateLive2DModel(ab, def.modID, def.modelName, rigJsonPath: def.rigJsonPath, renderTargetName: renderTarget, eyeFollowMouse: def.eyeFollowMouse);
            
            if (l2dInstance == null)
            {
                Log.Error($"[FS.L2D] failed to load live2d model named {def.defName}");
                return null;
            }

            if (setAsDefault)
            {
                defaultModelInstance = l2dInstance; 
                l2dDef = def;
            }
            
            if (modelID != null)
            {
                if (TypeDef.cachedL2DModel.ContainsKey(modelID)) TypeDef.cachedL2DModel[modelID] = l2dInstance;
                else TypeDef.cachedL2DModel.Add(modelID, l2dInstance);
            }

            return l2dInstance;
        }

        #endregion

        #region 封装的加载模型方法
        /// <summary>
        /// 启用已经实例化的模型时，并重新指定渲染目标。
        /// 禁用时直接setActive(false)就可以了。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="active"></param>
        /// <param name="renderTarget"></param>
        /// <returns></returns>
        public static GameObject SetModelActive(this GameObject model, GameObject renderTarget = null)
        {
            if (model == null)
            {
                Log.Error($"FS.L2D. invaild model");
                return null;
            }
            if (renderTarget == null)
            {
                renderTarget = SetDefaultCanvas(true);
            }
            model.GetComponent<OffscreenRendering>().renderTarget = renderTarget;
            model.SetActive(true);
            return model;
        }

        public static GameObject DrawModel(int drawAt, LiveModelDef def)
        {
            if (defaultModelInstance == null)
            {
                Log.Error("[FS.L2D] default model is null");
            }
            ModelTransform modelTransform;
            if (def.transform.ContainsKey(drawAt))
            {
                modelTransform = def.transform[drawAt];
            }
            else modelTransform = null;

            defaultModelInstance.SetModelActive();

            return defaultModelInstance;
        }

        public static GameObject ChangeDefaultModel(LiveModelDef def)
        {
            if (defaultModelInstance != null) defaultModelInstance.SetActive(false);
            GameObject obj = InstantiateLive2DModel(def, null, true, def.defName);
            if (obj != null) obj.SetActive(false);
            else Log.Error($"[FS.L2D] failed to change default l2d to {def.defName}");
            return obj;
        }

        //加载/启用禁用 默认Canvas，并返回渲染目标。
        public static GameObject SetDefaultCanvas(bool active = true)
        {
            if (defaultCanvas == null)
            {
                GameObject canvasPrefab = l2dResource.LoadAsset<GameObject>("L2DCanvas");
                defaultCanvas = GameObject.Instantiate(canvasPrefab);
                defaultRenderTarget = defaultCanvas.transform.GetChild(0).gameObject;
            }
            defaultCanvas.SetActive(active);
            return defaultRenderTarget;
        }

        #endregion
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

        #region 主界面/商店 看板相关
        static GameObject StoreFront => TypeDef.cachedStoreFront;
        public static void SetStoreFront(GameObject model, bool destroyModel = true, Vector3? location = null, Vector3? scale = null)
        {
            if (StoreFront != null)
            {
                StoreFront.SetActive(false);
                if (destroyModel)
                {
                    GameObject.Destroy(StoreFront);
                }
            }
            if (location is Vector3 loc) model.transform.position = loc;
            if (scale is Vector3 sca) model.transform.localScale = sca;
            TypeDef.cachedStoreFront = model;
        }

        public static void SetStoreFrontActive(bool active, Vector3? setLocation)
        {
            if (StoreFront != null)
            {
                StoreFront.SetActive(active);
                if (setLocation is Vector3 loc) StoreFront.transform.position = loc;
            }
        }

        #endregion
    }
}
