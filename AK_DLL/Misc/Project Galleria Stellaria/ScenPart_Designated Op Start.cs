using AK_DLL.UI;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace AK_DLL
{
    public class ScenPart_Designated_Op_Start : ScenPart
    {
        private int operatorClass;

        private OperatorDef operatorDef;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref operatorClass, "opClass");
            Scribe_Defs.Look(ref operatorDef, "opDef");
        }

        //ui
        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
            Rect rect = new(scenPartRect.x, scenPartRect.y, scenPartRect.width, scenPartRect.height / 2f);
            if (operatorDef == null)
            {
                Widgets.ButtonText(rect, "No operator exist");
                return;
            }
            //IMGUI，选择干员职业
            if (Widgets.ButtonText(rect, RIWindowHandler.operatorClasses[operatorClass].label.Translate()))
            {
                List<FloatMenuOption> list = new();
                foreach (OperatorClassDef i in RIWindowHandler.operatorClasses.Values)
                {
                    OperatorClassDef localClass = i;
                    list.Add(new FloatMenuOption(i.label.Translate(), delegate
                    {
                        operatorClass = localClass.sortingOrder;
                        operatorDef = RIWindowHandler.operatorDefs[operatorClass].Values.RandomElement();
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }

            //选择干员def
            rect.y += scenPartRect.height / 2f;
            if (Widgets.ButtonText(rect, operatorDef.nickname.Translate()))
            {
                List<FloatMenuOption> list = new();
                foreach (OperatorDef i in RIWindowHandler.operatorDefs[operatorClass].Values)
                {
                    //OperatorDef localOp = i;
                    list.Add(new FloatMenuOption(i.nickname.Translate(), delegate
                    {
                        operatorDef = i;
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }

        }
        public static string PlayerStartWithIntro => "ScenPart_StartWith".Translate();

        public override string Summary(Scenario scen)
        {
            return ScenSummaryList.SummaryWithList(scen, "PlayerStartsWith", PlayerStartWithIntro);
        }

        public override void Randomize()
        {
            if (RIWindowHandler.operatorClasses == null || RIWindowHandler.operatorClasses.Keys.Count == 0) return;

            operatorClass = RIWindowHandler.operatorClasses.Keys.RandomElement();

            operatorDef = RIWindowHandler.operatorDefs[operatorClass].Values.RandomElement();
        }

        //游戏开始后，删掉所有原生pawn
        public override void PostGameStart()
        {
            List<Pawn> colonists = new(PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists /*PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists*/);
            //Log.Message($"pgs {colonists.Count}");
            operatorDef.Recruit(Find.CurrentMap);
            foreach (Pawn p in colonists)
            {
                p.Destroy();
            }
        }
    }
}
