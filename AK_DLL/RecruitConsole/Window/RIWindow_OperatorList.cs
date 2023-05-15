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

//记得给干员页面写下拉 series
//sortOperator可能封装一下更好 特别是按字母排序
//有没有可能按钮效果不用委托而是封装好一点呢
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
#region 属性，字段，快捷属性
        private static int sortType = (int)OperatorSortType.Alphabet;
        private static bool sortReverseOrder = false;
        //排序按钮的面板（通用父类）
        private static Transform sorterColumnLoc = null;
        //干员面板
        private static Transform opListPanel = null;
        private static List<GameObject> opList = new List<GameObject>();
        //相当投机取巧地在名字里面存储数据 字符串内可以随便填。现在的值是12。
        public static int orderInName = "FSUI_whatev_".Length;

        private GameObject ClickedBtn
        {
            get
            {
                return EventSystem.current.currentSelectedGameObject;
            }
        }

        private GameObject ClickedBtnParent
        {
            get
            {
                return ClickedBtn.transform.parent.gameObject;
            }
        }
        //完了 好像创建临时变量就可以 不需要这么麻烦的方法
        private int btnOrder(GameObject clickedBtn)
        {
            return int.Parse(clickedBtn.name.Substring(orderInName));
        }

#endregion

        public override void DoContent()
        {
            DrawSortBtns();
            DrawNavBtn();
            DrawOperatorList();
            DrawClassBtn();
        }

        private void DrawNavBtn()
        {
            GameObject navBtn;
            //左上角返回按钮 虽然现在返回就是主界面，但就是有这个按钮
            navBtn = GameObject.Find("btnBack").transform.GetChild(0).gameObject;
            navBtn.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                RIWindow_OperatorDetail.isRecruit = true;
                this.Close(false);
                RIWindowHandler.OpenRIWindow(RIWindowType.MainMenu);
            });
            //主界面按钮
            navBtn = GameObject.Find("btnHome").transform.GetChild(0).gameObject;
            navBtn.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                RIWindow_OperatorDetail.isRecruit = true;
                this.Close(false);
                RIWindowHandler.OpenRIWindow(RIWindowType.MainMenu);
            });
            //退出按钮
            navBtn = GameObject.Find("btnEscape").transform.GetChild(0).gameObject;
            navBtn.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                RIWindow_OperatorDetail.isRecruit = true;
                this.Close();
            });
        }
        
        /// <summary>
        /// 致敬经典 - 废话summary
        /// Just Draw classes buttons
        /// </summary>
        private void DrawClassBtn()
        {
            GameObject classBtnPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("btnClassTemplate");
            Transform classColumn = GameObject.Find("btnClassColumn").transform;
            GameObject classBtnInstance;
            int i = 0;
            foreach(KeyValuePair<int, OperatorClassDef> node in RIWindowHandler.operatorClasses)
            {
                classBtnInstance = GameObject.Instantiate(classBtnPrefab, classColumn);
                //位置
                Vector3 pos = classBtnInstance.transform.localPosition;
                classBtnInstance.transform.localPosition = new Vector3(pos.x, pos.y * i);
                i++;
                //名字
                classBtnInstance.name = "FSUI_classs_" + node.Key;
                //按钮 非实时
                classBtnInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                {
                    operatorClass = btnOrder(ClickedBtnParent);
                    cachedOperatorList = RIWindowHandler.operatorDefs[operatorClass].Values.ToList();
                    //默认使用 首字母排序
                    NeedSortTo((int)OperatorSortType.Alphabet, true);
                    
                    //实际排序
                    SortOperator<string>(delegate (string a, string b)
                    {
                        return string.Compare(a, b) <= 0;
                    }, delegate (OperatorDef def)
                    {
                        return AK_Tool.GetOperatorIDFrom(def.defName);
                    });
                    DrawOperatorListContent();
                });
                //没有图片 就显示字
                if (node.Value.Icon == null)
                {
                    TextMeshProUGUI TMP = classBtnInstance.GetComponentInChildren<TextMeshProUGUI>();
                    TMP.gameObject.SetActive(true);
                    TMP.text = node.Value.label.Translate();
                }
                else
                {
                    Texture2D classImage = node.Value.Icon;
                    classBtnInstance.GetComponent<Image>().sprite = Sprite.Create(classImage, new Rect(0, 0, classImage.width, classImage.height), new Vector2(0.5f, 0.5f));
                }
                classBtnInstance.SetActive(true);
            }
        }

#region 绘制干员列表(不含排序逻辑)
        //绘制干员列表的初期准备
        public void DrawOperatorList()
        {
            if (operatorClass == -1)
            {
                Log.Message("MIS. Critical error: No class loaded");
            }
            cachedOperatorList = RIWindowHandler.operatorDefs[operatorClass].Values.ToList();
            NeedSortTo((int)OperatorSortType.Alphabet, true);
            SortOperator<string>(delegate (string a, string b)
            {
                return string.Compare(a, b) <= 0;
            }, delegate (OperatorDef def)
            {
                return AK_Tool.GetOperatorIDFrom(def.defName);
            });
            opListPanel = GameObject.Find("OpListPanel").transform;
            
            DrawOperatorListContent();
        }
        //实际绘制所有干员列表 会在排序等情况被重复调用
        private void DrawOperatorListContent()
        {
            int cnt = cachedOperatorList.Count;
            if (!sortReverseOrder)
            {
                for (int i = 0; i < cnt; ++i)
                {
                    DrawOperatorPortrait(i, cachedOperatorList[i]);
                }
            }
            else
            {
                for (int i = cnt - 1, j = 0; i >= 0; --i, ++j)
                {
                    DrawOperatorPortrait(i, cachedOperatorList[i], j);
                }
            }
            //如果小于3*8个干员 就不显示右边的没用滑动条
            if (cnt <= 24)
            {
                GameObject.Find("OpReg_Scrollbar").SetActive(false);
            }

            while (cnt < opList.Count)
            {
                if (opList[cnt] != null) opList[cnt].SetActive(false);
                else break;
                ++cnt;
            }
            //干员绘制
        }

        //绘制单个干员, i是此干员在cachedList中的顺序,j是在此ui中实际显示顺序
        private void DrawOperatorPortrait(int i, OperatorDef def, int j = -1)
        {
            GameObject opPortraitInstance;
            if (j == -1) j = i;
            //上面有检查，估计已经多余了
            if (j >= cachedOperatorList.Count)
            {
                Log.Error("MIS. DrawOperatorPortrait out of range");
                return;
            }
            Texture2D operatorTex = ContentFinder<Texture2D>.Get(def.headPortrait);
            if (j >= opList.Count || opList[j] == null)
            {
                GameObject opRectPrefab = AK_Tool.FSAsset.LoadAsset<GameObject>("OperatorTemplate");
                opPortraitInstance = GameObject.Instantiate(opRectPrefab, opListPanel);
                //opPortraitInstance.transform.parent = opListLoc;
                if (j >= opList.Count) opList.Add(opPortraitInstance);
                else opList[j] = opPortraitInstance;

                opPortraitInstance.GetComponentInChildren<Button>().onClick.AddListener(delegate ()
                {
                    RIWindowHandler.OpenRIWindow_OpDetail(cachedOperatorList[btnOrder(ClickedBtnParent)]);
                    this.Close(false);
                });
                //决定头像框的位置。目前一行8个，共3行。  已经改用unity的grid。
                //opPortraitInstance.transform.localPosition = new Vector3((j / 8 * -8 + j) * opPortraitInstance.transform.localPosition.x, (j / 8) * opPortraitInstance.transform.localPosition.y);
            }
            else
            {
                opPortraitInstance = opList[j];
            }
            //设定名字中 以后检索干员def时用的数字序
            opPortraitInstance.name = "FSUI_Opertr_" + i;
            //设定头像 决定干员头像的子物体是第一个子物体。
            opPortraitInstance.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(operatorTex, new Rect(0, 0, operatorTex.width, operatorTex.height), new Vector2(0, 0));
            //干员名字
            opPortraitInstance.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = def.nickname.Translate();
            //排序信息
            opPortraitInstance.transform.GetChild(1).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = GetSortDetail(def);
            opPortraitInstance.SetActive(true);
        }
#endregion

#region 排序相关
        //按钮的功能有一定的优化空间，但我懒得做
        private void DrawSortBtns()
        {
            GameObject sortBtnPrefab = bundle.LoadAsset<GameObject>("btnSortTemplate");
            GameObject sortBtnInstance;
            TextMeshProUGUI textTMP;
            if (sorterColumnLoc == null)
            {
                sorterColumnLoc = GameObject.Find("sorterBg").transform;
            }
            sorterBtns.Clear();

            //按某种技能的等级排序
            for (int i = 0; i < TypeDef.SortOrderSkill.Count(); ++i)
            {
                sortBtnInstance = GameObject.Instantiate(sortBtnPrefab, sorterColumnLoc);
                textTMP = sortBtnInstance.GetComponentInChildren<TextMeshProUGUI>();
                //按钮的显示文字
                textTMP.text = TypeDef.SortOrderSkill[i].label.Translate();
                //使用了投机取巧而不是正常的方式存储数据。
                sortBtnInstance.name = "FSUI_Sorter_" + i;
                int k = i;
                sortBtnInstance.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    Log.Message($"{i}:{k}:{btnOrder(ClickedBtn)}");
                    sortBtnInstance = ClickedBtn;
                    if (NeedSortTo(btnOrder(ClickedBtn)))
                    {
                        SortOperator<int>(delegate (int a, int b)
                        {
                            return !(a <= b);
                        }, delegate (OperatorDef def)
                        {
                            return def.SortedSkills[btnOrder(ClickedBtn)].level;
                        });
                    }
                    DrawOperatorListContent();
                });
                sortBtnInstance.SetActive(true);

                sortBtnInstance.transform.localPosition = new Vector3(sortBtnInstance.transform.localPosition.x * (i - (i / 7 * 7)), sortBtnInstance.transform.localPosition.y * (i / 7));

                sorterBtns.Add(sortBtnInstance.transform);
            }

            //按面板DPS排序
            sortBtnInstance = GameObject.Instantiate(sortBtnPrefab, sorterColumnLoc);
            textTMP = sortBtnInstance.GetComponentInChildren<TextMeshProUGUI>();
            //按钮的显示文字
            textTMP.text = "AK_SortDPS".Translate();

            int temp = (int)OperatorSortType.Dps;
            sortBtnInstance.name = "FSUI_Sorter_" + temp;

            sortBtnInstance.GetComponent<Button>().onClick.AddListener(delegate ()
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
                DrawOperatorListContent();
            });
            sortBtnInstance.SetActive(true);

            sortBtnInstance.transform.localPosition = new Vector3(sortBtnInstance.transform.localPosition.x * (temp - (temp / 7 * 7)), sortBtnInstance.transform.localPosition.y * (temp / 7));

            sorterBtns.Add(sortBtnInstance.transform);

            //按字母表排序,a->z
            sortBtnInstance = GameObject.Instantiate(sortBtnPrefab, sorterColumnLoc);
            textTMP = sortBtnInstance.GetComponentInChildren<TextMeshProUGUI>();
            //按钮的显示文字
            textTMP.text = "AK_SortAlphabet".Translate();

            temp = (int)OperatorSortType.Alphabet;
            sortBtnInstance.name = "FSUI_Sorter_" + temp;

            sortBtnInstance.GetComponent<Button>().onClick.AddListener(delegate ()
            {
                if (NeedSortTo((int)OperatorSortType.Alphabet))
                {
                    SortOperator<string>(delegate (string a, string b)
                    {
                        return string.Compare(a, b) <= 0;
                    }, delegate (OperatorDef def)
                    {
                        return AK_Tool.GetOperatorIDFrom(def.defName);
                    });
                }
                DrawOperatorListContent();
            });
            sortBtnInstance.SetActive(true);

            sortBtnInstance.transform.localPosition = new Vector3(sortBtnInstance.transform.localPosition.x * (temp - (temp / 7 * 7)), sortBtnInstance.transform.localPosition.y * (temp / 7));

            sorterBtns.Add(sortBtnInstance.transform);
        }

        //在干员名字下面 显示排序信息，比如："驯兽：12"
        private string GetSortDetail(OperatorDef def)
        {
            string detail = "";
            if (sortType < TypeDef.SortOrderSkill.Count())
            {
                detail = TypeDef.SortOrderSkill[sortType].label.Translate() + ": " + def.SortedSkills[sortType].level;
            }
            else if (sortType == (int)OperatorSortType.Dps)
            {
                detail = "AK_SortDPS".Translate() + ": " + DPSCalculator(def).ToString("G4");
            }
            else if (sortType == (int)OperatorSortType.Alphabet && LanguageDatabase.activeLanguage != LanguageDatabase.defaultLanguage)
            {
                detail = AK_Tool.GetOperatorIDFrom(def.defName);
            }
            return detail;
        }

        //好像因为之前写了一半就跑了 反正现在有bug
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
                //Log.Message($"p3 : {w.defName}");
                //命中率100%远程dps
                if (w.Verbs != null && w.Verbs.Count > 0)
                {
                    //Log.Message(w.Verbs[0].ToString());
                    //Log.Message(w.Verbs[0].verbClass.ToString());
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
        private bool NeedSortTo(int newSortType, bool overrd = false)
        {
            Vector3 tempV3;
            if (sortType == newSortType && !overrd)
            {
                tempV3 = sorterBtns[sortType].GetChild(3).eulerAngles;
                sorterBtns[sortType].GetChild(3).eulerAngles = new Vector3(tempV3.x, tempV3.y, (tempV3.z + 180) % 360);
                sortReverseOrder = !sortReverseOrder;
                return false;
            }
            else
            {
                //先把之前的按钮改回去,3是被选中的白标,2是没选中的灰标
                sorterBtns[sortType].GetChild(3).gameObject.SetActive(false);
                sorterBtns[sortType].GetChild(2).gameObject.SetActive(true);
                
                sortReverseOrder = false;
                sortType = newSortType;
                //更改新按钮的标记
                sorterBtns[sortType].GetChild(3).gameObject.SetActive(true);
                sorterBtns[sortType].GetChild(2).gameObject.SetActive(false);
                //因为其他按钮默认从大往小排，但字母序是小往大
                if (sortType == (int)OperatorSortType.Alphabet) sorterBtns[sortType].GetChild(3).eulerAngles = new Vector3(0, 0, 180);
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

        private void SortOperator(int sortType)
        {
            switch(sortType)
            {
                case 1:
                    break;
                default:
                    break;
            }
        }
        #endregion

        static List<Transform> sorterBtns = new List<Transform>();

        static List<OperatorDef> cachedOperatorList = new List<OperatorDef>();
        public Thing RecruitConsole
        {
            get { return RIWindowHandler.recruitConsole; }
        }
        public static int operatorClass = -1;
    }
}