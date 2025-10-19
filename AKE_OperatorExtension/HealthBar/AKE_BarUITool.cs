using AK_DLL;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AKE_OperatorExtension
{
    [StaticConstructorOnStartup]
    public static class AKE_BarUITool
    {
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

        private const string HP_IconTexPath = "UI/Abilities/icon_sort_hp";
        private const string DEF_IconTexPath = "UI/Abilities/icon_sort_def";
        private const string Timer_IconTexPath = "UI/Abilities/icon_sort_respawn";
        private const string BurstIconTexPath = "UI/Abilities/Burst";
        private const string RotateRingIconTexPath = "UI/Abilities/RotateRing";

        static AKE_BarUITool()
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

        internal static VAbility_Operator GetVAbility(this Pawn p)
        {
            if (p?.abilities == null) return null;
            VAbility_Operator va = null;
            Ability ability = p?.abilities?.abilities.Find(a => a.def == AKDefOf.AK_VAbility_Operator);
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
                if (!mat)
                {
                    Log.Error("MIS. Critical Error: Missing UI Texture in file path: UI/Abilities/" + mat.name);
                }
            }

        }

        internal static string FormatTicksToDate(this int tick)
        {
            TimeSpan timespan;
            //float TynanFactor = 2500 / 600;
            if (tick < 2500 && (tick < 600 || Math.Round(tick / 2500f, 1) == 0.0))
            {
                float seconds = (float)Math.Round(tick / 60f, 1);
                timespan = TimeSpan.FromSeconds(seconds);
            }
            else if (tick < 60000)
            {
                float hours = (float)Math.Round(tick / 2500f, 1);
                timespan = TimeSpan.FromHours(hours);
            }
            else//360000
            {
                float days = (float)Math.Round(tick / 60000f, 1);
                timespan = TimeSpan.FromDays(days);
            }
            string result = $"{timespan.Hours:D2}H: {timespan.Minutes:D2}M: {timespan.Seconds:D2}S";
            return result;
        }

        internal static float HealthPercentage(this Pawn pawn)
        {
            float healthPercent = pawn.health?.summaryHealth?.SummaryHealthPercent ?? -1f;
            return healthPercent;
        }

        internal static bool IsAlly(this Pawn pawn)
        {
            if (pawn?.Faction == null || pawn.Faction == Faction.OfPlayer)
                return false;
            if (pawn.RaceProps is not { Humanlike: true } || pawn.HostileTo(Faction.OfPlayer))
                return false;
            if (pawn.Faction?.PlayerGoodwill >= 75)
                return true;
            return false;
        }

        internal static bool IsNeutral(this Pawn pawn)
        {
            if (pawn?.Faction == null || pawn.Faction == Faction.OfPlayer)
                return false;
            if (pawn.RaceProps is not { Humanlike: true } || pawn.HostileTo(Faction.OfPlayer))
                return false;
            if (pawn.Faction?.PlayerGoodwill is >= 0 and < 75)
                return true;
            return false;
        }
    }
}
