using RimWorld;
using RimWorld.Planet;
using System.Security.Cryptography;
using Verse;
using Verse.Sound;

namespace AKA_Ability.AbilityEffect
{
    //左上角会嘟嘟一下的消息 估计都是给自动释放技能用做提示
    public class AE_SendMessage : AbilityEffectBase
    {
        public string content;
        public MessageTypeDef messageType;
        public override bool DoEffect(AKAbility_Base caster, GlobalTargetInfo globalTargetInfo = default, LocalTargetInfo localTargetInfo = default)
        {
            messageType ??= MessageTypeDefOf.NegativeHealthEvent;
            Messages.Message(content, caster.CasterPawn, messageType, false);
            return true;
        }

    }
}
