using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AK_DLL.DynaLoad
{
    //已废弃
    /*public class AudioGrain_DynaLoad_Clip : AudioGrain_Clip
    {
        //有哪些语音包 硬编译的，以后非要动态加就写个config def啥的
        List<string> soundsModPackageID = new() { "MIS.SoundCN", "MIS.SoundEN" };
        bool? hasAnySoundPack = null;
        //挺蠢的，拿不到父类
        public static string ModID => SubSoundDef_DynaLoading.loadingFromMod;

        //从非Sounds文件夹动态读取，而非开游戏时加载所有语音
        //懒得做多语音包兼容，所以语音包是老办法
        public override IEnumerable<ResolvedGrain> GetResolvedGrains()
        {
            if (!SubSoundDef_DynaLoading.shouldResolve) yield break;

            if (hasAnySoundPack == null)
            {
                hasAnySoundPack = false;
                foreach (string id in soundsModPackageID)
                {
                    if (Utilities_Unity.modPath.ContainsKey(id.ToLower()))
                    {
                        hasAnySoundPack = true;
                        break;
                    }
                }
            }
            if (hasAnySoundPack is true)
            {
                AudioClip alterSound = ContentFinder<AudioClip>.Get(clipPath, false);
                if (alterSound != null)
                {
                    yield return new ResolvedGrain_Clip(alterSound); ;
                    yield break;
                }
            }

            AudioClip audioClip = Utilities_Unity.LoadResourceIO<AudioClip>(Utilities_Unity.DynaLoad_PathRelativeToFull<AudioClip>(clipPath, ModID));
            //AudioClip audioClip = ContentFinder<AudioClip>.Get(clipPath);
            if (audioClip != null)
            {
                yield return new ResolvedGrain_Clip(audioClip);
            }
            else
            {
                Log.Error("[AK]Grain couldn't resolve: Clip not found at " + clipPath);
            }
        }
    }*/
}
