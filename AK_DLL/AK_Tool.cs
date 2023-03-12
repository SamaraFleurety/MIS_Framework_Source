using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AK_DLL
{
	//这个属性已经保证静态构造在DefDatabase加载后以及交叉引用之后执行了
	[StaticConstructorOnStartup]
    public static class AK_Tool
	{
		public static readonly Texture2D Frame_HeadPortrait = ContentFinder<Texture2D>.Get("UI/Frame/Frame_HeadPortrait");
		public static readonly Texture2D Frame_Skills = ContentFinder<Texture2D>.Get("UI/Frame/Frame_Skills");
		public static readonly Texture2D Frame_Null = ContentFinder<Texture2D>.Get("UI/Frame/Null");
        public static readonly Texture2D Ability_White = ContentFinder<Texture2D>.Get("UI/Abilities/White");

		public static string[] operatorTypeStiring = new string[] { "Caster", "Defender", "Guard", "Vanguard", "Specialist", "Supporter", "Medic", "Sniper" };
		public static string[] romanNumber = new string[] { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" , "XI"};
		//如果增加了要自动绑定的服装种类，只需要往这个字符串数组增加。
		public static string[] apparelType = new string[] { "Apparel", "Hat" };
		/// <summary>
		/// For caching loaded mod name. Used in loading log only at the moment
		/// </summary>
		public static readonly HashSet<string> modNames= new HashSet<string>();

		public static Dictionary<int, Dictionary<string, OperatorDef>> operatorDefs = new Dictionary<int, Dictionary<string, OperatorDef>>();
		//public static Dictionary<string, OperatorDef>[] operatorDefs = new Dictionary<string, OperatorDef>[(int)OperatorType.Count];

		public static Dictionary<int, OperatorClassDef> operatorClasses = new Dictionary<int, OperatorClassDef>();

		/*public static void PrintfHairColor(this Pawn p)
        {
			Log.Message($"pawnHC: {p.story.HairColor.r}, {p.story.HairColor.g}, {p.story.HairColor.b},{p.story.HairColor.a}");
		}

		public static void PrinftSkinColor(this Pawn p)
        {
			Log.Message($"pawnSC: {p.story.SkinColor.r}, {p.story.SkinColor.g}, {p.story.SkinColor.b},{p.story.SkinColor.a}");
		}*/
		/// <summary>
		/// All OperatorClassDef's defName should follow this rule:
		/// ModAbbr_WhatEverYouLike_ActualName
		/// Example:
		/// RA_OperatorClass_SRT, AK_OperatorClass_Caster
		/// And its label should be the same as the last segment of the defName (SRT, Caster)
		/// </summary>
		/// <exception cref="NullReferenceException"></exception>
		static AK_Tool()
        {
            int loadorder = 0;
            try
            {
                foreach (OperatorClassDef def in DefDatabase<OperatorClassDef>.AllDefsListForReading)
                {
                    modNames.Add(def.defName.Split('_').First());
                    operatorClasses[loadorder] = def;
					operatorDefs.Add(loadorder, new Dictionary<string, OperatorDef>((from x in DefDatabase<OperatorDef>.AllDefs
                                                                                     where x.operatorType == def
																					 orderby x.modContentPack.Name
                                                                                     select x).ToDictionary(x => GetOperatorIDFrom(x.defName), x => x)));
                    loadorder++;
                }
                ResolveOperatorClassIcons();
            }
            catch (Exception e)
            {
                Log.Error($"Error During Loading OperatorClassDef: {e}\n Last loading mod name:{modNames.Last()}\n {loadorder}th iteration");
            }
            finally
            {
                if (!operatorDefs.NullOrEmpty())
                {
                    foreach (Dictionary<string,OperatorDef> deflist in operatorDefs.Values)
                    {
                        foreach (OperatorDef def in deflist.Values)
                            def.AutoFill();
                    }

                }
                else
                {
                    throw new NullReferenceException("operatorDefs loaded, but is null or empty");
                }
            }
            Log.Message($"MIS.初始化完成\n Loaded mods:\n{string.Join("\n",modNames.ToArray())}");
			Log.Message($"Loaded {operatorClasses.Count} operatorClasses:\n{string.Join("\n", operatorClasses.Values.Select(x=>x.defName).ToArray())}");
            try
            {
                Window_Recruit.operatorType = operatorClasses.FirstOrDefault().Key;
            }
            catch when (operatorClasses.NullOrEmpty())
            {
                Log.Error($"operatorClasses NullOrEmpty");
            }

			if (!Window_Recruit.ResolveVariables())
				Log.Error("Couldn't cache viewRect's height.");

            static void ResolveOperatorClassIcons()
			{
                foreach (var def in operatorClasses.Values)
				{
					if (def.textureFolder == "" || def.textureFolder == null)
						continue;
					string itemPath = def.textureFolder + "/" + def.defName.Split('_').Last();
					//Log.Message($"Trying to load texture from: {itemPath}");
                    if (def.textureFolder != null)
					{
						def.tex = ContentFinder<Texture2D>.Get(itemPath, false);
					}

					if(def.tex == null)
					{
						Log.Error($"Error when loading {def.defName}'s Icon. Wrong xml format. If no icon is present, do not add node \"textureFolder\"");
					}
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

		public static string GetPrefixFrom(string XMLdefName)
        {
			string[] temp = XMLdefName.Split('_');
			if (temp.Length <= 1) return null;
			return temp[0];
		}
		public static string GetOperatorIDFrom(string XMLdefName)
        {
			string[] temp = XMLdefName.Split('_');
			if (temp.Length <= 1) return null;
			return temp[temp.Length - 1];
        }

		//要求defName格式：前缀_随便啥_人物名；返回物品defName格式：前缀_{string thingType}_人物名
		public static string GetThingdefNameFrom(string XMLdefName, string thingType)
        {
			string[] temp = XMLdefName.Split('_');
			if (temp.Length <= 1) return null;

			return temp[0] + "_" + thingType + "_" + temp[temp.Length - 1];
		}

		public static string GetThingdefNameFrom(string operatorID, string prefix, string thingType)
        {
			return prefix + "_" + thingType + "_" + operatorID;
        }

		public static OperatorDocument GetDoc(this Pawn p)
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
			Widgets.DrawTextureFitted(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 408, 408), AK_Tool.GetDoc(p).operatorDef._stand, (float)AK_ModSettings.ratio * 0.05f);
		}

		public static Hediff_Operator GetArkNightsHeDiffByPawn(Pawn pawn)
		{
			var hediffCandidate = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("AK_Operator"));
			if (hediffCandidate is null)
			{
				return null;
			}
			return hediffCandidate as Hediff_Operator;
		}

		//模式1是精确搜索，2是找第一个更大值，3是找第一个更小值
		public static int quickSearch(int[] arr, int leftPtr, int rightPtr, int target, int mode)
        {
			int middle;
			middle = 0;
			if (rightPtr - leftPtr < 0) return -1;
			if (target > arr[rightPtr])
			{
				if (mode == 3) return rightPtr;
				else return -1;
			}
			else if (target < arr[leftPtr])
			{
				if (mode == 2) return leftPtr;
				else return -1;
			}
			while (rightPtr >= leftPtr)
			{
				middle = (leftPtr + rightPtr) / 2;
				if (arr[middle] == target)
				{
					return middle;
				}
				else if (leftPtr == rightPtr) break;
				else if (arr[middle] < target)
				{
					leftPtr = middle + 1;
				}
				else
				{
					rightPtr = middle;
				}
			}
			if (mode == 2) return rightPtr;
			else if (mode == 3) return rightPtr - 1;
			else return -1;
		}

		public static int weightArrayRand(int[] arr)
        {
			int rd = UnityEngine.Random.Range(1, arr.Last());

			return quickSearch(arr, 0, arr.Length - 1, rd, 2);
        }
	}
}
