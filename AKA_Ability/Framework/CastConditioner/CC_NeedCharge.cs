namespace AKA_Ability.CastConditioner
{
    public class CC_NeedCharge : CastConditioner_Base
    {
        public int chargeRequire = 1;

        public CC_NeedCharge()
        {
            failReason = "AKA_ChargeIsZero";
        }

        public override bool Castable(AKAbility_Base instance)
        {
            return instance.cooldown.Charge >= chargeRequire;
        }
    }
}