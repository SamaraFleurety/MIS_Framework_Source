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
        public static AssetBundle ricepicotest;

        public static string ModID = "FS.LivelyRim";

        //用来缓存ab包。ab重复读是null
        public static Dictionary<string, AssetBundle> cachedAssetBundle = new Dictionary<string, AssetBundle>();

        public static void Initialize()
        {
            //CubismBuiltinMaterials.AB = AssetBundle.LoadFromFile(FS_Tool.modPath[ModID] + "/Asset/l2dtest");
            CubismBuiltinMaterials.AB = FS_Tool.LoadAssetBundle(ModID, "cubismresources");
            ricepicotest = FS_Tool.LoadAssetBundle(ModID, "ricepro");
        }
    }
}
