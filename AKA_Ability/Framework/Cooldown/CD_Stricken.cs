using Verse;

namespace AKA_Ability.Cooldown
{
    public class CD_Stricken : Cooldown_Regen
    {
        public CD_Stricken(CooldownProperty property, AKAbility ability) : base(property, ability)
        {
        }

        //不衰减
        public override void Tick(int amt)
        {
        }

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            AKA_Utilities.RegisterAsStricken(parent.CasterPawn, parent);
        }

        public override void PostDespawn()
        {
            base.PostDespawn();
            AKA_Utilities.DeregisterAsStricken(parent.CasterPawn, parent);
        }

        public override void Notify_PawnStricken(ref DamageInfo dinfo)
        {
            if (charge >= MaxCharge) return;
            ++SP;
            RefreshChargeAndSP();
        }
    }
}