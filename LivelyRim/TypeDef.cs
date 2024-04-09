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

        public static void Initialize()
        {
            CubismBuiltinMaterials.AB = FS_Tool.LoadAssetBundle(ModID, "cubismresources");
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
