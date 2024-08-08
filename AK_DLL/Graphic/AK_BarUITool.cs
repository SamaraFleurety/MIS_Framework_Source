using UnityEngine;
using Verse;

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
            }
            catch
            {
                Log.Error("MIS. Critical Error: Initialization fail");
            }
        }
        public struct SimpleRectBarRequest
        {
            public Vector3 center;

            public Vector2 size;

            public Material filledMat;

            public Quaternion rotation;

        }
        public static void DrawSimpleRectBar(SimpleRectBarRequest r)
        {
            Vector3 s = new Vector3(r.size.x, 1f, r.size.y);
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
            Material[] checkList = { HP_Icon, DEF_Icon, Timer_Icon, BurstIcon, RotateRingIcon };
            foreach (Material icon in checkList)
            {
                if (icon == null)
                {
                    Log.Error("MIS. Critical Error: Missing UI Texture in file path: UI/Abilities/" + icon.name);
                }
            }

        }

        public static void AddCompsToPawn(Pawn p)
        {
            if (p == null || p.Dead) return;

            bool hasHealthComp = false;
            bool hasSkillComp = false;
            bool hasShieldComp = false;

            foreach (ThingComp comp in p.AllComps)
            {
                if (comp is TC_HealthBarController) hasHealthComp = true;
                else if (comp is TC_SkillBarController) hasSkillComp = true;
                else if (comp is TC_ShieldBarController) hasShieldComp = true;
            }

            if (ModLister.GetActiveModWithIdentifier("Mlie.VanyaShield") == null) hasShieldComp = true;

            if (!hasHealthComp) p.AllComps.Add(new TC_HealthBarController() { parent = p });
            if (!hasSkillComp) p.AllComps.Add(new TC_SkillBarController() { parent = p });
            if (!hasShieldComp) p.AllComps.Add(new TC_ShieldBarController() { parent = p });

        }
    }
}
