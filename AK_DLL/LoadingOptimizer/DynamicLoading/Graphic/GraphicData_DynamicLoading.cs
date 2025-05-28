using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using AK_TypeDef;

namespace AK_DLL.DynamicLoading
{
    public class GraphicData_DynamicLoading : GraphicData
    {
        FieldInfo field_cachedGraphic = typeof(GraphicData).GetField("cachedGraphic", BindingFlags.Instance | BindingFlags.NonPublic);

        public bool initiated = false;

        public Graphic CachedGraphic
        {
            get
            {
                return (Graphic)field_cachedGraphic.GetValue(this);
            }
            set
            {
                field_cachedGraphic.SetValue(this, value);
            }
        }
        public GraphicData_DynamicLoading()
        {
            CachedGraphic = BaseContent.BadGraphic;
        }

        public void ForceLoad(string modID)
        {
            if (initiated) return;
            ShaderTypeDef cutout = shaderType;
            if (cutout == null)
            {
                cutout = ShaderTypeDefOf.Cutout;
            }
            Shader shader = cutout.Shader;
            Texture2D textureDynamicLoading = Utilities_Unity.LoadResourceIO<Texture2D>(Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(texPath,modID));
            CachedGraphic = GraphicDatabase.Get(graphicClass, textureDynamicLoading, shader, drawSize, color, colorTwo, this, shaderParameters, maskPath);
            if (onGroundRandomRotateAngle > 0.01f)
            {
                CachedGraphic = new Graphic_RandomRotated(CachedGraphic, onGroundRandomRotateAngle);
            }
            if (Linked)
            {
                CachedGraphic = GraphicUtility.WrapLinked(CachedGraphic, linkType);
            }

            initiated = true;
        }
    }
}
