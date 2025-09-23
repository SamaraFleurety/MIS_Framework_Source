using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public static class AK_BarUITool
    {
        public static bool CameraPlusModEnabled = false;
        public static bool SimpleCameraModEnabled = false;
        public static Color32 BarColor => AK_ModSettings.Color_RGB;
        public static Color32 BarColor_enemy => AK_ModSettings.Color_RGB_enemy;

        public static Material HP_Icon;
        public static Material DEF_Icon;
        public static Material Timer_Icon;
        public static Material BurstIcon;
        public static Material RotateRingIcon;

        public static Material BarUnfilledMat;
        public static Material HealthBarFilledMat => SolidColorMaterials.SimpleSolidColorMaterial(BarColor);
        public static Material EnemyHealthBarFilledMat => SolidColorMaterials.SimpleSolidColorMaterial(BarColor_enemy);
        public static Material SkillBarFilledMat;

        internal static Dictionary<string, GameObject> PrefabTMPInstancesDictionary = new();

        private static readonly string HP_IconTexPath = "UI/Abilities/icon_sort_hp";
        private static readonly string DEF_IconTexPath = "UI/Abilities/icon_sort_def";
        private static readonly string Timer_IconTexPath = "UI/Abilities/icon_sort_respawn";
        private static readonly string BurstIconTexPath = "UI/Abilities/Burst";
        private static readonly string RotateRingIconTexPath = "UI/Abilities/RotateRing";
        static AK_BarUITool()
        {
            try
            {
                InitializeIcons();
                if (ModLister.GetActiveModWithIdentifier("brrainz.cameraplus") != null)
                {
                    CameraPlusModEnabled = true;
                }
                else if (ModLister.GetActiveModWithIdentifier("ray1203.SimpleCameraSetting") != null)
                {
                    SimpleCameraModEnabled = true;
                }
            }
            catch
            {
                Log.Error("MIS. Critical Error: Initialization fail");
            }
        }
        internal static VAbility_Operator GetVAbility(this Pawn p)
        {
            if (p == null) return null;
            if (p.abilities == null) return null;
            VAbility_Operator va = null;
            Ability ability = p?.abilities?.abilities.Find((Ability a) => a.def == AKDefOf.AK_VAbility_Operator);
            if (ability != null)
            {
                va = ability as VAbility_Operator;
            }
            return va;
        }
        internal struct SimpleRectBarRequest
        {
            public Vector3 center;

            public Vector2 size;

            public Material filledMat;

            public Quaternion rotation;

        }
        internal static void DrawSimpleRectBar(SimpleRectBarRequest r)
        {
            Vector3 s = new(r.size.x, 1f, r.size.y);
            Matrix4x4 matrix = default;
            matrix.SetTRS(r.center, r.rotation, s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, r.filledMat, 0);
        }
        private static void InitializeIcons()
        {
            HP_Icon = MaterialPool.MatFrom(HP_IconTexPath, ShaderDatabase.Transparent);
            DEF_Icon = MaterialPool.MatFrom(DEF_IconTexPath, ShaderDatabase.Transparent);
            Timer_Icon = MaterialPool.MatFrom(Timer_IconTexPath, ShaderDatabase.Transparent);
            BurstIcon = MaterialPool.MatFrom(BurstIconTexPath, ShaderDatabase.Transparent);
            RotateRingIcon = MaterialPool.MatFrom(RotateRingIconTexPath, ShaderDatabase.Transparent);
            SkillBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(160, 170, 60, 200));
            BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.50f));
            Material[] checkList = { HP_Icon, DEF_Icon, Timer_Icon, BurstIcon, RotateRingIcon };
            foreach (Material mat in checkList)
            {
                if (mat == null)
                {
                    Log.Error("MIS. Critical Error: Missing UI Texture in file path: UI/Abilities/" + mat.name);
                }
            }

        }
        internal static string FormatTicksToHHMMSS(this int tick)
        {
            TimeSpan timespan;
            string result;
            //float TynanFactor = 2500 / 600;
            if (tick < 2500 && (tick < 600 || Math.Round((float)tick / 2500f, 1) == 0.0))
            {
                float seconds = (float)Math.Round(tick / 60f, 1);
                timespan = TimeSpan.FromSeconds(seconds);
                result = string.Format("{0:D2}H: {1:D2}M: {2:D2}S", timespan.Hours, timespan.Minutes, timespan.Seconds);
                return result;
            }
            if (tick < 60000)
            {
                float hours;
                hours = (float)Math.Round(tick / 2500f, 1);
                timespan = TimeSpan.FromHours(hours);
                result = string.Format("{0:D2}H: {1:D2}M: {2:D2}S", timespan.Hours, timespan.Minutes, timespan.Seconds);
                return result;
            }
            else//360000
            {
                float days = (float)Math.Round(tick / 60000f, 1);
                timespan = TimeSpan.FromDays(days);
                result = string.Format("{0:D2}H: {1:D2}M: {2:D2}S", timespan.Hours, timespan.Minutes, timespan.Seconds);
                return result;
            }
        }
        internal static float GetHealthPercent(this Pawn pawn)
        {
            float HealthPercent = pawn.health?.summaryHealth?.SummaryHealthPercent ?? (-1f);
            return HealthPercent;
        }
        internal static bool IsAlly(this Pawn pawn)
        {
            if (pawn == null) return false;
            if (pawn.Faction == null || pawn.Faction == Faction.OfPlayer)
            {
                return false;
            }
            if (pawn.RaceProps == null || !pawn.RaceProps.Humanlike || pawn.HostileTo(Faction.OfPlayer))
            {
                return false;
            }
            if (pawn.Faction?.PlayerGoodwill >= 75)
            {
                return true;
            }
            return false;
        }
        internal static bool IsNeutral(this Pawn pawn)
        {
            if (pawn == null) return false;
            if (pawn.Faction == null || pawn.Faction == Faction.OfPlayer)
            {
                return false;
            }
            if (pawn.RaceProps == null || !pawn.RaceProps.Humanlike || pawn.HostileTo(Faction.OfPlayer))
            {
                return false;
            }
            if (pawn.Faction?.PlayerGoodwill is >= 0 and < 75)
            {
                return true;
            }
            return false;
        }
    }
}
