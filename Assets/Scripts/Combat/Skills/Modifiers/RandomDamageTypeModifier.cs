using UnityEngine;


[CreateAssetMenu(fileName = "RandomDamageTypeModifer", menuName = "ScriptableObjects/Modifier/RandomDamageTypeModifer")]
public class RandomDamageTypeModifier : SkillModifier
{
    public override DamageData Apply(DamageData damageData)
    {
        damageData.HitType = HitTypeEnum.Damage;
        return damageData;
    }
}
