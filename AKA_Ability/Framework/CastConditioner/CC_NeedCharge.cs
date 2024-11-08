namespace AKA_Ability.CastConditioner
{
    public class CC_NeedCharge : CastConditioner_Base
    {
        public int chargeRequire = 1;

        public CC_NeedCharge()
        {
            failReason = "AKA_ChargeIsZero";
        }

        public override bool Castable(AKAbility instance)
        {
            return instance.cooldown.charge >= chargeRequire;
        }
    }
}