using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    //RI: Rhodes Island 也许会叫罗德岛通用信息终端啥的;不是 riw window的意思。
    //调用之前记得关闭现有的window
    //因为做完主页面嫌弃widgets难用转用UGUI。要是你是后面接手的人，不是完全懂建议你不要大面积改UGUI，我做起来发现全是坑。 芙露蕾蒂留。
    public static class RIWindowHandler
    {
        public static RIWindowType window = RIWindowType.MainMenu;
        public static Thing recruitConsole;
        public static OperatorDef def;
        public static RIWindow actualRIWindow;

        //<干员职业数字序， <干员ID, 干员Def> >
        public static Dictionary<int, Dictionary<string, OperatorDef>> operatorDefs = new Dictionary<int, Dictionary<string, OperatorDef>>();

        //<唯一数字序， 干员职业Def>
        public static Dictionary<int, OperatorClassDef> operatorClasses = new Dictionary<int, OperatorClassDef>();


#region 方舟信息窗口
        public static void OpenRIWindow()
        {
            if (AK_Tool.FSAsset == null)
            {
                Log.Error("MIS. Critical error: FSAsset is invalid");
                return;
            }
            switch (window)
            {
                case RIWindowType.MainMenu:
                    RIWindow_OperatorDetail.isRecruit = true;
                    RIWindow_MainMenu window_MainMenu = new RIWindow_MainMenu(new DiaNode(new TaggedString()), true);
                    Find.WindowStack.Add(window_MainMenu);
                    break;
                case RIWindowType.Op_Series:
                    //break;
                case RIWindowType.Op_Gacha:
                    //break;
                case RIWindowType.Op_List:
                    actualRIWindow = new RIWindow_OperatorList();
                    actualRIWindow.DrawUI("Operator List");
                    /*GameObject EVSystem = AK_Tool.FSAsset.LoadAsset<GameObject>("EventSystem");
                    GameObject EVSystemInstance = GameObject.Instantiate(EVSystem);
                    EVSystemInstance.SetActive(true);*/

                    /*uiInstance.transform.position = new Vector3(0, 0, 0);
                    uiInstance.transform.rotation = Quaternion.identity;
                    uiInstance.transform.localScale = Vector3.one;

                    // 将UI实例设置为可见状态
                    uiInstance.SetActive(true);
                    AK_Tool.EVSystemInstance.SetActive(true);*/



                    break;
                    RIWindow_OperatorList windowOpList = new RIWindow_OperatorList();
                    break;
                case RIWindowType.Op_Detail:
                    RIWindow_OperatorDetail windowOpDetail = new RIWindow_OperatorDetail(new DiaNode(new TaggedString(def.nickname)), true);
                    windowOpDetail.soundAmbient = SoundDefOf.RadioComms_Ambience;
                    Find.WindowStack.Add(windowOpDetail);
                    break;
                default:
                    Log.ErrorOnce("MIS. Invaild RIWindow Type", 1);
                    break;
            }
        }

        public static void OpenRIWindow(RIWindowType windowType)
        {
            window = windowType;
            OpenRIWindow();
        }

        public static void OpenRIWindow(RIWindowType windowType, Thing console)
        {
            recruitConsole = console;
            OpenRIWindow(windowType);
        }

        public static void OpenRIWindow_OpDetail(OperatorDef operatorDef)
        {
            window = RIWindowType.Op_Detail;
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
