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

        public static void Initialize()
        {
            //CubismBuiltinMaterials.AB = AssetBundle.LoadFromFile(FS_Tool.modPath[ModID] + "/Asset/l2dtest");
            CubismBuiltinMaterials.AB = FS_Tool.LoadAssetBundle(ModID, "l2dtest");
            ricepicotest = FS_Tool.LoadAssetBundle(ModID, "ricepro");
        }
    }
}
