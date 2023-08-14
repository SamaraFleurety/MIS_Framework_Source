using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace FS_LivelyRim
{
    public class ModelTransform
    {
        public Vector3? location = null;
        public Vector3? rotation = null;
    }

    public class LiveModelDef : Def
    {
        public string modID;
        public string assetBundle;
        public string modelName;
        public string rigJsonPath = null;
        public bool eyeFollowMouse = true;
        public Dictionary<int, ModelTransform> transform = new Dictionary<int, ModelTransform>();
    }
}