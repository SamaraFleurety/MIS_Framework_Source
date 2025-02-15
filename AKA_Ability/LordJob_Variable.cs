using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AKA_Ability
{
    public class LordJob_Variable : LordJob
    {
        private DutyDef dutyDef;

        public LocalTargetInfo focus = LocalTargetInfo.Invalid;

        public LocalTargetInfo focusSecond = LocalTargetInfo.Invalid;

        public LocalTargetInfo focusThird = LocalTargetInfo.Invalid;

        public float radius = -1f;

        public override bool AddFleeToil => false;

        public LordJob_Variable()
        {
        }

        public LordJob_Variable(DutyDef dutyDef)
        {
            this.dutyDef = dutyDef;
        }

        public LordJob_Variable(DutyDef dutyDef, LocalTargetInfo focus)
        {
            this.dutyDef = dutyDef;
            this.focus = focus;
        }

        public LordJob_Variable(DutyDef dutyDef, LocalTargetInfo focus, LocalTargetInfo focusSecond, LocalTargetInfo focusThird, float radius = -1f)
        {
            this.dutyDef = dutyDef;
            this.focus = focus;
            this.focusSecond = focusSecond;
            this.focusThird = focusThird;
            this.radius = radius;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            stateGraph.AddToil(new LordToil_Variable(dutyDef, focus, focusSecond, focusThird, radius));
            return stateGraph;
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref dutyDef, "dutyDef");
            Scribe_TargetInfo.Look(ref focus, saveDestroyedThings: true, "focus", LocalTargetInfo.Invalid);
            Scribe_TargetInfo.Look(ref focusSecond, saveDestroyedThings: true, "focusSecond", LocalTargetInfo.Invalid);
            Scribe_TargetInfo.Look(ref focusThird, saveDestroyedThings: true, "focusThird", LocalTargetInfo.Invalid);
            Scribe_Values.Look(ref radius, "radius");
        }
    }
    public class LordToil_Variable : LordToil
    {
        private DutyDef dutyDef;

        public LocalTargetInfo focus = LocalTargetInfo.Invalid;

        public LocalTargetInfo focusSecond = LocalTargetInfo.Invalid;

        public LocalTargetInfo focusThird = LocalTargetInfo.Invalid;

        public float radius = -1f;

        public LordToil_Variable()
        {
        }

        public LordToil_Variable(DutyDef dutyDef)
        {
            this.dutyDef = dutyDef;
        }

        public LordToil_Variable(DutyDef dutyDef, LocalTargetInfo focus)
        {
            this.dutyDef = dutyDef;
            this.focus = focus;
        }

        public LordToil_Variable(DutyDef dutyDef, LocalTargetInfo focus, LocalTargetInfo focusSecond, LocalTargetInfo focusThird, float radius = -1f)
        {
            this.dutyDef = dutyDef;
            this.focus = focus;
            this.focusSecond = focusSecond;
            this.focusThird = focusThird;
            this.radius = radius;
        }

        public override void UpdateAllDuties()
        {
            if (dutyDef == null) return;
            for (int i = 0; i < lord.ownedPawns.Count; i++)
            {
                Pawn pawn = lord.ownedPawns[i];
                if (pawn?.mindState != null)
                {
                    pawn.mindState.duty = new PawnDuty(dutyDef, focus, focusSecond, focusThird, radius);
                }
            }
        }
    }
}
