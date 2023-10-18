using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Live2D.Cubism.Rendering;

namespace FS_LivelyRim
{
    public static class TypeDef
    {
        public static AssetBundle l2dResource => CubismBuiltinMaterials.AB;

        //fixme: delete when done
        //public static AssetBundle ricepicotest;
        //public static AssetBundle janustest;

        public static string ModID = "FS.LivelyRim";

        //用来缓存ab包。ab包重复从路径读是null
        public static Dictionary<string, AssetBundle> cachedAssetBundle = new Dictionary<string, AssetBundle>();
        //缓存已经加载的l2d的GameObject(模型)
        public static Dictionary<string, GameObject> cachedL2DModel = new Dictionary<string, GameObject>();

        //商店和主界面看板
        public static GameObject cachedStoreFront = null;
        //l2d分别放在商店和主界面时 的默认位置。注意需要考虑离屏相机的偏移。
        //public static Vector3 defaultStoreFrontLocAtMenu = new Vector3(0, 0, 0);
        //public static Vector3 defaultStoreFrontLocAtMerchant = new Vector3(0, 0, 0);

        //public static RenderTexture tempRT;

        public static void Initialize()
        {
            //CubismBuiltinMaterials.AB = AssetBundle.LoadFromFile(FS_Tool.modPath[ModID] + "/Asset/l2dtest");
            CubismBuiltinMaterials.AB = FS_Tool.LoadAssetBundle(ModID, "cubismresources");
            //ricepicotest = FS_Tool.LoadAssetBundle(ModID, "ricepro");
            //janustest = FS_Tool.LoadAssetBundle(ModID, "janustest");
            //Log.Message($"{janustest == null}");
        }
    }

    public static class DisplayModelAt
    {
        public static int MainMenu = 1;
        public static int MerchantRight = 2;
        public static int RIWMain = 3;
        public static int RIWDetail = 4;
    }

}
