using AK_DLL.UI;
using LudeonTK;
using System.Collections.Generic;
using Verse;

namespace AK_DLL
{
    public static class MIS_DebugAction
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

        [DebugAction("MIS-AK Actions", "Print Recruited Operators", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void PrintOperators()
        {
            foreach (string id in GC_OperatorDocumentation.opDocArchive.Keys)
            {
                OperatorDocument doc = GC_OperatorDocumentation.opDocArchive[id];

                Log.Message($"[AK] 已招募 {id} - {doc.operatorDef.nickname}, 存活:{!doc.pawn.Dead}");
            }
        }

        [DebugAction("MIS-AK Actions", "Print Operator Voice Defs", false, false, false, false, false, 0, false, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void PrintVoicePacks()
        {
            OperatorDocument doc = Find.Selector.SelectedPawns?.FirstOrDefault(pawn => true).GetDoc();
            if (doc is not { voicePack: { } voicePackDef }) return;

            List<SoundDef> sounds = new()
            {
                voicePackDef.recruitSound,
                voicePackDef.undraftSound,
                voicePackDef.diedSound,
            };
            sounds.AddRange(voicePackDef.selectSounds);
            sounds.AddRange(voicePackDef.draftSounds);
            sounds.AddRange(voicePackDef.abilitySounds);
            foreach (SoundDef sound in sounds)
            {
                Log.Message($"[AK] {doc.pawn.Name} bound sound: {sound.defName}");
            }
        }
    }
}
