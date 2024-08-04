using AKA_Ability;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Verse;


namespace AK_DLL
{
    [StaticConstructorOnStartup]
    public class SkillBarControllerComp : ThingComp
    {
        //TCP_SkillBarControllerComp Props => (TCP_SkillBarControllerComp)props;
        private Pawn pawn => parent as Pawn;
        private AKAbility ability;
        private VAbility_Operator operatorID;
        private bool IsGrouped = false;
        private float SkillPercent;
        private Vector3 IconMargin => Vector3.back * 1.0625f + Vector3.left * 0.8f;
        private static Vector3 TopMargin => Vector3.forward * 1f;
        private static Vector3 BottomMargin => Vector3.back * 1.075f;
        private static Vector2 BarSize = new Vector2(1.5f, 0.075f);
        //绿色技能充能条 和 橙色技能消耗条；
        private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(160, 170, 60, 180));
        private static readonly Material BarConsumedMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color32(255, 165, 0, 180));
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f, 0.50f));
        private Material Timer_Icon;
        private Material RotateRing;
        private Material BurstButton;
        //使用prefab可以避免new GameObject出来的object不能用Find方法找到；
        GameObject PrefabTMP => AK_Tool.PAAsset.LoadAsset<GameObject>("PrefabTMPPopup");
        GameObject PrefabTMPInstance;
        public string ObjectName => (pawn.GetDoc()?.operatorID ?? pawn.Label) + ".objTMP";
        private bool MatRefreshed = false;
        private static bool IsClosed = true;
        private float RotateAngle = 0;
        //一个Pawn身上只带一个唯一的Object
        private void InitObjectOnce()
        {
            if (PrefabTMPInstance == null)
            {
                PrefabTMPInstance = GameObject.Instantiate(PrefabTMP);
                PrefabTMPInstance.name = ObjectName;
                TextMeshPro compTMP = PrefabTMPInstance.GetComponent<TextMeshPro>();
                PrefabTMPInstance.layer = 2;
                PrefabTMPInstance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                //compTMP.font = AK_Tool.PAAsset.LoadAsset<TMP_FontAsset>("NumFont Subset SDF");
                //compTMP.material = compTMP.font.material;
                //compTMP.renderer.material = compTMP.material;
            }
            else
            {
                if (pawn.Dead)
                {
                    if (GameObject.Find(ObjectName))
                    {
                        GameObject.Destroy(GameObject.Find(ObjectName));
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
        private void SetAct(bool value)
        {
            if (value)
            {
                PrefabTMPInstance?.SetActive(!IsClosed);
            }
            else
            {
                PrefabTMPInstance?.SetActive(IsClosed);
            }
        }
        public override void PostDraw()
        {
            base.PostDraw();
            if (AK_ModSettings.displayBarModel)
            {
                //征召显示开关
                if (!pawn.Drafted || pawn.GetDoc() == null)
                {
                    IsClosed = true;
                    SetAct(true);
                    //GameObject.Find(ObjectName)?.SetActive(false);
                    return;
                }
                if (pawn.Drafted)
                {
                    IsClosed = false;
                    SetAct(true);
                }
                GenDraw.FillableBarRequest fbr = default;
                fbr.center = pawn.DrawPos + (Vector3.up * 5f) + BottomMargin;
                fbr.size = BarSize;
                fbr.filledMat = BarFilledMat;
                fbr.unfilledMat = BarUnfilledMat;
                fbr.margin = 0.001f;
                fbr.rotation = Rot4.North;
                operatorID = pawn.abilities?.abilities.Find((Ability a) => a.def == AKDefOf.AK_VAbility_Operator) as VAbility_Operator;
                if (operatorID != null)
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
                fbr.fillPercent = (SkillPercent < 0f) ? 0f : SkillPercent;
                GenDraw.DrawFillableBar(fbr);
                GenTimerIcon();
                Matrix4x4 Imatrix = default;
                Imatrix.SetTRS(pawn.DrawPos + IconMargin, Rot4.North.AsQuat, new Vector3(0.25f, 1f, 0.25f));
                Graphics.DrawMesh(MeshPool.plane025, Imatrix, material: Timer_Icon, 2);
                Vector3 OriginCenter = pawn.DrawPos + TopMargin + (Vector3.up * 5f);
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
                    GameObject Instance = GameObject.Find(ObjectName);
                    if (Instance != null)
                    {
                        //删去了单独DrawTMP方法
                        TextMeshPro tmp = Instance.GetComponent<TextMeshPro>();
                        Vector3 scale = new Vector3(0.25f, 0.25f, 1f);
                        int ChargeTimes = ability.cooldown.charge;
                        tmp.transform.position = OriginCenter;
                        tmp.transform.localScale = scale;
                        tmp.fontSize = 6;
                        tmp.SetText(ChargeTimes.ToString());
                        Instance.SetActive(true);
                        IsClosed = false;
                    }
                }
            }
        }
    }
}
