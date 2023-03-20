using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public static class AK_Tool
    {

        /*public static void PrintfHairColor(this Pawn p)
        {
			Log.Message($"pawnHC: {p.story.HairColor.r}, {p.story.HairColor.g}, {p.story.HairColor.b},{p.story.HairColor.a}");
		}

		public static void PrinftSkinColor(this Pawn p)
        {
			Log.Message($"pawnSC: {p.story.SkinColor.r}, {p.story.SkinColor.g}, {p.story.SkinColor.b},{p.story.SkinColor.a}");
		}*/

        static AK_Tool()
        {
            RIWindowHandler.LoadOperatorClasses();
            RIWindowHandler.AutoFillOperators();
            Log.Message($"MIS.初始化完成");
        }
        //技能残留 早晚给删咯
        public static List<IntVec3> GetSector(OperatorAbilityDef ability, Pawn caster)
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

        public static OperatorDef GetDef(OperatorClassDef classDef, string operatorID)
        {
            return RIWindowHandler.operatorDefs[classDef.sortingOrder][operatorID];
        }
        public static OperatorDef GetDef(int operatorClass, string operatorID)
        {
            return RIWindowHandler.operatorDefs[operatorClass][operatorID];
        }
        public static OperatorDef GetDef(string operatorID)
        {
            foreach (Dictionary<string, OperatorDef> i in RIWindowHandler.operatorDefs.Values)
            {
                return i[operatorID];
            }
            return null;
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
            Widgets.DrawTextureFitted(new Rect(AK_ModSettings.xOffset * 5, AK_ModSettings.yOffset * 5, 408, 408), ContentFinder<Texture2D>.Get(AK_Tool.GetDoc(p).operatorDef.stand), (float)AK_ModSettings.ratio * 0.05f);
        }
        public static void DrawHighlightMouseOver(this Rect rect, Texture2D highlight)
        {
            if (Mouse.IsOver(rect))
            {
                GUI.DrawTexture(rect, highlight);
            }
        }

        /*public static Hediff_Operator GetArkNightsHeDiffByPawn(Pawn pawn)
		{
			var hediffCandidate = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("AK_Operator"));
			if (hediffCandidate is null)
			{
				return null;
			}
			return hediffCandidate as Hediff_Operator;
		}*/

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
