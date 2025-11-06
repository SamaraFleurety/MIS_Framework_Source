using RuntimeAudioClipLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;

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
            if (!obj) return;
            Transform t = obj.transform;
            ClearAllChild(t);
        }

        public static void ClearAllChild(Transform t)
        {
            if (!t) return;
            for (int i = 0; i < t.childCount; ++i)
            {
                Object.Destroy(t.GetChild(i).gameObject);
            }
        }

        public static TMP_FontAsset GetUGUIFont(FontDef def)
        {
            AssetBundle ab = LoadAssetBundle(def.modID, def.assetBundle);
            return ab.LoadAsset<TMP_FontAsset>(def.modelName);
        }


        //存所有mod的路径 <packageID(原生), 路径>
        public static readonly Dictionary<string, string> modPath = new();

        private static void LoadAllModPath()
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
        private static readonly Dictionary<string, AssetBundle> cachedAssetBundle = new();

        //从随便一个模型/prefab的def，仅读其ab包
        public static AssetBundle LoadAssetBundle(this AssetDef assetDef)
        {
            return LoadAssetBundle(assetDef.modID, assetDef.assetBundle);
        }

        /// <summary>
        /// 从mod根目录/Asset/开始索引。假设需要读根目录/Aseet/ab，那就入参mod的packageID 和 "ab"
        /// </summary>
        /// <param name="modPackageID">写在About.xml里面的PackageID</param>
        /// <param name="assetBundlePath">上述mod根目录/Asset/本参数。也就是说本参数通常是ab包的名字</param>
        /// <param name="assetBundleID">如果path和ab包实际名字不一致或有冲突，需要输入想使用的自定义名字</param>>
        /// <returns></returns>
        public static AssetBundle LoadAssetBundle(string modPackageID, string assetBundlePath, string assetBundleID = null)
        {
            AssetBundle assetBundle;

            assetBundleID ??= assetBundlePath;

            if (cachedAssetBundle.TryGetValue(assetBundleID, out AssetBundle bundle)) return bundle;

            string fullPath = ModIDtoPath(modPackageID, assetBundlePath, "/Asset/");
            Log.Message(fullPath);
            try
            {
                assetBundle = AssetBundle.LoadFromFile(fullPath);
                if (assetBundle)
                {
                    //记录这次加载的ab包
                    cachedAssetBundle.Add(assetBundleID, assetBundle);
                }
                else
                {
                    Log.Error($"[FS.UGUI] Unable to load AssetBundle at {fullPath}");
                    return null;
                }
            }
            catch
            {
                Log.Error($"[FS.UGUI] Unable to load AssetBundle at {fullPath}");
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
            if (cachedAssetBundle.TryGetValue(AssetBundleID, out AssetBundle bundle)) return bundle;
            else
            {
                Log.Error($"[FS.UGUI] Unable to load AssetBundle named {AssetBundleID}");
                return null;
            }
        }

        /// <summary>
        /// 对于一个asset def，获取其model name字段中的物体（通常是prefab）
        /// </summary>
        /// <param name="def"></param>
        /// <returns></returns>
        public static GameObject LoadPrefab(this AssetDef def)
        {
            AssetBundle ab = def.LoadAssetBundle();
            if (ab == null)
            {
                Log.Error($"[AK.Unity]未找到ab包 {def.assetBundle} 于mod {def.modID}");
                return null;
            }
            GameObject prefab = ab.LoadAsset<GameObject>(def.modelName);
            if (prefab == null)
            {
                Log.Error($"[AK.Unity]未成功初始化prefab 于 {def.defName}");
            }
            return prefab;
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
        public static readonly Dictionary<string, Texture2D> dynamicLoadingTextures = new();

        public static Texture2D GetDynamicLoadTexture(string itemFullHardwarePath, bool cacheIntoDictionary = false)
        {
            dynamicLoadingTextures.TryGetValue(itemFullHardwarePath, out Texture2D texture);
            if (texture) return texture;

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
            int index = fullPath.IndexOf(middlePath, StringComparison.Ordinal);
            if (index == -1)
            {
                Log.Error($"[AK] {typeof(T).Name} {fullPath} 无法转变为相对路径");
                return null;
            }
            temp = temp.Substring(index + middlePath.Length);
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
            if (!File.Exists(fileFullPath)) return null;

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
            byte[] data;
            using (FileStream fileSource = new(path, FileMode.Open, FileAccess.Read))
            {
                data = new byte[fileSource.Length];

                int numBytesToRead = (int)fileSource.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int read = fileSource.Read(data, numBytesRead, numBytesToRead);
                    // Break when the end of the file is reached.
                    if (read == 0) break;

                    numBytesRead += read;
                    numBytesToRead -= read;
                }
            }

            Texture2D texture2D = new(2, 2, TextureFormat.Alpha8, mipChain: true);
            texture2D.LoadImage(data);
            if (texture2D.width % 4 != 0 || texture2D.height % 4 != 0)
            {
                if (Prefs.LogVerbose)
                {
                    Debug.LogWarning($"Texture does not support mipmapping, needs to be divisible by 4 ({texture2D.width}x{texture2D.height}) for '{path}'");
                }
                texture2D = new Texture2D(2, 2, TextureFormat.Alpha8, mipChain: false);
                texture2D.LoadImage(data);
            }

            if (Prefs.TextureCompression) texture2D.Compress(highQuality: true);

            texture2D.name = Path.GetFileNameWithoutExtension(path);
            texture2D.filterMode = FilterMode.Trilinear;
            texture2D.anisoLevel = 2;
            texture2D.Apply(updateMipmaps: true, makeNoLongerReadable: true);

            return texture2D;
        }
        #endregion
    }
}
