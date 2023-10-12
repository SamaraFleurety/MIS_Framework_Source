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
    public abstract class AKAbility : IExposable
    {
        public AKAbility_Container container;

        public CDandCharge CoolDown;

        public OperatorAbilityDef def;

        public AKAbility()
        {

        }
        public AKAbility(OperatorAbilityDef def)
        {
            this.def = def;
        }

        protected OperatorDocument Doc => container.doc;

        protected Pawn CasterPawn => Doc.pawn;

        public virtual void Tick ()
        {
            if (CoolDown.charge >= CoolDown.maxCharge) return;
            CoolDown.CD -= 1;
            if (CoolDown.CD <= 0)
            {
                CoolDown.charge += 1;
                if (CoolDown.charge < CoolDown.maxCharge) CoolDown.CD = CoolDown.maxCD;
            }
        }

        public virtual Gizmo GetGizmo ()
        {
            return null;
        }

        public virtual void ExposeData()
        {
            throw new NotImplementedException();
        }

        protected virtual void UseOneCharge()
        {
            if (CoolDown.charge == CoolDown.maxCharge) CoolDown.CD = CoolDown.maxCD;
            CoolDown.charge -= 1;
        }
    }
}
