using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace FS_LivelyRim
{
    public class ModelTransform
    {
        public Vector3? location = null;
        public Vector3? rotation = null;
    }

    public class AssetResourceDef : Def
    {
        public string modID;
        public string assetBundle;
        public string modelName; 
    }

    public class LiveModelDef : AssetResourceDef
    {
        public string rigJsonPath = null;
        public bool eyeFollowMouse = true;
        public Dictionary<int, ModelTransform> transform = new Dictionary<int, ModelTransform>();
    }
}