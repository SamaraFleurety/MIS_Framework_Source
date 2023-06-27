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

namespace FS_LivelyRim
{
    [StaticConstructorOnStartup]
    public static class FS_Tool
    {
        //存所有mod的路径 <packageID, 路径>
        public static Dictionary<string, string> modPath = new Dictionary<string, string>();

        static string ModID => TypeDef.ModID;

#region 离屏渲染相机
        private static Camera camera = null;
        public const int CanvasHeight = 4096;
        internal static RenderTexture empty = new RenderTexture(1, 1, 0, RenderTextureFormat.ARGB32);
        public static CubismModel cubismModel = null;
        public static CubismPhysicsController cubismPhysics = null;
        internal static Camera Camera
        {
            get
            {
                if (camera == null)
                {
                    GameObject gameObject = new GameObject("Off_Screen_Rendering_Camera");
                    camera = gameObject.AddComponent<Camera>();
                    camera.orthographic = true;
                    camera.orthographicSize = 10;
                    camera.transparencySortMode = TransparencySortMode.Orthographic;
                    camera.nearClipPlane = 0;
                    camera.farClipPlane = 71.5f;
                    camera.transform.position = new Vector3(0, CanvasHeight + 65, 0);
                    camera.transform.rotation = Quaternion.Euler(0, 0, 0);
                    camera.backgroundColor = Color.clear;
                    camera.clearFlags = CameraClearFlags.SolidColor;
                    camera.targetTexture = empty;
                    camera.enabled = true;
                }
                return camera;
            }
        }
#endregion

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
        //实际路径为[MOD根目录]\[subfolder]\path
        private static string ModIDtoPath(string modPackageID, string path, string subfolder = "")
        {
            return modPath[modPackageID.ToLower()] + subfolder + path;
        }

        //从mod根目录/Asset/开始索引。假设需要读根目录/Aseet/ab，那就入参mod的packageID 和 "Aseet/ab"
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

        //从Json手动加载物理rig到模型上。我也不知道为什么原版读ab包时无法处理有嵌套的
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

        public static GameObject LoadModelfromAB(AssetBundle AB, string path)
        {
            GameObject l2dPrefab = AB.LoadAsset<GameObject>(path);
            return GameObject.Instantiate(l2dPrefab);
        }

        public static void DrawLive()
        {

            if (true)
            {
                //GameObject l2dPrefab = TypeDef.ricepicotest.LoadAsset<GameObject>("rice_pro_t03Motion");
                //GameObject l2dins = GameObject.Instantiate(l2dPrefab);

                /*cubismPhysics = l2dins.GetComponent<CubismPhysicsController>();
                //l2dins.transform.position = new Vector3(10000, 10000, 0);
                string physics3JsonAsString = File.ReadAllText(@"S:\rice_pro_zh\runtime\rice_pro_t03.physics3.json");
                CubismPhysics3Json physics3Json = LitJson.JsonMapper.ToObject<CubismPhysics3Json>(physics3JsonAsString);
                
                l2dins.GetComponent<CubismPhysicsController>().Initialize(physics3Json.ToRig());
                cubismPhysics.HasUpdateController = true;*/

                GameObject l2dins = LoadModelfromAB(TypeDef.ricepicotest, "rice_pro_t03Motion");
                l2dins.GetComponent<OffScreenCameraRendering>().renderTarget = GameObject.Find("AG");

                LoadRigtoModel(l2dins, ModID, "rice_pro_t03.physics3.json");


                GameObject eyeTargetPrefab = TypeDef.ricepicotest.LoadAsset<GameObject>("EyeTarget");
                GameObject eyeTargetIns = GameObject.Instantiate(eyeTargetPrefab);
                l2dins.GetComponent<CubismLookController>().Target = eyeTargetIns;
            }
            //l2dins.SetActive(true);
            //Log.Message($"{l2dins.GetComponentInChildren<CubismPart>() == null}");

            if (false)
            {

                RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
                Camera.targetTexture = renderTexture;
                Camera.transform.position = new Vector3(10000, 10000, -10);
                Camera.transparencySortAxis = new Vector3(0, 0, 1);


                GameObject.Find("AG").GetComponent<Image>().material = TypeDef.l2dResource.LoadAsset<Material>("OffScreenCameraMaterial");
                GameObject.Find("AG").GetComponent<Image>().material.mainTexture = renderTexture;
            }
            //GameObject.Find("AG").GetComponent<Image>().material.mainTexture = ContentFinder<Texture2D>.Get(RIWindowHandler.operatorDefs[1].Values.First().stand);
        }
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
