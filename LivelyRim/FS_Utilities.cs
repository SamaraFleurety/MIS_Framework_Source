using Live2D.Cubism.FSAddon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static void SetDefaultModelInActive()
        {
            if (defaultModelInstance == null) return;

            Log.Message("inact");
            defaultCanvas.SetActive(false);
            defaultModelInstance.SetActive(false);
        }

        public static GameObject DrawModel(int drawAt, LiveModelDef def, GameObject renderTarget = null)
        {
            if (defaultModelInstance == null)
            {
                ChangeDefaultModel(def);
            }

            //if (defaultModelInstance.activeInHierarchy) return defaultModelInstance;

            defaultModelInstance.SetModelActive(renderTarget);
            //模型的位置是否需要偏移
            if (def.transform.ContainsKey(drawAt))
            {
                ModelTransform modelTransform;
                modelTransform = def.transform[drawAt];
                if (modelTransform != null)
                {
                    if (modelTransform.location is Vector3 loc)
                    {
                        defaultModelInstance.transform.position = loc;
                    }
                    if (modelTransform.rotation is Vector3 rot)
                    {
                        defaultModelInstance.transform.rotation = Quaternion.Euler(rot);
                    }
                }
            }

            return defaultModelInstance;
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
