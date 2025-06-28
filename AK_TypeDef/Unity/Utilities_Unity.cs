using RimWorld.IO;
using RuntimeAudioClipLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public static class Utilities_Unity
    {
        public static List<ModContentPack> AllMods => LoadedModManager.RunningMods.ToList();

        public static void Init()
        {
            LoadAllModPath();
        }
        /*public static TMP_FontAsset GetUGUIFont()
        {
            if (FS_ModSettings.Font == null)
            {
                Log.Error($"[AK.UGUI] missing font. reset to default");
                FS_ModSettings.Font = ConfigDef.Config.defaultFont;
                FS_ModSettings settings = LoadedModManager.GetMod<FS_Mod>().settings;
                settings.Write();
            }
            return GetUGUIFont(FS_ModSettings.Font);
        }*/

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

        public static TMP_FontAsset GetUGUIFont(FontDef def)
        {
            AssetBundle AB = LoadAssetBundle(def.modID, def.assetBundle);
            return AB.LoadAsset<TMP_FontAsset>(def.modelName);
        }


        //存所有mod的路径 <packageID(原生), 路径>
        public static Dictionary<string, string> modPath = new Dictionary<string, string>();
        static void LoadAllModPath()
        {
            List<ModContentPack> Mods = LoadedModManager.RunningMods.ToList();
            for (int i = 0; i < Mods.Count; ++i)
            {
                modPath.Add(Mods[i].PackageId, Mods[i].RootDir);
            }
        }

        //从mod的ID读获取要的文件的路径
        //实际路径为[MOD根目录][subfolder]path
        //subfolder前后应该包括'/'
        private static string ModIDtoPath(string modPackageID, string path, string subfolder = "/")
        {
            if (!modPath.ContainsKey(modPackageID.ToLower()))
            {
                Log.Error($"[FS.UGUI] Error loading mod with ID {modPackageID}");
                return null;
            }
            return modPath[modPackageID.ToLower()] + subfolder + path;
        }

        #region AB包
        //用来缓存ab包。ab包重复从路径读是null。外部不应该直接读这个
        private static Dictionary<string, AssetBundle> cachedAssetBundle = new Dictionary<string, AssetBundle>();

        //从随便一个模型/prefab的def，仅读其ab包
        public static AssetBundle LoadAssetBundle(AssetDef assetDef)
        {
            return LoadAssetBundle(assetDef.modID, assetDef.assetBundle);
        }

        /// <summary>
        /// 从mod根目录/Asset/开始索引。假设需要读根目录/Aseet/ab，那就入参mod的packageID 和 "ab"
        /// </summary>
        /// <param name="modPackageID">写在About.xml里面的PackageID</param>
        /// <param name="AssetBundlePath">上述mod根目录/Asset/本参数。也就是说本参数通常是ab包的名字</param>
        /// <param name="AssetBundleID">如果path和ab包实际名字不一致或有冲突，需要输入想使用的自定义名字</param>>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string modPackageID, string AssetBundlePath, string AssetBundleID = null)
        {
            AssetBundle assetBundle;

            if (AssetBundleID == null) AssetBundleID = AssetBundlePath;

            if (cachedAssetBundle.ContainsKey(AssetBundleID)) return cachedAssetBundle[AssetBundleID];

            string fullPath = ModIDtoPath(modPackageID, AssetBundlePath, "/Asset/");
            Log.Message(fullPath);
            try
            {
                assetBundle = AssetBundle.LoadFromFile(fullPath);
                if (assetBundle != null)
                {
                    //记录这次加载的ab包
                    cachedAssetBundle.Add(AssetBundleID, assetBundle);
                }
                else
                {
                    Log.Error($"[FS.UGUI] Unable to load assetbundle at {fullPath}");
                    return null;
                }
            }
            catch
            {
                Log.Error($"[FS.UGUI] Unable to load assetbundle at {fullPath}");
                return null;
            }
            return assetBundle;
        }

        /// <summary>
        /// 对于确定已经加载的ab包，直接通过id读
        /// </summary>
        /// <param name="AssetBundleID"></param>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string AssetBundleID)
        {
            if (cachedAssetBundle.ContainsKey(AssetBundleID)) return cachedAssetBundle[AssetBundleID];
            else
            {
                Log.Error($"[FS.UGUI] Unable to load assetbundle named {AssetBundleID}");
                return null;
            }
        }
        #endregion

        public static Sprite PathToSprite(string texturePath)
        {
            Texture2D icon = ContentFinder<Texture2D>.Get(texturePath);
            Sprite iconSprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.zero);
            return iconSprite;
        }

        public static Sprite Image2Spirit(this Texture2D image)
        {
            Sprite sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        #region 运行时加载媒体资源
        //将路径标准化 -- 统一使用'/'来区别文件夹
        public static string StandardizePath(this string path)
        {
            return path.Replace("\\", "/");
        }

        //直接从硬盘上的，环世界开游戏时不会读取的文件夹里面读取贴图/语音。

        //需要重复调用的贴图会放在这里(有的在别的地方自带缓存)
        public static Dictionary<string, Texture2D> dynamicLoadingTextures = new();
        public static Texture2D GetDynamicLoadTexture(string itemFullHardwarePath, bool cacheIntoDictionary = false)
        {
            Texture2D texture;
            dynamicLoadingTextures.TryGetValue(itemFullHardwarePath, out texture);
            if (texture != null) return texture;

            texture = LoadResourceIO<Texture2D>(itemFullHardwarePath);

            if (cacheIntoDictionary)
            {
                dynamicLoadingTextures.Add(itemFullHardwarePath, texture);
            }
            return texture;

        }
        //输入相对路径，获得texture。相对路径开头不带'/'，末尾不带后缀，和原版相同。例：UI/path1/bg(
        public static Texture2D GetDynamicLoadTexture(string itemPath, string modID, bool cacheIntoDictionary = false)
        {
            /*Texture2D texture;
            string path = ModIDtoPath_DynaLoading<Texture2D>(itemPath, modID);
            dynamicLoadingTextures.TryGetValue(path, out texture);
            if (texture != null) return texture;

            texture = LoadResourceIO<Texture2D>(path);

            if (cacheIntoDictionary)
            {
                dynamicLoadingTextures.Add(path, texture);
            }
            return texture;*/
            string path = DynaLoad_PathRelativeToFull<Texture2D>(itemPath, modID);
            return GetDynamicLoadTexture(path, cacheIntoDictionary);
        }
        //根据参数，获得一个文件在硬盘上面的路径
        public static string DynaLoad_PathRelativeToFull<T>(string itemPath, string modPackageID, string fileExtension = null) where T : class
        {
            string path;
            if (typeof(T) == typeof(Texture2D))
            {
                fileExtension ??= ".png";
                path = ModIDtoPath(modPackageID, itemPath + fileExtension, DynContentPath<Texture2D>());
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                fileExtension ??= ".wav";
                path = ModIDtoPath(modPackageID, itemPath + fileExtension, DynContentPath<AudioClip>());
            }
            else
            {
                throw new ArgumentException();
            }
            return path;
        }

        //完整硬盘路径转相对路径(变成xml里面填的那种)
        public static string DynaLoad_PathFullToRelative<T>(string fullPath) where T : class
        {
            string temp = StandardizePath(fullPath);
            string middlePath = DynContentPath<T>();
            int index = fullPath.IndexOf(middlePath);
            if (index == -1)
            {
                Log.Error($"[AK] {typeof(T).Name} {fullPath} 无法转变为相对路径");
                return null;
            }
            temp = fullPath.Substring(index + middlePath.Length);
            return Path.GetFileNameWithoutExtension(temp);
        }
        public static bool VerifyFileExistIO(string fileFullPath)
        {
            if (!File.Exists(@fileFullPath))
            {
                Log.Error($"[AK] 尝试获取不存在的文件于{@fileFullPath}");
                return false;
            }
            return true;
        }

        //自动分类和加载路径中的文件。完整路径需要用上面的函数获得
        public static T LoadResourceIO<T>(string fileFullPath) where T : class
        {
            if (!File.Exists(@fileFullPath)) return default(T);
            T val = null;
            if (typeof(T) == typeof(Texture2D))
            {
                Texture2D texture = LoadTexture(fileFullPath);
                val = (T)(object)texture;
            }
            if (typeof(T) == typeof(AudioClip))
            {
                AudioClip audio = Manager.Load(fileFullPath);
                return (T)(object)audio;
                //val = (T)(object)Resources.Load<AudioClip>(ModIDtoPath(modPackageID, itemPath, DynContentPath<AudioClip>()));
            }
            if (val == null)
            {
                Log.Error($"[AK] 无法动态加载{typeof(T)}资源: {fileFullPath}");
            }
            return val;
        }

        public static string DynContentPath<T>() where T : class
        {
            if (typeof(T) == typeof(AudioClip))
            {
                return "/DynaLoad/Sounds/";
            }
            if (typeof(T) == typeof(Texture2D))
            {
                return "/DynaLoad/Textures/";
            }
            throw new ArgumentException();
        }

        //从路径到贴图实例，IO在此完成。
        private static Texture2D LoadTexture(string path)
        {
            FileStream file = new(@path, FileMode.Open, FileAccess.Read);
            Texture2D texture2D;
            byte[] data = new byte[file.Length];
            file.Read(data, 0, data.Length);
            texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, mipChain: true);
            texture2D.LoadImage(data);
            if (texture2D.width % 4 != 0 || texture2D.height % 4 != 0)
            {
                if (Prefs.LogVerbose)
                {
                    Debug.LogWarning($"Texture does not support mipmapping, needs to be divisible by 4 ({texture2D.width}x{texture2D.height}) for '{file.Name}'");
                }
                texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, mipChain: false);
                texture2D.LoadImage(data);
            }
            if (Prefs.TextureCompression)
            {
                texture2D.Compress(highQuality: true);
            }
            texture2D.name = Path.GetFileNameWithoutExtension(file.Name);
            texture2D.filterMode = FilterMode.Trilinear;
            texture2D.anisoLevel = 2;
            texture2D.Apply(updateMipmaps: true, makeNoLongerReadable: true);

            file.Close();
            return texture2D;
        }

        #endregion
    }
}
