using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FSUI;
using TMPro;
using FS_LivelyRim;
using Verse;

namespace AK_DLL
{
    public static class AK_Tool4Unity
    {
        public static void ClearAllChild(GameObject obj)
        {
            if (obj == null) return;
            Transform t = obj.transform;
            ClearAllChild(t);
        }

        public static void ClearAllChild(Transform t)
        {
            if (t == null) return;
            for (int i = 0; i < t.childCount; ++i)
            {
                GameObject.Destroy(t.GetChild(i).gameObject);
            }
        }

        public static Sprite Image2Spirit(Texture2D image)
        {
            Sprite sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        public static TMP_FontAsset GetUGUIFont()
        {
            if (AK_ModSettings.Font == null)
            {
                Log.Error($"[MIS] missing font. reset to YouYuan");
                AK_ModSettings.Font = AKDefOf.AK_Font_YouYuan;
                AK_ModSettings settings = LoadedModManager.GetMod<AK_Mod>().settings;
                settings.Write();
            }
            if (AK_ModSettings.Font == AKDefOf.AK_Font_YouYuan)
            {
                return AK_Tool.FSAsset.LoadAsset<TMP_FontAsset>(AK_ModSettings.Font.modelName);
            }
            return GetUGUIFont(AK_ModSettings.Font);
        }

        public static TMP_FontAsset GetUGUIFont(FontDef def)
        {
            Debug.Log("get font stdef");
            AssetBundle AB = FS_Tool.LoadAssetBundle(def.modID, def.assetBundle);
            Debug.Log("get font stdefret");
            return AB.LoadAsset<TMP_FontAsset>(def.modelName);
        }
    }
}
