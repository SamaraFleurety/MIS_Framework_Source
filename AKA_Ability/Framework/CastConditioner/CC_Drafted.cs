namespace AKA_Ability.CastConditioner
{
    public class CC_Drafted : CastConditioner_Base
    {
        public override bool Castable(AKAbility_Base instance)
        {
            return instance.CasterPawn.Drafted;
        }
    }
}
