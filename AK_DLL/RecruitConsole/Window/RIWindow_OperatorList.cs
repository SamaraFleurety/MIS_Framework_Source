using System;
using Verse;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//记得给skillandfire写排序，在调用之前
namespace AK_DLL
{
    #region legacy
    /*public class RIWindow_OperatorList : Dialog_NodeTree
    {
        private static readonly int maxX = 1920, maxY = 1080;
        private static readonly int btnHeight = 20;
        private static readonly int btnWidth = 80;
        private static readonly int xMargin = 10, escapeBtnMargin = 10;
        private static readonly int yMargin = 10;
        private static readonly int classSideLength = 70;
        private static readonly int classMargin;
        private static int sortType;
        private static bool sortReverseOrder = false;
        private int partitionWidth = 2, partitionVerticalX, partitionHorizontalY;
        private int btnLength => RIWindow_MainMenu.btnLength;
        public RIWindow_OperatorList(DiaNode startNode, bool radioMode) : base(startNode, radioMode, false, null)
        {

        }
        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(maxX, maxY);
            }
        }
        #endregion
        public static Dictionary<int, Dictionary<string, OperatorDef>> operatorDefs => RIWindowHandler.operatorDefs;
        public static Dictionary<int, OperatorClassDef> operatorClasses => RIWindowHandler.operatorClasses;

        public override void PreOpen()
        {
            base.PreOpen();
            if (operatorDefs == null)
            {
                Log.Error("MIS. 干员库字典是空的.");
            }
            if (operatorDefs[(int)operatorType] == null)
            {
                Log.Error($"MIS.{operatorType}号干员库是null.");
            }
            if (operatorType >= 1 && cachedOperatorList.Count == 0)
            {
                cachedOperatorList = operatorDefs[operatorType].Values.ToList();
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
            float crntX = inRect.x + xMargin;
            float crntY = inRect.y + yMargin;
            Rect rect_Back = new Rect(inRect.xMax - btnLength - escapeBtnMargin, inRect.y, btnLength / 2, btnLength / 2);
            //返回上一级按钮
            if (Widgets.ButtonText(rect_Back, "AK_Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent)
            {
                this.Close();
                RIWindowHandler.OpenRIWindow(RIWindow.MainMenu);
            }
            //返回主界面按钮
            rect_Back.x += btnLength / 2 + escapeBtnMargin;
            if (Widgets.ButtonText(rect_Back, "退出按钮"))
            {
                RIWindow_OperatorDetail.isRecruit = true;
                this.Close();
            }
            if (operatorType == -1)
            {
                Log.WarningOnce("MIS.No operator classes found.", 1);
                return;
            }

            //干员职业按钮
            Rect btnRect = new Rect(crntX, crntY, btnWidth, btnHeight);
            Rect classBtn = new Rect(inRect.xMax - xMargin - classSideLength, yMargin * 2 + 100, classSideLength, classSideLength);
            foreach (KeyValuePair<int, OperatorClassDef> i in operatorClasses)
            {
                /*if (Widgets.ButtonText(new Rect(inRect.x + xOffset, inRect.y + 15f, 80f, 20f), i.Value, true, true, operatorType != i.Key)) operatorType = i.Key;
                xOffset += 100;
                if (Widgets.ButtonText(classBtn, i.Value.label.Translate(), true, true, operatorType != i.Key))
                {
                    sortType = -1;
                    operatorType = i.Key;
                    cachedOperatorList = operatorDefs[operatorType].Values.ToList();
                }
                classBtn.y += btnWidth + 25;
            }

            //隔断
            partitionHorizontalY = (int)inRect.y + btnLength / 2;
            partitionVerticalX = (int)inRect.xMax - btnLength - escapeBtnMargin;
            Rect partition = new Rect(inRect.x, partitionHorizontalY, inRect.xMax, partitionWidth);
            GUI.DrawTexture(partition, BaseContent.WhiteTex);

            partition = new Rect(partitionVerticalX, inRect.y, partitionWidth, inRect.yMax);
            GUI.DrawTexture(partition, BaseContent.WhiteTex);
            /*btnRect.x = crntX;
            btnRect.y += btnHeight + yMargin * 2 + classSideLength / 2;
            btnRect.size = inRect.size;
            GUI.DrawTexture(new Rect(btnRect.position, new Vector2(inRect.width - 2 * xMargin, 2f)), BaseContent.WhiteTex);
            btnRect.y += yMargin;

            //统一绘制干员            
            DrawSortBtns(inRect);
            DrawOperatorList(btnRect);
            
        }*/
    #endregion

    [HotSwappable]
    public class RIWindow_OperatorList : RIWindow
    {

        private static int sortType = -1;
        private static bool sortReverseOrder = false;
        //排序按钮的面板（通用父类）
        private static Transform sorterColumnLoc = null;
        //干员面板
        private static Transform opListLoc = null;
        private static List<GameObject> opList = new List<GameObject>();
        private static GameObject opRectPrefab = null;
        //相当投机取巧地在名字里面存储数据 字符串内可以随便填。现在的值是12。
        private static int orderInName = "FSUI_whatev_".Length;
        public override void DoContent()
        {
            DrawSortBtns();
            DrawNavBtn();
            DrawOperatorList();
        }

        private GameObject ClickedBtn
        {
            get
            {
                return EventSystem.current.currentSelectedGameObject;
            }
        }

        private void DrawNavBtn()
        {
            GameObject navBtn;
            //左上角返回按钮 虽然现在返回就是主界面，但就是有这个按钮
            navBtn = GameObject.Find("btnBack").transform.GetChild(0).gameObject;
            navBtn.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                this.Close();
                RIWindowHandler.OpenRIWindow(RIWindowType.MainMenu);
            });
            //主界面按钮
            navBtn = GameObject.Find("btnHome").transform.GetChild(0).gameObject;
            navBtn.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                this.Close();
                RIWindowHandler.OpenRIWindow(RIWindowType.MainMenu);
            });
            //退出按钮
            navBtn = GameObject.Find("btnEscape").transform.GetChild(0).gameObject;
            navBtn.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                this.Close();
            });
        }

        public void DrawOperatorList()
        {
            if (sortType == -1)
            {
                sortType = RIWindowHandler.operatorClasses.First().Key;
                cachedOperatorList = RIWindowHandler.operatorDefs[sortType].Values.ToList();
            }
            if (opListLoc == null) opListLoc = GameObject.Find("OpRegister").transform;
            if (opRectPrefab == null)
            {
                opRectPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("OperatorTemplate");
                opRectPrefab.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                {
                    this.Close();
                    RIWindowHandler.OpenRIWindow_OpDetail(cachedOperatorList[int.Parse(ClickedBtn.name.Substring(orderInName))]);
                });
            }
            Log.Message($"{opRectPrefab.transform.localPosition.x}, {opRectPrefab.transform.localPosition.y}");
            //fixme：tmd写重了 
            if (!sortReverseOrder)
            {
                for (int i = 0; i < Math.Max(cachedOperatorList.Count, opList.Count); ++i)
                {
                    DrawOperatorPortrait(i, cachedOperatorList[i]);
                }
            }
            else
            {
                for (int i = 0; i < Math.Max(cachedOperatorList.Count, opList.Count); ++i)
                {
                    DrawOperatorPortrait(i, cachedOperatorList[i]);
                }
            }
            //干员绘制
        }

        private void DrawOperatorPortrait(int i, OperatorDef def)
        {
            Log.Message($"drawing {def.name}");
            Log.Message($"{i}, {cachedOperatorList.Count}");
            Texture2D operatorTex = ContentFinder<Texture2D>.Get(def.headPortrait);
            GameObject opRectInstance;
            if (i >= cachedOperatorList.Count)
            {
                Log.Message("a");
                opList[i].SetActive(false);
                return;
            }
            if (i >= opList.Count || opList[i] == null)
            {
                opRectInstance = GameObject.Instantiate(opRectPrefab, opListLoc);
                opRectInstance.transform.parent = opListLoc;
                opList.Add(opRectInstance);
                opRectInstance.SetActive(true);
            }
            else
            {
                opRectInstance = opList[i];
            }
            //设定名字中 以后检索干员def时用的数字序
            opRectInstance.name = "FSUI_Opertr_" + i;
            //设定头像 决定干员头像的子物体是第一个子物体。
            opRectInstance.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(operatorTex, new Rect(0, 0, operatorTex.width, operatorTex.height), new Vector2(0, 0));
            //决定头像框的位置。目前一行9个，共3行。
            Log.Message($"{opRectPrefab.transform.localPosition.x}, {opRectPrefab.transform.localPosition.y}");
            Log.Message($"{opRectInstance.transform.localPosition.x}, {opRectInstance.transform.localPosition.y}");
            opRectInstance.transform.localPosition = new Vector3((i / 9 * -9 + i) * opRectInstance.transform.localPosition.x, (i / 9) * opRectInstance.transform.localPosition.y);
            Log.Message($"{opRectInstance.transform.localPosition.x}, {opRectInstance.transform.localPosition.y}");
            
        }


        #region 排序相关
        private void DrawSortBtns()
        {
            GameObject sortBtnPrefab = bundle.LoadAsset<GameObject>("btnSortTemplate");
            GameObject sortBtnInstance;
            TextMeshProUGUI textTMP;
            //Rect sortBtn = new Rect(inRect.xMin, inRect.y + yMargin, classSideLength, classSideLength / 2);
            //按某种技能的等级排序
            if (sorterColumnLoc == null)
            {
                sorterColumnLoc = GameObject.Find("sorterBg").transform;
            }
            for (int i = 0; i < TypeDef.SortOrderSkill.Count(); ++i)
            {
                sortBtnInstance = GameObject.Instantiate(sortBtnPrefab, sorterColumnLoc);
                textTMP = sortBtnInstance.GetComponentInChildren<TextMeshProUGUI>();
                //按钮的显示文字
                textTMP.text = TypeDef.SortOrderSkill[i].label.Translate();
                //使用了投机取巧而不是正常的方式存储数据。
                sortBtnInstance.name = "FSUI_Sorter_" + i;
                sortBtnInstance.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    sortBtnInstance = ClickedBtn;
                    Log.Message($"clicked {sortBtnInstance.name.Substring(orderInName)}");
                    if (NeedSortTo(int.Parse(sortBtnInstance.name.Substring(orderInName))))
                    {
                        SortOperator<int>(delegate (int a, int b)
                        {
                            return !(a <= b);
                        }, delegate (OperatorDef def)
                        {
                            return def.SortedSkills[i].level;
                        });
                    }
                });
                sortBtnInstance.SetActive(true);

                sortBtnInstance.transform.localPosition = new Vector3(sortBtnInstance.transform.localPosition.x * (i - (i / 7 * 7)), sortBtnInstance.transform.localPosition.y * (i / 7), i);
                Log.Message($"{i}: {sortBtnInstance.transform.localPosition.x}, {sortBtnInstance.transform.localPosition.y}, {sortBtnInstance.transform.localPosition.z}");
            }
            /*if (Widgets.ButtonText(sortBtn, TypeDef.SortOrderSkill[i].label.Translate()))
            {
                if (NeedSortTo(i))
                {
                    SortOperator<int>(delegate (int a, int b)
                    {
                        return !(a <= b);
                    }, delegate (OperatorDef def)
                    {
                        return def.SortedSkills[i].level;
                    });
                }
            }
        }
        //FIXME 按面板DPS排序
        if (Widgets.ButtonText(sortBtn, "AK_SortDPS".Translate()))
        {
            if (NeedSortTo((int)OperatorSortType.Dps))
            {
                SortOperator<double>(delegate (double a, double b)
                {
                    return !(a <= b);
                }, delegate (OperatorDef def)
                {
                    return DPSCalculator(def);
                });
            }
        }
        sortBtn.x += classSideLength + xMargin;
        //按字母表排序
        if (Widgets.ButtonText(sortBtn, "AK_SortAlphabet".Translate()))
        {
            if (NeedSortTo((int)OperatorSortType.Alphabet))
            {
                SortOperator<string>(delegate (string a, string b)
                {
                    return string.Compare(a, b) <= 0;
                }, delegate (OperatorDef def)
                {
                    return def.label.Translate();
                });
            }
        }*/
        }

        private double DPSCalculator(OperatorDef def)
        {
            double meleeDPS = 0;
            double rangedDPS = 0;
            double localDPS = 0;
            ThingDef w = def.weapon;
            if (w != null)
            {
                //计算命中率100%时的近战dps
                if (w.tools != null)
                {
                    double weight = 0;
                    foreach (Tool i in w.tools)
                    {
                        localDPS = 0;
                        localDPS += i.power;
                        if (i.extraMeleeDamages != null)
                        {
                            foreach (ExtraDamage j in i.extraMeleeDamages)
                            {
                                localDPS += (j.amount * Math.Min(1.0, j.chance));
                            }
                        }
                        localDPS /= i.cooldownTime;
                        localDPS *= i.chanceFactor;
                        meleeDPS += localDPS;
                        weight += i.chanceFactor;
                    }
                    if (weight == 0) meleeDPS = 0;
                    else meleeDPS /= weight;
                }
                Log.Message($"p3 : {w.defName}");
                //命中率100%远程dps
                if (w.Verbs != null && w.Verbs.Count > 0)
                {
                    Log.Message(w.Verbs[0].ToString());
                    Log.Message(w.Verbs[0].verbClass.ToString());
                    VerbProperties verb = w.Verbs[0];
                    ProjectileProperties bullet = verb.defaultProjectile.projectile;
                    if (bullet == null) goto LABEL_NoRanged;
                    System.Reflection.FieldInfo damage = bullet.GetType().GetField("damageAmountBase", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    //单发伤害
                    rangedDPS = (int)damage.GetValue(bullet);
                    if (bullet.extraDamages != null) foreach (ExtraDamage l in bullet.extraDamages) rangedDPS += (l.amount * Math.Min(l.chance, 1.0));

                    double realCD = verb.warmupTime;
                    foreach (StatModifier k in w.statBases)
                    {
                        if (k.stat == StatDefOf.RangedWeapon_Cooldown)
                        {
                            realCD += k.value;
                            break;
                        }
                    }
                    if (verb.burstShotCount > 1)
                    {
                        rangedDPS *= verb.burstShotCount;
                        realCD += verb.ticksBetweenBurstShots / 60 * (verb.burstShotCount - 1);
                    }
                    if (realCD <= 0) goto LABEL_NoRanged;
                    rangedDPS /= realCD;
                }
            }
            return Math.Max(meleeDPS, rangedDPS); ;
        LABEL_NoRanged:
            return meleeDPS;
        }
        private bool NeedSortTo(int newSortType)
        {
            if (sortType == newSortType)
            {
                sortReverseOrder = !sortReverseOrder;
                return false;
            }
            else
            {
                sortReverseOrder = false;
                sortType = newSortType;
                return true;
            }
        }

        private void Merge<T>(int i, int j, int k, Func<T, T, bool> comparer, Func<OperatorDef, T> compraree)
        {
            int mergedSize = k - i + 1;
            List<OperatorDef> mergedOps = new List<OperatorDef>(new OperatorDef[mergedSize]);

            int mergePos = 0, leftPos = i, rightPos = j + 1;

            while (leftPos <= j && rightPos <= k)
            {
                if (comparer(compraree(cachedOperatorList[leftPos]), compraree(cachedOperatorList[rightPos])))
                {
                    mergedOps[mergePos] = cachedOperatorList[leftPos];
                    ++leftPos;
                }
                else
                {
                    mergedOps[mergePos] = cachedOperatorList[rightPos];
                    ++rightPos;

                }
                ++mergePos;
            }

            while (leftPos <= j)
            {
                mergedOps[mergePos] = cachedOperatorList[leftPos];
                ++leftPos;
                ++mergePos;
            }

            while (rightPos <= k)
            {
                mergedOps[mergePos] = cachedOperatorList[rightPos];
                ++rightPos;
                ++mergePos;
            }

            for (mergePos = 0; mergePos < mergedSize; ++mergePos)
            {
                cachedOperatorList[i + mergePos] = mergedOps[mergePos];
            }
        }

        private void MergeSort<T>(int lp, int rp, Func<T, T, bool> comparer, Func<OperatorDef, T> compraree)
        {
            int middle;
            if (lp < rp)
            {
                middle = (lp + rp) / 2;

                MergeSort(lp, middle, comparer, compraree);
                MergeSort(middle + 1, rp, comparer, compraree);

                Merge(lp, middle, rp, comparer, compraree);
            }
        }

        //比较器是左边小于右边ret true是从小到大，反过来是从大到小
        private void SortOperator<T>(Func<T, T, bool> comparer, Func<OperatorDef, T> compraree)
        {
            MergeSort(0, cachedOperatorList.Count() - 1, comparer, compraree);

        }
        #endregion

        static List<OperatorDef> cachedOperatorList = new List<OperatorDef>();
        public Thing RecruitConsole
        {
            get { return RIWindowHandler.recruitConsole; }
        }
        public static int operatorType = -1;
    }
}