using AKA_Ability;
using RimWorld;
using UnityEngine;
using TMPro;
using Verse;

namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public class TC_SkillBarController : ThingComp
    {
        private Pawn Pawn => parent as Pawn;
        private AKAbility ability;
        private VAbility_Operator operatorID;
        private bool IsGrouped = false;
        private float SkillPercent;
        private Vector3 IconMargin => Vector3.back * 1.0625f + Vector3.left * 0.8f;
        private static Vector3 TopMargin => Vector3.forward * 1f;
        private static Vector3 BottomMargin => Vector3.back * 1.075f;
        private static readonly Vector2 BarSize = new Vector2(1.5f, 0.075f);
        const float s = 1;
        private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(160, 170, 60, 180));
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.50f));
        private Material Timer_Icon;
        private Material RotateRing;
        private Material BurstButton;
        //使用prefab可以避免new GameObject出来的object不能用Find方法找到；
        GameObject PrefabTMP => AK_Tool.PAAsset.LoadAsset<GameObject>("PrefabTMPPopup");
        GameObject PrefabTMPInstance;
        public string ObjectName => (Pawn.GetDoc()?.operatorID ?? Pawn.Label) + ".objTMP";
        private bool MatRefreshed = false;
        private float RotateAngle = 0;
        private float BurstFlashFactor = 0;
        //一个Pawn身上只带一个唯一的Object
        private void InitObjectOnce()
        {
            if (PrefabTMPInstance == null)
            {
                PrefabTMPInstance = GameObject.Instantiate(PrefabTMP);
                PrefabTMPInstance.name = ObjectName;
                PrefabTMPInstance.layer = 2;
                PrefabTMPInstance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                //TextMeshPro compTMP = PrefabTMPInstance.GetComponent<TextMeshPro>();
                //compTMP.font = AK_Tool.PAAsset.LoadAsset<TMP_FontAsset>("NumFont Subset SDF");
                //compTMP.material = compTMP.font.material;
            }
            else
            {
                if (Pawn.Dead)
                {
                    if (PrefabTMPInstance == GameObject.Find(ObjectName))
                    {
                        GameObject.DestroyImmediate(PrefabTMPInstance);
                    }
                }
                return;
            }
        }
        private float CooldownPercent()
        {
            if (ability.cooldown.charge == ability.cooldown.maxCharge) return 1;
            return 1f - (float)ability.cooldown.CD / (float)ability.cooldown.maxCD;
        }
        private void GenBurstButton()
        {
            if (BurstButton == null)
            {
                BurstButton = AK_BarUITool.BurstIcon;
            }
        }
        private void GenRotateRing()
        {
            if (RotateRing == null)
            {
                RotateRing = AK_BarUITool.RotateRingIcon;
            }
        }
        private void GenTimerIcon()
        {
            if (Timer_Icon == null)
            {
                Timer_Icon = AK_BarUITool.Timer_Icon;
            }
        }
        public override void PostDraw()
        {
            base.PostDraw();
            if (!AK_ModSettings.displayBarModel || Pawn.GetDoc() == null)
            {
                return;
            }
            //倒地销毁TMP物件D
            if (Pawn.Downed || Pawn.Dead)
            {
                if (PrefabTMPInstance == GameObject.Find(ObjectName))
                {
                    GameObject.Destroy(PrefabTMPInstance);
                }
            }
            //征召显示开关
            if (PrefabTMPInstance != null)
            {
                PrefabTMPInstance?.SetActive(Pawn.Drafted || Pawn.Downed);
            }
            if (!Pawn.Drafted)
            {
                return;
            }
            if (operatorID == null)
            {
                operatorID = Pawn.abilities?.abilities.Find((Ability a) => a.def == AKDefOf.AK_VAbility_Operator) as VAbility_Operator;
            }
            if (operatorID != null && ability == null)
            {
                ability = operatorID?.AKATracker?.innateAbilities.Find((AKAbility a) => !a.def.grouped);
                if (ability == null)
                {
                    ability = operatorID?.AKATracker?.groupedAbilities.Find((AKAbility a) => a.def.grouped);
                    IsGrouped = true;
                }
            }
            //有AKA技能的Pawn才会显示技能CD进度，否则全为空
            if (ability != null)
            {
                SkillPercent = CooldownPercent();
            }
            else
            {
                SkillPercent = 0f;
            }
            GenDraw.FillableBarRequest fbr = default;
            fbr.center = Pawn.DrawPos + (Vector3.up * 3f) + BottomMargin;
            fbr.size = BarSize;
            fbr.filledMat = BarFilledMat;
            fbr.unfilledMat = BarUnfilledMat;
            //fbr.margin = 0;
            fbr.rotation = Rot4.North;
            fbr.fillPercent = (SkillPercent < 0f) ? 0f : SkillPercent;
            GenDraw.DrawFillableBar(fbr);
            GenTimerIcon();
            Matrix4x4 Imatrix = default;
            Imatrix.SetTRS(Pawn.DrawPos + IconMargin, Rot4.North.AsQuat, new Vector3(0.25f, 1f, 0.25f));
            Graphics.DrawMesh(MeshPool.plane025, Imatrix, material: Timer_Icon, 2);
            Vector3 OriginCenter = Pawn.DrawPos + TopMargin + (Vector3.up * 3f);
            Vector3 Scale = new Vector3(0.3f, 1f, 0.3f);
            //自动回复技能
            if (ability != null && ability.cooldown.charge == ability.cooldown.maxCharge && !IsGrouped && !MatRefreshed)
            {
                GenBurstButton();
                if (BurstButton != null)
                {
                    Matrix4x4 matrix = default;
                    matrix.SetTRS(OriginCenter, Rot4.North.AsQuat, Scale);
                    Graphics.DrawMesh(MeshPool.plane10, matrix, material: BurstButton, 2);
                    //闪烁
                    BurstFlashFactor = (BurstFlashFactor + 0.025f) % 1.5f;
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
            if (ability != null && IsGrouped && !MatRefreshed)
            {
                GenRotateRing();
                InitObjectOnce();
                if (RotateRing != null)
                {
                    Matrix4x4 matrix2 = default;
                    RotateAngle = (RotateAngle + 0.25f) % 360;
                    matrix2.SetTRS(OriginCenter, Quaternion.AngleAxis(RotateAngle, Vector3.up), Scale);
                    Graphics.DrawMesh(MeshPool.plane10, matrix2, material: RotateRing, 1);
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
