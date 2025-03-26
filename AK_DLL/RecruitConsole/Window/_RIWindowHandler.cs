using FS_UGUIFramework.UI;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace AK_DLL.UI
{
    //RI: Rhodes Island 也许会叫罗德岛通用信息终端啥的;不是 riw window的意思。
    //调用之前记得关闭现有的window
    //因为做完主页面嫌弃widgets难用转用UGUI。要是你是后面接手的人，不是完全懂建议你不要大面积改UGUI，我做起来发现全是坑。 芙露蕾蒂留。
    public static class RIWindowHandler
    {
        public static RIWindowType window = RIWindowType.MainMenu;
        public static Thing RecruitConsole => UGUIHandler.console;
        public static OperatorDef def;
        public static RIWindow actualRIWindow;

        //连续招募，招募干员之后不是退出UI而是返回花名册
        public static bool continuousRecruit = false;


        //<干员职业数字序， <干员ID, 干员Def> >
        public static Dictionary<int, Dictionary<string, OperatorDef>> operatorDefs = new Dictionary<int, Dictionary<string, OperatorDef>>();

        //<唯一数字序(sortingOrder)， 干员职业Def>
        public static Dictionary<int, OperatorClassDef> operatorClasses = new Dictionary<int, OperatorClassDef>();

        //包含所有系列 这个想不到有什么离散检索的需求。系列内有写包含职业。
        public static List<OperatorSeriesDef> operatorSeries = new List<OperatorSeriesDef>();

        #region 方舟信息窗口
        /*public static void OpenRIWindow()
        {
            if (AK_Tool.FSAsset == null)
            {
                Log.Error("MIS. Critical error: FSAsset is invalid");
                return;
            }
            switch (window)
            {
                case RIWindowType.MainMenu:
                    RIWindow_OperatorDetail.windowPurpose = OpDetailType.Recruit;
                    actualRIWindow = new RIWindow_MainMenu();
                    actualRIWindow.DrawUI("yccMainMenu");

                    break;
                case RIWindowType.Op_Series:
                //break;  //可能不再会做，而是整合进opList
                case RIWindowType.Op_Gacha:
                //break;
                case RIWindowType.Op_List:
                    actualRIWindow = new RIWindow_OperatorList();
                    //actualRIWindow.DrawUI("Operator List");
                    actualRIWindow.DrawUI("yccOpList");
                    break;
                case RIWindowType.Op_Detail:
                    actualRIWindow = new RIWindow_OperatorDetail();
                    actualRIWindow.DrawUI("Operator Detail");
                    break;
                default:
                    Log.ErrorOnce("MIS. Invaild RIWindow Type", 1);
                    break;
            }
        }*/


        public static void OpenRIWindow(UIPrefabDef prefab, Thing console = null, Pawn pawn = null, OpDetailType purpose = OpDetailType.Recruit)
        {
            RIWindow_OperatorDetail.windowPurpose = purpose;

            if (console != null)
            {
                if (pawn != null)
                {
                    UGUIHandler.OpenUIWindow(prefab, console, pawn);
                }
                else
                {
                    UGUIHandler.OpenUIWindow(prefab, console);
                }
            }
            else
            {
                UGUIHandler.OpenUIWindow(prefab);
            }
            //OpenRIWindow(windowType);
        }

        //打开干员详情界面时，必须输入干员def
        public static void OpenRIWindow_OpDetail(UIPrefabDef prefab, OperatorDef operatorDef)
        {
            //window = RIWindowType.Op_Detail;
            def = operatorDef;
            //OpenRIWindow();
            UGUIHandler.OpenUIWindow(prefab);
        }
        #endregion

        #region 初始化数据
        //仅开始游戏时会调用一次
        public static void LoadOperatorSeries()
        {
            foreach (OperatorSeriesDef i in DefDatabase<OperatorSeriesDef>.AllDefs)
            {
                operatorSeries.Add(i);
                i.includedClasses = new List<int>();
            }
            if (operatorSeries.Count > 0)
            {
                if (RIWindow_OperatorList.Series < 0)
                {
                    RIWindow_OperatorList.Series = 0;
                }
                else if (RIWindow_OperatorList.Series > 0 && RIWindow_OperatorList.Series >= operatorSeries.Count)       //可能是减少了mod数量
                {
                    RIWindow_OperatorList.Series = 0;
                }
            }
            else RIWindow_OperatorList.Series = -1;
        }
        public static void AutoFillOperators()
        {
            foreach (int i in operatorClasses.Keys)
            {
                operatorDefs[i] = new Dictionary<string, OperatorDef>();
            }
            foreach (OperatorDef i in DefDatabase<OperatorDef>.AllDefs)
            {
                try
                {
                    i.AutoFill();
                }
                catch (Exception ex) 
                {
                    Log.Error("MIS. 自动补全失败:" + i.nickname + $"at { ex.ToString()}\n{ex.StackTrace}");
                }

                try
                {
                    operatorDefs[i.operatorType.sortingOrder].Add(AK_Tool.GetOperatorIDFrom(i.defName), i);
                }
                catch (Exception ex)
                {
                    Log.Error("[MIS] 添加失败: " + i.nickname + $"at {ex.ToString()}\n{ex.StackTrace}");
                }
            }
        }
        public static void LoadOperatorClasses()
        {
            foreach (OperatorClassDef i in DefDatabase<OperatorClassDef>.AllDefs)
            {
                try
                {
                    int tempOrder = 10000001;
                    if (i.sortingOrder >= 10000000)
                    {
                        Log.Error(i.label.Translate() + "'s sorting order must lower than 10000000");
                    }
                    else if (operatorClasses.ContainsKey(i.sortingOrder))
                    {
                        Log.Error(i.label.Translate() + "has duplicate loading order with" + operatorClasses[i.sortingOrder].defName);
                        i.sortingOrder = tempOrder;
                        operatorClasses.Add(tempOrder, i);
                        tempOrder++;
                    }
                    else
                    {
                        operatorClasses.Add(i.sortingOrder, i);
                    }
                    i.series.includedClasses.Add(i.sortingOrder);
                }
                catch
                {
                    Log.Error($"AK. Failed loading class def named {i.defName}");
                }
            }
            if (operatorClasses.Count >= 1)
            {
                if (operatorSeries.NullOrEmpty()) return;   //有职业但没系列 不该出现
                int classIndex = RIWindow_OperatorList.OperatorClass;
                OperatorSeriesDef seriesDef = operatorSeries[RIWindow_OperatorList.Series];
                if (classIndex < 0 || !seriesDef.includedClasses.Contains(classIndex))
                {
                    classIndex = seriesDef.includedClasses.First();
                }

                RIWindow_OperatorList.OperatorClass = classIndex;

                //RIWindow_OperatorList.OperatorClass = operatorClasses.First().Key;
            }
        }
        #endregion
    }
}
