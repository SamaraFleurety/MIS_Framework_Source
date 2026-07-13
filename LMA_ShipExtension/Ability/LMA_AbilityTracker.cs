using AK_DLL;
using AKA_Ability;
using System.Collections.Generic;
using Verse;

namespace LMA_Lib.Ability
{
    public class LMA_AbilityTracker : AK_AbilityTracker
    {
        public Dictionary<int, LocalTargetInfo> shipEqTargets = new();
        //舰装用 基本上是开火的效果，伪装成技能
        public Dictionary<int, AKAbility_Base> equipmentAbilities = new();

        public AKAbility_Base AddAbility(AKAbilityDef def, int id)
        {
            if (equipmentAbilities.TryGetValue(id, out AKAbility_Base ea))
            {
                Log.Error($"[LMA]AbilityTracker for {owner} already has an equipment ability with ID {id}.");
                return ea;
            }

            AKAbility_Base ability = def.MakeAbility(this);

            equipmentAbilities.Add(id, ability);

            return ability;
        }

        public void RemoveAbility(int id)
        {
            if (equipmentAbilities.ContainsKey(id))
            {
                equipmentAbilities.Remove(id);
            }
        }
    }
}
