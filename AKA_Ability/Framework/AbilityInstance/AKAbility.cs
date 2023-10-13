using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace AKA_Ability
{
    public abstract class AKAbility : IExposable
    {
        public AKAbility_Tracker container;

        public CDandCharge cooldown;

        public OpAbilityDef def;

        protected Gizmo cachedGizmo = null;
        public AKAbility()
        {

        }
        public AKAbility(OpAbilityDef def)
        {
            this.def = def;
        }

        protected Pawn CasterPawn => container.owner;

        public virtual void Tick()
        {
            if (cooldown.charge >= cooldown.maxCharge) return;
            cooldown.CD -= 1;
            if (cooldown.CD <= 0)
            {
                cooldown.charge += 1;
                if (cooldown.charge < cooldown.maxCharge) cooldown.CD = cooldown.maxCD;
            }
        }

        public virtual Gizmo GetGizmo()
        {
            if (cachedGizmo == null) InitializeGizmo();
            UpdateGizmo();
            return cachedGizmo;
        }

        protected virtual void UpdateGizmo()
        {
            cachedGizmo.disabled = false;
            if (cooldown.charge == 0 && def.targetMode != TargetMode.AutoEnemy)
            {
                cachedGizmo.Disable("AK_ChargeIsZero".Translate());
            }
        }

        protected abstract void InitializeGizmo();

        public virtual void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            Scribe_Deep.Look(ref cooldown, "CD");
        }

        protected virtual void UseOneCharge()
        {
            if (cooldown.charge == cooldown.maxCharge) cooldown.CD = cooldown.maxCD;
            cooldown.charge -= 1;
            container.PostPlayAbilitySound(this);
        }
    }
}
