using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace AK_DLL
{
    public static class AK_Tool
	{
		static bool doneInitialization = false;
		static float lastVoiceTime = 0;
		public static string[] operatorTypeStiring = new string[] { "Caster", "Defender", "Guard", "Vanguard", "Specialist", "Supporter", "Medic", "Sniper" };
		public static string[] romanNumber = new string[] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" , "XI"};
		//如果增加了要自动绑定的服装种类，只需要往这个字符串数组增加。
		public static string[] apparelType = new string[] { "Hat" };
		public static float lastVoiceLength = 0;
		public static SoundDef[] abilitySFX = new SoundDef[4] {DefDatabase<SoundDef>.GetNamed("AK_SFX_Atkboost"), DefDatabase<SoundDef>.GetNamed("AK_SFX_Defboost"), DefDatabase<SoundDef>.GetNamed("AK_SFX_Healboost"), DefDatabase<SoundDef>.GetNamed("AK_SFX_Tactboost") };

		public static Dictionary<int, Dictionary<string, OperatorDef>> operatorDefs = new Dictionary<int, Dictionary<string, OperatorDef>>();
		//public static Dictionary<string, OperatorDef>[] operatorDefs = new Dictionary<string, OperatorDef>[(int)OperatorType.Count];

		public static Dictionary<int, string> operatorClasses = new Dictionary<int, string>();

		public static void PrintfHairColor(this Pawn p)
        {
			Log.Message($"pawnHC: {p.story.HairColor.r}, {p.story.HairColor.g}, {p.story.HairColor.b},{p.story.HairColor.a}");
		}

		public static void PrinftSkinColor(this Pawn p)
        {
			Log.Message($"pawnSC: {p.story.SkinColor.r}, {p.story.SkinColor.g}, {p.story.SkinColor.b},{p.story.SkinColor.a}");
		}

		public static void Initialization()
        {
			if (doneInitialization) return;
			LoadOperatorClasses();
			AutoFillOperators();
			Log.Message("MIS.初始化完成");
			doneInitialization = true;
		}
		public static void AutoFillOperators()
        {
			foreach (int i in operatorClasses.Keys)
            {
				operatorDefs[i] = new Dictionary<string, OperatorDef>();
            }
			foreach (OperatorDef i in DefDatabase<OperatorDef>.AllDefs)
			{
				i.AutoFill();
				try
				{
					operatorDefs[i.operatorType.sortingOrder].Add(i.nickname, i);
				}
				catch (Exception)
				{
					Log.Error("没加起" + i.nickname);
				}
			}
		}
		public static void LoadOperatorClasses()
        {
			foreach(OperatorClassDef i in DefDatabase<OperatorClassDef>.AllDefs)
            {
				int tempOrder = 10000001;
				if (i.sortingOrder >= 10000000)
                {
					Log.Error(i.label + "'s sorting order must lower than 10000000");
                }
				else if (operatorClasses.ContainsKey(i.sortingOrder))
                {
					Log.Error(i.label + "has duplicate loading order with" + operatorClasses[i.sortingOrder]);
					operatorClasses.Add(tempOrder, i.label);
					tempOrder++;
                }
				else
                {
					operatorClasses.Add(i.sortingOrder, i.label);
                }
				if (operatorClasses.Count >= 1)
                {
					Window_Recruit.operatorType = operatorClasses.First().Key;
				}
			}
        }
		public static List<IntVec3> GetSector(OperatorAbilityDef ability,Pawn caster)
        {
			List<IntVec3> result = new List<IntVec3>();
			foreach (IntVec3 intVec3 in GenRadial.RadialCellsAround(caster.Position, ability.sectorRadius, false))
			{
				float mouseAngle = caster.Position.ToIntVec2.ToVector2().AngleTo(Event.current.mousePosition);
				float curAngle = caster.Position.ToVector3().AngleToFlat(intVec3.ToVector3());
				if (intVec3.InBounds(caster.Map) && curAngle > ability.minAngle + mouseAngle && curAngle < ability.maxAngle + mouseAngle)
				{
					result.Add(intVec3);
				}
			}
			return result;
		}
		public static string GetOperatorNameFromDefName(string defName)
        {
			string[] temp = defName.Split('_');
			if (temp.Length <= 1) return null;
			return temp[temp.Length - 1];
        }

		//要求defName格式：前缀_随便啥_人物名；物品格式：前缀_{string thingType}_人物名
		public static string GetThingsDefName(string defName, string thingType)
        {
			string[] temp = defName.Split('_');
			if (temp.Length <= 1) return null;

			return temp[0] + "_" + thingType + "_" + temp[temp.Length - 1];
		}
		public static void PlaySound(this SoundDef sound)
		{
			//Log.Message($"尝试播放{sound.defName}.{Time.realtimeSinceStartup} : {lastVoiceTime}");
			if (Time.realtimeSinceStartup - lastVoiceTime <= AK_ModSettings.voiceIntervalTime / 2.0 || Time.realtimeSinceStartup - lastVoiceTime <= lastVoiceLength || !AK_ModSettings.playOperatorVoice) return;
			lastVoiceLength = sound.Duration.max;
			sound.PlayOneShotOnCamera(null);
			lastVoiceTime = Time.realtimeSinceStartup;
			return;
		}

		//播放技能语音
		public static void PlaySound(this Pawn pawn, SFXType type)
        {
			abilitySFX[(int)type].PlayOneShot(null);
			//Log.Message((pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("AK_Operator")) as Hediff_Operator).document.voicePack.abilitySounds.RandomElement().defName);
			(pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("AK_Operator")) as Hediff_Operator).document.voicePack.abilitySounds.RandomElement().PlaySound();
        }

		public static void LoadedGame()
        {
			lastVoiceTime = 0;
			lastVoiceLength = 0;
        }

		public static OperatorDocument GetDoc(Pawn p)
        {
			if (p.health.hediffSet.GetFirstHediff<Hediff_Operator>() == null) return null;
			return p.health.hediffSet.GetFirstHediff<Hediff_Operator>().document;
        } 

		public static void DrawBottomLeftPortrait()
        {
			if (AK_ModSettings.displayBottomLeftPortrait == false) return;
			if (Find.World == null || Find.CurrentMap == null || Find.Selector == null || Find.Selector.AnyPawnSelected == false || Find.Selector.SelectedPawns.Count == 0) return;
			Pawn p = Find.Selector.SelectedPawns.First();
			if (p == null) return;
			OperatorDocument doc = AK_Tool.GetDoc(p);
			if (doc == null) return;
			Widgets.DrawTextureFitted(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 408, 408), ContentFinder<Texture2D>.Get(AK_Tool.GetDoc(p).operatorDef.stand), (float)AK_ModSettings.ratio * 0.05f);
		}
	}
}
