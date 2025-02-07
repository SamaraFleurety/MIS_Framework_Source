using Live2D.Cubism.FSAddon;
using System.Reflection;
using UnityEngine;
using Verse;

namespace FS_LivelyRim
{
    public static class FS_Utilities
    {
        #region
        static AssetBundle l2dResource => TypeDef.l2dResource;
        static GameObject defaultCanvas => FS_Tool.defaultCanvas;
        static GameObject defaultModelInstance => FS_Tool.defaultModelInstance;
        #endregion

        public static void VerifyL2DDefname(string nickname, string defname)
        {
            LiveModelDef i = DefDatabase<LiveModelDef>.GetNamed(defname);
            
            if (defname == null)
            {
                Log.Error($"[L2D] 不存在 l2d def {defname}");
            }
            if (ModLister.GetActiveModWithIdentifier(i.modID) == null)
            {
                Log.Error($"FS.L2D. error with {nickname}'s live2d named {i} : missing mod with ID {i.modID}");
                return;
            }
            AssetBundle ab = FS_Tool.LoadAssetBundle(i.modID, i.assetBundle);
            if (ab == null)
            {
                Log.Error($"FS.L2D. error with {nickname}'s live2d named {i} : missing assetbundle named {i.assetBundle}");
                return;
            }
            GameObject modelPrefab = ab.LoadAsset<GameObject>(i.modelName);
            if (modelPrefab == null)
            {
                Log.Error($"FS.L2D. error with {nickname}'s live2d named {i} : missing model named {i.modelName}");
                return;
            }
        }

        public static LiveModelDef Live2DModelDef(string live2dModel)
        {
            return DefDatabase<LiveModelDef>.GetNamed(live2dModel);
        }

        public static GameObject DrawLive2DModel(int drawAt, string l2dDefname, GameObject renderTarget = null)
        {
            return DrawModel(drawAt, Live2DModelDef(l2dDefname), renderTarget);

            /*MethodInfo method = typeof(FS_Utilities).GetMethod("DrawModel", BindingFlags.Public | BindingFlags.Static);
            return (GameObject)method.Invoke(null, new object[] { drawAt, def.Live2DModelDef(l2dDefname), renderTarget });*/
            //return FS_Utilities.DrawModel(drawAt, RIWindowHandler.def.Live2DModelDef(l2dDefname), renderTarget);
        }

        //离屏相机的原输出。
        public static RenderTexture OffScreenCameraRenderTarget => OffscreenRendering.OffCamera.targetTexture;

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
                renderTarget = FS_Tool.SetDefaultCanvas(true);
            }
            model.GetComponent<OffscreenRendering>().renderTarget = renderTarget;
            model.SetActive(true);
            return model;
        }

        public static void SetDefaultModelInactive()
        {
            if (defaultModelInstance == null) return;

            defaultCanvas.SetActive(false);
            defaultModelInstance.SetActive(false);
        }

        /// <summary>
        /// 给定live def和渲染目标，返回实例化的模型以备后处理。
        /// </summary>
        /// <param name="drawAt"></param>
        /// <param name="def"></param>
        /// <param name="renderTarget"></param>
        /// <returns></returns>
        public static GameObject DrawModel(int drawAt, LiveModelDef def, GameObject renderTarget = null)
        {
            if (def == null)
            {
                Log.Error("[FS.L2D] trying to draw a null live2d def");
                return null;
            }
            GameObject model;
            if (drawAt == DisplayModelAt.MainMenu || drawAt == DisplayModelAt.MerchantRight)
            {
                if (defaultModelInstance == null)
                {
                    ChangeDefaultModel(def);
                }
                defaultModelInstance.SetModelActive(renderTarget);
                model = defaultModelInstance;
            }
            else
            {
                model = FS_Tool.InstantiateLive2DModel(def, renderTarget, false, def.defName);
            }
            //模型的位置是否需要偏移
            if (def.transform.ContainsKey(drawAt))
            {
                ModelTransform modelTransform;
                modelTransform = def.transform[drawAt];
                if (modelTransform != null)
                {
                    if (modelTransform.location is Vector3 loc)
                    {
                        model.transform.position = loc;
                    }
                    if (modelTransform.rotation is Vector3 rot)
                    {
                        model.transform.rotation = Quaternion.Euler(rot);
                    }
                }
            }
            else
            {
                model.transform.position = new Vector3(10000, -10, 10000);
                model.transform.rotation = Quaternion.Euler(90, 0, 0);
            }
            return model;
        }

        /// <summary>
        /// 给定一个def，将主菜单/商人等界面的默认模型改成这个。
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public static GameObject ChangeDefaultModel(LiveModelDef def)
        {
            if (defaultModelInstance != null) defaultModelInstance.SetActive(false);
            GameObject obj = FS_Tool.InstantiateLive2DModel(def, null, true, def.defName);
            if (obj != null)
            {
                obj.SetActive(false);
                FS_ModSettings.l2dDef = def;
                FS_ModSettings.l2dDefname = def.defName;
            }
            else Log.Error($"[FS.L2D] failed to change default l2d to {def.defName}");
            return obj;
        }
    }
}
