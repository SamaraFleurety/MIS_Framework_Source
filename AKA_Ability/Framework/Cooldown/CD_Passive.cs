using Verse;

namespace AKA_Ability.Cooldown
{
    //不会自动变动的 被动CD
    public class CD_Passive : Cooldown_Regen
    {
        public int maxSP;

        public override int MaxSP => maxSP;

        public CD_Passive(CooldownProperty property, AKAbility_Base ability) : base(property, ability)
        {
            maxSP = prop.SPPerCharge * (int)prop.SPUnit;
        }

        public override string GetExplanation()
        {
            return string.Empty;
        }

        public override void Tick(int amt)
        {
        }
    }
}
