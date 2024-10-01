using UnityEngine;
using Verse;
using Verse.Sound;

namespace AK_DLL
{
    public static class VoicePlayer
	{
		public static float lastVoiceLength = 0;
		static float lastVoiceTime = 0; 
		//public static SoundDef[] abilitySFX = new SoundDef[4] { DefDatabase<SoundDef>.GetNamed("AK_SFX_Atkboost"), DefDatabase<SoundDef>.GetNamed("AK_SFX_Defboost"), DefDatabase<SoundDef>.GetNamed("AK_SFX_Healboost"), DefDatabase<SoundDef>.GetNamed("AK_SFX_Tactboost") };
		public static void LoadedGame()
		{
			lastVoiceTime = 0;
			lastVoiceLength = 0;
		}

		public static bool CanPlayNow()
        {
			if (Time.realtimeSinceStartup - lastVoiceTime <= AK_ModSettings.voiceIntervalTime / 2.0 || Time.realtimeSinceStartup - lastVoiceTime <= lastVoiceLength || !AK_ModSettings.playOperatorVoice) return false;
			return true;
		}

		public static void PlaySound(this SoundDef sound)
		{
			//Log.Message($"尝试播放{sound.defName}.{Time.realtimeSinceStartup} : {lastVoiceTime}");
			if (!CanPlayNow()) return;
			lastVoiceLength = sound.Duration.max;
			sound.PlayOneShotOnCamera(null);
			lastVoiceTime = Time.realtimeSinceStartup;
			return;
		}

		//随机播放技能语音
		public static void PlaySound(this Pawn pawn)
		{
			OperatorDocument doc = pawn.GetDoc();
			if (doc != null) doc.voicePack.abilitySounds.RandomElement().PlaySound();
		}
	}
}
