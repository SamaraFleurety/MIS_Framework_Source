using AKR_Random.Rewards;
using System;
using System.Xml;
using Verse;

namespace AKR_Random
{
    //用于动态生成reward set节点。和直接填效果一样的，但是用这个填比较简单。
    //用法: <{defname}>权重</{defname}> 例: <AK_Weapon_Dusk>10</AK_Weapon_Dusk>
    public class DefWithWeight
    {
        public Def def;

        public int weight = 1;

        //解析xml时，仅会匹配对应类型的def，不会自动匹配子类。需要手动指定，类似原版Class=""
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            XmlAttribute attributeDefType = xmlRoot.Attributes["DefClass"];
            Type defType = typeof(Def);
            if (attributeDefType != null)
            {
                defType = GenTypes.GetTypeInAnyAssembly(attributeDefType.Value);
                if (defType == null)
                {
                    Log.Error("[AKR]Could not find type named " + attributeDefType.Value + " from node " + xmlRoot.OuterXml);
                    defType = typeof(Def);
                }
            }
            DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "def", xmlRoot.Name, overrideFieldType: defType);
            if (xmlRoot.HasChildNodes)
            {
                weight = ParseHelper.FromString<int>(xmlRoot.FirstChild.Value);
            }
        }

        public T TransformToRewardSet<T>() where T : RewardSet_Base
        {
            return RewardSet_Base.GenerateRewardSet<T>(def, 1);
        }
    }
}
