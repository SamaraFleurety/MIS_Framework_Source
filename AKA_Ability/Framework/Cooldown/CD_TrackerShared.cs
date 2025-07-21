using AKA_Ability.SharedData;

namespace AKA_Ability.Cooldown
{
    //实现多个技能共享cd
    //此CD是虚拟CD，任何参数皆无意义，对此CD的使用会跳转到主cd
    public class CD_TrackerShared : Cooldown_Regen
    {
        SD_SharedCharge SharedChargeData => parent.container.TryGetSharedData<SD_SharedCharge>();

        public override int SP
        {
            get => SharedChargeData.cooldown.SP;
            set
            {
                SharedChargeData.cooldown.SP = value;
            }
        }
        public override int Charge { get => SharedChargeData.cooldown.Charge; set => SharedChargeData.cooldown.Charge = value; }
        public override int MaxCharge => SharedChargeData.cooldown.MaxCharge;
        public CD_TrackerShared(CooldownProperty property, AKAbility_Base ability) : base()
        {
            parent = ability;
            prop = property;
            charge = 0;
            Tick(0);
        }

        public override void Tick(int amt)
        {
            return;
        }

        public override void ExposeData()
        {
            //BackCompatibility.PostExposeData(this);
        }
    }
}
