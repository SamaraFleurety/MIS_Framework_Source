using RimWorld;
using RimWorld.Planet;
using Verse;

namespace AKA_Ability.AbilityEffect
{
    //左上角会嘟嘟一下的消息 估计都是给自动释放技能用做提示
    public class AE_SendMessage : AbilityEffectBase
    {
        public string content;
        public MessageTypeDef messageType = MessageTypeDefOf.CautionInput;
        public override bool DoEffect(AKAbility_Base caster, GlobalTargetInfo globalTargetInfo = default, LocalTargetInfo localTargetInfo = default)
        {
            Messages.Message(content, caster.CasterPawn, messageType);
            return true;
        }
    }
}
