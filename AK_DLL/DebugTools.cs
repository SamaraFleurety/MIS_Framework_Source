using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using RimWorld.Planet;
using Verse;
using System.Reflection;
using HarmonyLib;

namespace AK_DLL
{
	public static class FS_DebugAction
	{
		private static Map Map => Find.CurrentMap;
		private static BoolGrid usedCells;
		private static CellRect overRect;

		[DebugAction("Fleurety's Actions", "Make colony (Operators)", false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		public static void MakeColonyWeapon()
		{
			bool godMode = DebugSettings.godMode;
			DebugSettings.godMode = true;
			Thing.allowDestroyNonDestroyable = true;
			if (usedCells == null)
			{
				usedCells = new BoolGrid(Map);
			}
			else
			{
				usedCells.ClearAndResizeTo(Map);
			}
			overRect = new CellRect(Map.Center.x - 50, Map.Center.z - 50, 100, 100);

			GenDebug.ClearArea(overRect, Find.CurrentMap);

			List<OperatorDef> opDefs = new List<OperatorDef>();

			foreach (KeyValuePair<int, Dictionary<string, OperatorDef>> i in RIWindowHandler.operatorDefs)
            {
				opDefs.AddRange(i.Value.Values);
            }

			int num = 0;
			foreach (IntVec3 item in overRect)
			{
				if (item.x % 6 != 0 && item.z % 6 != 0)
				{
					opDefs[num].Recruit(item, Map);
					num++;
					if (num >= opDefs.Count)
					{
						break;
					}
				}
			}

			DebugSettings.godMode = godMode;
			Thing.allowDestroyNonDestroyable = false;
		}
	}
}
