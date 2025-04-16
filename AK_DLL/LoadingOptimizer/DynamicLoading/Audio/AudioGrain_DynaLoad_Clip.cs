using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AK_DLL.DynaLoad
{
    public class AudioGrain_DynaLoad_Clip : AudioGrain_Clip
    {
        //挺蠢的，拿不到父类
        public static string ModID => SubSoundDef_DynaLoading.loadingFromMod;
        public override IEnumerable<ResolvedGrain> GetResolvedGrains()
        {
            if (!SubSoundDef_DynaLoading.shouldResolve) yield break;
            //Log.Message($"resolving {clipPath}");
            AudioClip audioClip = Utilities_Unity.LoadResourceIO<AudioClip>(Utilities_Unity.ModIDtoPath_DynaLoading<AudioClip>(clipPath, ModID));
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
    }
}
