using UnityEngine;

[CreateAssetMenu(fileName = "ShieldDamageSkillModifier", menuName = "ScriptableObjects/Modifier/ShieldDamageSkillModifier")]
public class ShieldDamageSkillModifier : SkillModifier
{
    public override DamageData Apply(DamageData damageData)
    {
        var shields = damageData.Caster.ShieldPoints;
        damageData.Power += shields;

        return damageData;
    }
}
