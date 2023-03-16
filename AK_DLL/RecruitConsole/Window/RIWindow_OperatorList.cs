using System;
using Verse;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using System.Collections.Generic;
using static AK_DLL.AK_Tool;

//记得给skillandfire写排序，在调用之前
namespace AK_DLL
{
    [HotSwappable]
    public class RIWindow_OperatorList : Dialog_NodeTree
    {
        private static readonly int maxX = 1920, maxY = 1080;
        private static readonly int btnHeight = 20;
        private static readonly int btnWidth = 80;
        private static readonly int xMargin = 10;
        private static readonly int yMargin = 10;
        private static readonly int classSideLength = 70;
        private static readonly int classMargin;
        private static int sortType;
        private static bool sortReverseOrder = false;
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
            Rect rect_Back = new Rect(inRect.xMax - xMargin - classSideLength, inRect.y + yMargin, classSideLength, classSideLength / 2);
            //返回上一级按钮
            if (Widgets.ButtonText(rect_Back, "AK_Back".Translate()) || KeyBindingDefOf.Cancel.KeyDownEvent)
            {
                this.Close();
                //RIWindowHandler.OpenRIWindow(RIWindow.Series);
            }
            //返回主界面按钮
            rect_Back.y += classSideLength / 2 + yMargin;
            if (Widgets.ButtonText(rect_Back, "AK_MainMenu".Translate()))
            {
                this.Close();
                RIWindowHandler.OpenRIWindow(RIWindow.MainMenu);
            }
            if (operatorType == -1)
            {
                Log.WarningOnce("MIS.No operator classes found.", 1);
                return;
            }
            //绘制选干员的tab
            Rect btnRect = new Rect(crntX, crntY, btnWidth, btnHeight);
            Rect classBtn = new Rect(inRect.xMax - xMargin - classSideLength, yMargin * 2 + 100, classSideLength, classSideLength);
            foreach (KeyValuePair<int, OperatorClassDef> i in operatorClasses)
            {
                /*if (Widgets.ButtonText(new Rect(inRect.x + xOffset, inRect.y + 15f, 80f, 20f), i.Value, true, true, operatorType != i.Key)) operatorType = i.Key;
                xOffset += 100;*/
                if (Widgets.ButtonText(classBtn, i.Value.label.Translate(), true, true, operatorType != i.Key))
                {
                    sortType = -1;
                    operatorType = i.Key;
                    cachedOperatorList = operatorDefs[operatorType].Values.ToList();
                }
                classBtn.y += btnWidth + 25;
            }
            btnRect.x = crntX;
            btnRect.y += btnHeight + yMargin * 2 + classSideLength / 2;
            btnRect.size = inRect.size;
            GUI.DrawTexture(new Rect(btnRect.position, new Vector2(inRect.width - 2 * xMargin, 2f)), BaseContent.WhiteTex);
            btnRect.y += yMargin;
            DrawSortBtns(inRect);
            DrawOperatorList(btnRect);
            //统一绘制干员            
            //干员列表已经改放在ModSettings

        }
        
        public void DrawOperatorList(Rect inRect)
        {
            Rect rect_operatorFrame = new Rect(inRect.position, new Vector2(100f, 100f));
            Text.Anchor = TextAnchor.UpperCenter;
            if (!sortReverseOrder)
            {
                foreach (OperatorDef operator_Def in cachedOperatorList)
                {
                    DrawOperator(ref rect_operatorFrame, inRect, operator_Def);
                }
            }
            else
            {
                for (int i = cachedOperatorList.Count - 1; i >= 0; --i)
                {
                    DrawOperator(ref rect_operatorFrame, inRect, cachedOperatorList[i]);
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
            //干员绘制
        }

        private void DrawOperator(ref Rect rect_operatorFrame, Rect inRect, OperatorDef operator_Def)
        {
            Texture2D operatorTex = ContentFinder<Texture2D>.Get(operator_Def.headPortrait);
            Widgets.LabelFit(new Rect(rect_operatorFrame.x + 20f, rect_operatorFrame.y + 110f, 100f, 50f), operator_Def.nickname);
            Widgets.DrawTextureFitted(new Rect(rect_operatorFrame.x, rect_operatorFrame.y, rect_operatorFrame.width + 2f, rect_operatorFrame.height + 2f), ContentFinder<Texture2D>.Get("UI/Frame/Frame_HeadPortrait"), 1f);
            if (Widgets.ButtonImage(new Rect(rect_operatorFrame.x + 3f, rect_operatorFrame.y + 5f, 97f, 95f), operatorTex))
            {
                this.Close();
                RIWindowHandler.OpenRIWindow_OpDetail(operator_Def);
            }
            rect_operatorFrame.x += 140f;
            if (rect_operatorFrame.x > 1700f)
            {
                rect_operatorFrame.x = inRect.x;
                rect_operatorFrame.y += 150f;
            }
        }


#region 排序相关
        private void DrawSortBtns(Rect inRect)
        {
            Rect sortBtn = new Rect(inRect.xMin, inRect.y + yMargin, classSideLength, classSideLength / 2);
            //按某种技能的等级排序
            for (int i = 0; i < TypeDef.SortOrderSkill.Count(); ++i)
            {
                if (Widgets.ButtonText(sortBtn, TypeDef.SortOrderSkill[i].label.Translate()))
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
                sortBtn.x += classSideLength + xMargin;
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
            }
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