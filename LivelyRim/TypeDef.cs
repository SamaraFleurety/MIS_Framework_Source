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

        public static void Initialize()
        {
            List<ModContentPack> Mods = LoadedModManager.RunningMods.ToList();
            for (int i = 0; i < Mods.Count; ++i)
            {
                if (Mods[i].PackageId == "FS.LivelyRim")
                {
                    CubismBuiltinMaterials.AB = AssetBundle.LoadFromFile(Mods[i].RootDir + "/Asset/l2dtest");
                    break;
                }
            }
        }
    }
}
