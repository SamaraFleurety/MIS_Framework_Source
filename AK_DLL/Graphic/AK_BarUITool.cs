using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public static class AK_BarUITool
    {
        public static Material HP_Icon;
        public static Material DEF_Icon;
        public static Material Timer_Icon;
        public static Material BurstIcon;
        public static Material RotateRingIcon;

        private static string HP_IconTexPath = "UI/Abilities/icon_sort_hp";
        private static string DEF_IconTexPath = "UI/Abilities/icon_sort_def";
        private static string Timer_IconTexPath = "UI/Abilities/icon_sort_respawn";
        private static string BurstIconTexPath = "UI/Abilities/Burst";
        private static string RotateRingIconTexPath = "UI/Abilities/RotateRing";
        static AK_BarUITool()
        {
            try
            {
                InitializeIcons();
            }
            catch
            {
                Log.Error("MIS. Critical Error: Initialization fail");
            }
        }
        private static void InitializeIcons()
        {
            HP_Icon = MaterialPool.MatFrom(HP_IconTexPath, ShaderDatabase.Transparent);
            DEF_Icon = MaterialPool.MatFrom(DEF_IconTexPath, ShaderDatabase.Transparent);
            Timer_Icon = MaterialPool.MatFrom(Timer_IconTexPath, ShaderDatabase.Transparent);
            BurstIcon = MaterialPool.MatFrom(BurstIconTexPath, ShaderDatabase.Transparent);
            RotateRingIcon = MaterialPool.MatFrom(RotateRingIconTexPath, ShaderDatabase.Transparent);
            Material[] checkList = { HP_Icon, DEF_Icon, Timer_Icon, BurstIcon, RotateRingIcon };
            foreach (Material icon in checkList)
            {
                if (icon == null)
                {
                    Log.Error("MIS. Critical Error: Missing UI Texture in file path: UI/Abilities/" + icon.name);
                }
            }

        }
    }
}
