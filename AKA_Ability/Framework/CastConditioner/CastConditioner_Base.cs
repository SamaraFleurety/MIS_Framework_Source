namespace AKA_Ability.CastConditioner
{
    public abstract class CastConditioner_Base
    {
        //想了想觉得还是有参数
        /*private static Dictionary<Type, CastConditioner_Base> cachedConditioners = new();

        public static bool IsCastable(Type conditionerType, AKAbility instance)
        {
            if (!cachedConditioners.ContainsKey(conditionerType)) cachedConditioners.Add(conditionerType, (CastConditioner_Base)Activator.CreateInstance(conditionerType));

            return cachedConditioners[conditionerType].Castable(instance);
        }*/

        public string failReason = "AKA_Disabled";

        public bool ignoredByAuto = false;

        public abstract bool Castable(AKAbility_Base instance);
    }
}
