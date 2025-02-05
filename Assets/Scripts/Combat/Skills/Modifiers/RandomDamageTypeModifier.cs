using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RandomDamageTypeModifer", menuName = "ScriptableObjects/Modifier/RandomDamageTypeModifer")]
public class RandomDamageTypeModifier : SkillModifier
{
    public override DamageData Apply(DamageData damageData)
    {
        var type = GetRandomType();
        damageData.DamageType = type;

        return damageData;
    }

    DamageTypeEnum GetRandomType()
    {
        int randomNumber = UnityEngine.Random.Range(0, 10);
        var values = Enum.GetValues(typeof(DamageTypeEnum));

        return (DamageTypeEnum)values.GetValue(randomNumber);
    }
}
