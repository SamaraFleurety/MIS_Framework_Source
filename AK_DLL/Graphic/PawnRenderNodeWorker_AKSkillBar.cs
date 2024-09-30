using AKA_Ability;
using TMPro;
using UnityEngine;
using Verse;
using System.Collections.Generic;

namespace AK_DLL
{
    //泰南我草你妈 Draw方法是交替执行的 Worker还是唯一实例
    public class PawnRenderNodeWorker_AKSkillBar : PawnRenderNodeWorker
    {
        //locOffset
        private static bool CameraPlusModEnabled => AK_BarUITool.CameraPlusModEnabled;
        private static bool SimpleCameraModEnabled => AK_BarUITool.SimpleCameraModEnabled;
        private ProgramState CurrentProgramState => Current.ProgramState;
        private float ZoomRootSize => Find.CameraDriver.ZoomRootSize;
        private static float Width => AK_ModSettings.barWidth * 0.01f;
        private static float Height => AK_ModSettings.barHeight * 0.001f;
        private static float Margin => AK_ModSettings.barMargin * 0.01f;
        private static float skillMargin => Margin * 1.075f;
        private static Vector2 BarSize => new Vector2(Width, Height);
        private static Vector3 BottomMargin => new Vector3(0f, 0f, skillMargin);
        //private static Vector2 BarSize = new Vector2(1.5f, 0.075f);
        //private static Vector3 BottomMargin = Vector3.back * 1.075f;
        private static Vector3 TopMargin = Vector3.forward * 1f;
        private static Vector3 IconMargin = Vector3.back * 1.0625f + Vector3.left * 0.8f;
        //Mat
        private static Material BarFilledMat => AK_BarUITool.SkillBarFilledMat;
        private static Material BarUnfilledMat => AK_BarUITool.BarUnfilledMat;
        private static Material Timer_Icon => AK_BarUITool.Timer_Icon;
        private static Material RotateRing => AK_BarUITool.RotateRingIcon;
        private static Material BurstButton => AK_BarUITool.BurstIcon;
        private string OperatorID(Pawn p) => (p.GetDoc()?.operatorID ?? p.Label);
        private string ObjectName(Pawn p) => (p.GetDoc()?.operatorID ?? p.Label) + ".objTMP";
        private GameObject PrefabTMP => AK_Tool.PAAsset.LoadAsset<GameObject>("PrefabTMPPopup");
        //使用prefab可以避免new GameObject出来的object被回收不能用Find方法找到；
        private Dictionary<string, GameObject> PrefabTMPInstancesDictionary = new Dictionary<string, GameObject>();
        private Dictionary<string, float> RotateAngleDictionary = new Dictionary<string, float>();
        private Dictionary<string, float> BurstFlashFactorDictionary = new Dictionary<string, float>();
        private void InitObjectOnce(Pawn p)
        {
            if (PrefabTMPInstancesDictionary.NullOrEmpty() || !PrefabTMPInstancesDictionary.ContainsKey(OperatorID(p)))
            {
                GameObject PrefabTMPInstance = GameObject.Instantiate(PrefabTMP);
                PrefabTMPInstance.name = ObjectName(p);
                PrefabTMPInstance.layer = 2;
                PrefabTMPInstance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                PrefabTMPInstancesDictionary.Add(OperatorID(p), PrefabTMPInstance);
            }
            else
            {
                if (p.Dead)
                {
                    GameObject PrefabTMPInstance = PrefabTMPInstancesDictionary.TryGetValue(OperatorID(p));
                    if (PrefabTMPInstance == GameObject.Find(ObjectName(p)))
                    {
                        GameObject.DestroyImmediate(PrefabTMPInstance);
                        PrefabTMPInstancesDictionary.Remove(OperatorID(p));
                    }
                }
                return;
            }
        }
        private float CooldownPercent(AKAbility ability)
        {
            return ability.cooldown.CooldownPercent() * -1;
            /*if (ability.cooldown.charge == ability.cooldown.MaxCharge)
            {
                return 1;
            }
            return 1f - (float)ability.cooldown.CDCurrent / (float)ability.cooldown.CDPerCharge;*/
        }
        private void DrawIcon(Vector3 pos)
        {
            Matrix4x4 matrix = default;
            matrix.SetTRS(pos + IconMargin, Rot4.North.AsQuat, new Vector3(0.25f, 1f, 0.25f));
            Graphics.DrawMesh(MeshPool.plane025, matrix, material: Timer_Icon, 2);
        }
        private float GetZoomRatio()
        {
            if (AK_ModSettings.zoomWithCamera)
            {
                return Mathf.Max(ZoomRootSize, 11) / 11;
            }
            return 1f;
        }
        private void DrawSkillBar(Material mat, Pawn pawn)
        {
            Vector3 drawPos = pawn.DrawPos;
            float percent = pawn.GetHealthPercent();
            float zoomRatio = GetZoomRatio();
            float zoomWidthRatio;
            float zoomYRatio;
            if (CameraPlusModEnabled || SimpleCameraModEnabled)
            {
                zoomWidthRatio = zoomRatio > 4.35f ? 4.35f : zoomRatio;
                zoomYRatio = zoomRatio > 5f ? 5f : zoomRatio;
            }
            else
            {
                zoomWidthRatio = zoomRatio > 3.75f ? 3.75f : zoomRatio;
                zoomYRatio = zoomRatio > 3f ? 3f : zoomRatio;
            }
            //
            GenDraw.FillableBarRequest fbr = default;
            if (CameraPlusModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.9f : zoomYRatio);
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 1.75f ? zoomRatio * 1.5f : zoomRatio;
            }
            else if (SimpleCameraModEnabled)
            {
                fbr.center = drawPos + (Vector3.up * 3f) + BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio);
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 3f ? zoomRatio * 1.05f : zoomRatio;
            }
            else
            {
                fbr.center = drawPos + (Vector3.up * 3f) + BottomMargin * (zoomYRatio > 1.75f ? zoomYRatio * 0.75f : zoomYRatio);
                fbr.size = BarSize;
                fbr.size.x *= zoomWidthRatio;
                fbr.size.y *= zoomRatio > 6.5f ? zoomRatio * 1.25f : zoomRatio;
            }
            fbr.fillPercent = (percent < 0f) ? 0f : percent;
            fbr.filledMat = mat;
            fbr.unfilledMat = BarUnfilledMat;
            fbr.rotation = Rot4.North;
            GenDraw.DrawFillableBar(fbr);
            DrawIcon(fbr.center);
            //Log.Message("现在的摄像机缩放" + Find.CameraDriver.ZoomRootSize.ToString());
            //Log.Message("现在的GUI缩放" + (Find.Camera.WorldToScreenPoint(drawPos) / Prefs.UIScale).ToString());
        }
        public override bool CanDrawNow(PawnRenderNode node, PawnDrawParms parms)
        {
            Pawn pawn = parms.pawn;
            if (!AK_ModSettings.displayBar || CurrentProgramState != ProgramState.Playing || pawn == null || pawn.Dead)
            {
                return false;
            }
            if (pawn.GetDoc() != null)
            {
                return true;
            }
            return false;
        }
        public override void PostDraw(PawnRenderNode node, PawnDrawParms parms, Mesh mesh, Matrix4x4 matrix)
        {
            Pawn pawn = parms.pawn;
            if (!AK_ModSettings.displayBar || pawn.GetDoc() == null)
            {
                return;
            }
            //倒地销毁TMP物件D
            GameObject PrefabTMPInstance = PrefabTMPInstancesDictionary.TryGetValue(OperatorID(pawn));
            if (pawn.Downed || pawn.Dead)
            {
                if (PrefabTMPInstance == GameObject.Find(ObjectName(pawn)))
                {
                    GameObject.Destroy(PrefabTMPInstance);
                    PrefabTMPInstancesDictionary.Remove(OperatorID(pawn));
                }
            }
            //征召显示开关
            if (PrefabTMPInstance != null)
            {
                PrefabTMPInstance?.SetActive(pawn.Drafted || pawn.Downed);
            }
            if (!pawn.Drafted)
            {
                return;
            }
            float SkillPercent = 0f;
            bool IsGrouped = false;
            VAbility_Operator operatorID = null;
            AKAbility ability = null;
            if (operatorID == null)
            {
                operatorID = pawn.GetVAbility();
            }
            if (operatorID != null && ability == null)
            {
                ability = operatorID?.AKATracker?.innateAbilities.Find((AKAbility a) => !a.def.grouped);
                IsGrouped = false;
                if (ability == null)
                {
                    ability = operatorID?.AKATracker?.groupedAbilities.Find((AKAbility a) => a.def.grouped);
                    IsGrouped = true;
                }
            }
            //有AKA技能的Pawn才会显示技能CD进度，否则全为空
            if (ability != null)
            {
                SkillPercent = CooldownPercent(ability);
            }
            else
            {
                SkillPercent = 0f;
            }
            DrawSkillBar(BarFilledMat, pawn);
            /*GenDraw.FillableBarRequest fbr = default;
            fbr.center = pawn.DrawPos + (Vector3.up * 3f) + BottomMargin;
            fbr.size = BarSize;
            fbr.filledMat = BarFilledMat;
            fbr.unfilledMat = BarUnfilledMat;
            //fbr.margin = 0;
            fbr.rotation = Rot4.North;
            fbr.fillPercent = (SkillPercent < 0f) ? 0f : SkillPercent;
            GenDraw.DrawFillableBar(fbr);
            //图标
            Matrix4x4 matrix1 = default;
            matrix1.SetTRS(pawn.DrawPos + IconMargin, Rot4.North.AsQuat, new Vector3(0.25f, 1f, 0.25f));
            Graphics.DrawMesh(MeshPool.plane025, matrix1, material: Timer_Icon, 2);
            */
            Vector3 OriginCenter = pawn.DrawPos + TopMargin + (Vector3.up * 3f);
            Vector3 Scale = new Vector3(0.3f, 1f, 0.3f);
            //自动回复技能
            if (ability != null && ability.cooldown.charge == ability.cooldown.MaxCharge && !IsGrouped)
            {
                if (BurstButton != null)
                {
                    if (!BurstFlashFactorDictionary.ContainsKey(OperatorID(pawn)))
                    {
                        BurstFlashFactorDictionary.SetOrAdd(OperatorID(pawn), 0f);
                    }
                    Matrix4x4 matrix2 = default;
                    matrix2.SetTRS(OriginCenter, Rot4.North.AsQuat, Scale);
                    Graphics.DrawMesh(MeshPool.plane10, matrix2, material: BurstButton, 2);
                    //闪烁
                    float BurstFlashFactor = BurstFlashFactorDictionary[OperatorID(pawn)];
                    BurstFlashFactor = (BurstFlashFactor + 0.025f) % 1.5f;
                    BurstFlashFactorDictionary[OperatorID(pawn)] = BurstFlashFactor;
                    float factor = Mathf.Sqrt(BurstFlashFactor);
                    float transparency = 120 - (BurstFlashFactor / 1.2f * 70);
                    //float transparency = Mathf.Lerp(150, 0, factor);
                    AK_BarUITool.SimpleRectBarRequest sbr = default;
                    sbr.center = OriginCenter + Vector3.down;
                    sbr.filledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(255, 255, 0, (byte)Mathf.Max(transparency, 20f))); ;
                    sbr.rotation = Quaternion.AngleAxis(45f, Vector3.up);
                    sbr.size = new Vector2(0.25f * BurstFlashFactor, 0.25f * BurstFlashFactor);
                    AK_BarUITool.DrawSimpleRectBar(sbr);
                }
            }
            //充能技能
            if (ability != null && IsGrouped)
            {
                InitObjectOnce(pawn);
                if (RotateRing != null)
                {
                    if (!RotateAngleDictionary.ContainsKey(OperatorID(pawn)))
                    {
                        RotateAngleDictionary.SetOrAdd(OperatorID(pawn), 0f);
                    }
                    float RotateAngle = RotateAngleDictionary[OperatorID(pawn)];
                    RotateAngle = (RotateAngle + 0.25f) % 360;
                    RotateAngleDictionary[OperatorID(pawn)] = RotateAngle;
                    Matrix4x4 matrix3 = default;
                    matrix3.SetTRS(OriginCenter, Quaternion.AngleAxis(RotateAngle, Vector3.up), Scale);
                    Graphics.DrawMesh(MeshPool.plane10, matrix3, material: RotateRing, 1);
                }
                if (PrefabTMPInstance != null)
                {
                    TextMeshPro tmp = PrefabTMPInstance.GetComponent<TextMeshPro>();
                    Vector3 scale = new Vector3(0.25f, 0.25f, 1f);
                    int ChargeTimes = ability.cooldown.charge;
                    tmp.transform.position = OriginCenter;
                    tmp.transform.localScale = scale;
                    tmp.fontSize = 6;
                    tmp.SetText(ChargeTimes.ToString());
                    if (!PrefabTMPInstance.activeSelf)
                    {
                        PrefabTMPInstance.SetActive(true);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

    }
}
