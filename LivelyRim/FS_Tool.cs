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

namespace FS_LivelyRim
{
    [StaticConstructorOnStartup]
    public static class FS_Tool
    {
        //执行所有初始化 FIXME：记得写个IO装载原生库
        static FS_Tool ()
        {
            TypeDef.Initialize();

            InitializeCubismDll();
        }

        static void InitializeCubismDll()
        {
            //因为cubism dll是在游戏中途加载，所以有些仅游戏开始时执行一次的初始化方法无法被执行
            MethodInfo method = typeof(CubismModel).GetMethod("RegisterCallbackFunction", BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, new object[0]);

            method = typeof(CubismLogging).GetMethod("Initialize", BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, new object[0]);
        }

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
        private static void DrawLive()
        {

            if (true)
            {
                GameObject l2dPrefab = TypeDef.l2dResource.LoadAsset<GameObject>("rice_pro_t03");
                GameObject l2dins = GameObject.Instantiate(l2dPrefab);
                cubismPhysics = l2dins.GetComponent<CubismPhysicsController>();
                l2dins.transform.position = new Vector3(10000, 10000, 0);
                string physics3JsonAsString = File.ReadAllText(@"S:\rice_pro_zh\runtime\rice_pro_t03.physics3.json");
                Debug.Log(physics3JsonAsString);
                cubismPhysics.HasUpdateController = true;

                //CubismPhysics3Json physics3Json = CubismPhysics3Json.LoadFrom(physics3JsonAsString);
                //CubismPhysics3Json physics3Json = JsonConvert.DeserializeObject<CubismPhysics3Json>(physics3JsonAsString);
                CubismPhysics3Json physics3Json = LitJson.JsonMapper.ToObject<CubismPhysics3Json>(physics3JsonAsString);

                //Log.Message($"json {physics3Json == null};; {physics3Json.Version};; {physics3Json.Meta.PhysicsSettingCount}");

                l2dins.GetComponent<CubismPhysicsController>().Initialize(physics3Json.ToRig());
            }
            //l2dins.SetActive(true);
            //Log.Message($"{l2dins.GetComponentInChildren<CubismPart>() == null}");
            RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
            Camera.targetTexture = renderTexture;
            Camera.transform.position = new Vector3(10000, 10000, -10);
            Camera.transparencySortAxis = new Vector3(0, 0, 1);

            if (false)
            {
                //string path = @"C:\Users\Fleurety\Downloads\Compressed\rice_pro_zh\runtime\rice_pro_t03.model3.json";
                string path = "S:/rice_pro_zh/runtime/rice_pro_t03.model3.json";
                Debug.Log(File.ReadAllText(path));
                CubismModel3Json cubismModel3Json = CubismModel3Json.LoadAtPath(path, BuiltinLoadAssetAtPath);
                Debug.Log($"json: {cubismModel3Json == null}");
                Debug.Log($" {cubismModel3Json.AssetPath}");
                Debug.Log($" {BuiltinLoadAssetAtPath(typeof(string), cubismModel3Json.AssetPath) as string}");
                Debug.Log($"{cubismModel3Json.FileReferences.Moc}");
                Debug.Log($" {cubismModel3Json.FileReferences.Physics} ;; {cubismModel3Json.FileReferences.Pose}");
                var model = cubismModel3Json.ToModel();
                model.transform.position = new Vector3(10000, 10000, 0);
                model.transform.localScale = new Vector3(7, 7, 7);
                model.GetComponent<Animator>().runtimeAnimatorController = TypeDef.l2dResource.LoadAsset<RuntimeAnimatorController>("rice_pro_t03");
            }




            GameObject.Find("AG").GetComponent<Image>().material = TypeDef.l2dResource.LoadAsset<Material>("newMat");
            GameObject.Find("AG").GetComponent<Image>().material.mainTexture = renderTexture;
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
