using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AK_DLL
{
    //RI: Rhodes Island 也许会叫罗德岛通用信息终端啥的;不是 riw window的意思。
    //调用之前记得关闭现有的window
    public static class RIWindowHandler
    {
        public static RIWindow window = RIWindow.MainMenu;
        public static Thing recruitConsole;
        public static OperatorDef def;

        //<干员职业数字序， <干员ID, 干员Def> >
        public static Dictionary<int, Dictionary<string, OperatorDef>> operatorDefs = new Dictionary<int, Dictionary<string, OperatorDef>>();

        //<唯一数字序， 干员职业Def>
        public static Dictionary<int, OperatorClassDef> operatorClasses = new Dictionary<int, OperatorClassDef>();

#region 方舟信息窗口
        public static void OpenRIWindow()
        {
            switch (window)
            {
                case RIWindow.MainMenu:
                    //break;
                case RIWindow.Series:
                    //break;
                case RIWindow.Gacha:
                    //break;
                case RIWindow.OpList:
                    RIWindow_OperatorList windowOpList = new RIWindow_OperatorList(new DiaNode(new TaggedString()), true);
                    windowOpList.soundAmbient = SoundDefOf.RadioComms_Ambience; //记得换好听的bgm
                    Find.WindowStack.Add(windowOpList);
                    break;
                case RIWindow.OpDetail:
                    RIWindow_OperatorDetail windowOpDetail = new RIWindow_OperatorDetail(new DiaNode(new TaggedString(def.nickname)), true);
                    windowOpDetail.soundAmbient = SoundDefOf.RadioComms_Ambience;
                    Find.WindowStack.Add(windowOpDetail);
                    break;
                default:
                    Log.ErrorOnce("MIS. Invaild RIWindow Type", 1);
                    break;
            }
        }

        public static void OpenRIWindow(RIWindow windowType)
        {
            window = windowType;
            OpenRIWindow();
        }

        public static void OpenRIWindow(RIWindow windowType, Thing console)
        {
            recruitConsole = console;
            OpenRIWindow(windowType);
        }

        public static void OpenRIWindow_OpDetail(OperatorDef operatorDef)
        {
            window = RIWindow.OpDetail;
            def = operatorDef;
            OpenRIWindow();
        }
#endregion

        public static void AutoFillOperators()
        {
            foreach (int i in operatorClasses.Keys)
            {
                operatorDefs[i] = new Dictionary<string, OperatorDef>();
            }
            foreach (OperatorDef i in DefDatabase<OperatorDef>.AllDefs)
            {
                i.AutoFill();
                try
                {
                    operatorDefs[i.operatorType.sortingOrder].Add(AK_Tool.GetOperatorIDFrom(i.defName), i);
                }
                catch (Exception)
                {
                    Log.Error("MIS. 没加起" + i.nickname);
                }
            }
        }
        public static void LoadOperatorClasses()
        {
            foreach (OperatorClassDef i in DefDatabase<OperatorClassDef>.AllDefs)
            {
                int tempOrder = 10000001;
                if (i.sortingOrder >= 10000000)
                {
                    Log.Error(i.label.Translate() + "'s sorting order must lower than 10000000");
                }
                else if (operatorClasses.ContainsKey(i.sortingOrder))
                {
                    Log.Error(i.label.Translate() + "has duplicate loading order with" + operatorClasses[i.sortingOrder]);
                    i.sortingOrder = tempOrder;
                    operatorClasses.Add(tempOrder, i);
                    tempOrder++;
                }
                else
                {
                    operatorClasses.Add(i.sortingOrder, i);
                }
            }
            if (operatorClasses.Count >= 1)
            {
                RIWindow_OperatorList.operatorType = operatorClasses.First().Key;
            }
        }
    }
}
