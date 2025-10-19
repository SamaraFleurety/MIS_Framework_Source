using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

namespace AK_DLL.DynamicLoading
{
    public class GraphicData_DynamicLoading : GraphicData
    {
        private readonly FieldInfo fieldCachedGraphic = typeof(GraphicData).GetField("cachedGraphic", BindingFlags.Instance | BindingFlags.NonPublic);

        public bool initiated;

        public Graphic CachedGraphic
        {
            get => (Graphic)fieldCachedGraphic.GetValue(this);
            set => fieldCachedGraphic.SetValue(this, value);
        }
        public GraphicData_DynamicLoading()
        {
            CachedGraphic = BaseContent.BadGraphic;
        }

        public void ForceLoad(string modID)
        {
            if (initiated) return;

            ShaderTypeDef cutout = shaderType;
            cutout ??= ShaderTypeDefOf.Cutout;
            Shader shader = cutout.Shader;
            Texture2D textureDynamicLoading = Utilities_Unity.LoadResourceIO<Texture2D>(Utilities_Unity.DynaLoad_PathRelativeToFull<Texture2D>(texPath, modID));
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
