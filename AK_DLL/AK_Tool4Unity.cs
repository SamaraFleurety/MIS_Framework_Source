using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using FSUI;

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
    }
}
