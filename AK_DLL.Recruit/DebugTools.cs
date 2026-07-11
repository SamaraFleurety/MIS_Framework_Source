using AK_DLL.UI;
using LudeonTK;
using System.Collections.Generic;
using Verse;

namespace AK_DLL.Recruit
{
    public static class DebugAction_Recruit
    {
        private static Map Map => Find.CurrentMap;
        private static BoolGrid _usedCells;
        private static CellRect _overRect;

        [DebugAction("MIS-AK Actions", "Make colony (Operators)", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void MakeColonyWeapon()
        {
            bool godMode = DebugSettings.godMode;
            DebugSettings.godMode = true;
            Thing.allowDestroyNonDestroyable = true;
            if (_usedCells == null)
            {
                _usedCells = new BoolGrid(Map);
            }
            else
            {
                _usedCells.ClearAndResizeTo(Map);
            }
            _overRect = new CellRect(Map.Center.x - 50, Map.Center.z - 50, 100, 100);

            GenDebug.ClearArea(_overRect, Find.CurrentMap);

            List<OperatorDef> opDefs = new();

            foreach (KeyValuePair<int, Dictionary<string, OperatorDef>> i in RIWindowHandler.operatorDefs)
            {
                opDefs.AddRange(i.Value.Values);
            }

            int num = 0;
            foreach (IntVec3 item in _overRect)
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
