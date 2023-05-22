using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

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
        
        public override void DoEditInterface(Listing_ScenEdit listing)
        {
            Rect scenPartRect = listing.GetScenPartRect(this, ScenPart.RowHeight * 2f);
            Rect rect = new Rect(scenPartRect.x, scenPartRect.y, scenPartRect.width, scenPartRect.height / 2f);
            if (operatorDef == null)
            {
                Widgets.ButtonText(rect, "No op exist");
                return;
            }
            if (Widgets.ButtonText(rect, RIWindowHandler.operatorClasses[operatorClass].label.Translate()))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach (OperatorClassDef i in RIWindowHandler.operatorClasses.Values)
                {
                    OperatorClassDef localClass = i;
                    list.Add(new FloatMenuOption(i.label.Translate(), delegate ()
                    {
                        operatorClass = localClass.sortingOrder;
                        operatorDef = RIWindowHandler.operatorDefs[operatorClass].Values.RandomElement();
                    }));
                }
                Find.WindowStack.Add(new FloatMenu(list));
            }

            rect.y += scenPartRect.height / 2f;
            if (Widgets.ButtonText(rect, operatorDef.nickname.Translate()))
            {
                List<FloatMenuOption> list = new List<FloatMenuOption>();
                foreach(OperatorDef i in RIWindowHandler.operatorDefs[operatorClass].Values)
                {
                    OperatorDef localOp = i;
                    list.Add(new FloatMenuOption(i.nickname.Translate(), delegate ()
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

        public override void PostMapGenerate(Map map)
        {
            List<Pawn> colonists = new List<Pawn>(PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists);
            Log.Message($"postmap {colonists.Count}");
        }

        public override IEnumerable<Thing> PlayerStartingThings()
        {
            List<Pawn> colonists = new List<Pawn>(PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists);
            Log.Message($"pst {colonists.Count}");
            return base.PlayerStartingThings();
        }

        public override void PostGameStart()
        {
            List<Pawn> colonists = new List<Pawn>(PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists);
            Log.Message($"pgs {colonists.Count}");
            operatorDef.Recruit(Find.CurrentMap);
            foreach (Pawn p in colonists)
            {
                p.Destroy();
            }
        }
    }
}
