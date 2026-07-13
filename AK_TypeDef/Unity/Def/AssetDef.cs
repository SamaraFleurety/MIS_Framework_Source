using System;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class AssetDef : Def
    {
        public string modID;
        //public string path;
        public string assetBundle;
        public string modelName;

        private string BundleSuffixForCurrentOs
        {
            get
            {
                return Application.platform switch
                {
                    RuntimePlatform.LinuxPlayer or RuntimePlatform.LinuxEditor => "_linux",
                    RuntimePlatform.OSXEditor or RuntimePlatform.OSXPlayer => "_mac",
                    RuntimePlatform.WindowsPlayer or RuntimePlatform.WindowsEditor => "_win",
                    _ => throw new NotSupportedException($"Unsupported platform for asset bundle loading: {Application.platform}"),
                };
            }
        }

        public string AssetBundleForCurrentOs
        {
            get
            {
                return assetBundle + BundleSuffixForCurrentOs;
            }
        }
    }
}
