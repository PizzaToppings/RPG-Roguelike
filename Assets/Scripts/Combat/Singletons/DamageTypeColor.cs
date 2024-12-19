using UnityEngine;

public class DamageTypeColor : MonoBehaviour
{
    [SerializeField] Color PhysicalDamageColor;
    [SerializeField] Color ArcaneDamageColor;
    [SerializeField] Color FireDamageColor;
    [SerializeField] Color WaterDamageColor;
    [SerializeField] Color EarthDamageColor;
    [SerializeField] Color IceDamageColor;
    [SerializeField] Color ElectricDamageColor;
    [SerializeField] Color PsychicDamageColor;
    [SerializeField] Color HolyDamageColor;
    [SerializeField] Color DarkDamageColor;
    [SerializeField] Color PoisonDamageColor;
    [SerializeField] Color HealingColor;
    [SerializeField] Color ShieldColor;

    public Color GetDamageTypeColor(DamageTypeEnum damageType)
    {
        switch (damageType)
        {
            case DamageTypeEnum.Arcane:
                return ArcaneDamageColor;
            case DamageTypeEnum.Fire:
                return FireDamageColor;
            case DamageTypeEnum.Water:
                return WaterDamageColor;
            case DamageTypeEnum.Earth:
                return EarthDamageColor;
            case DamageTypeEnum.Ice:
                return IceDamageColor;
            case DamageTypeEnum.Electric:
                return ElectricDamageColor;
            case DamageTypeEnum.Holy:
                return HolyDamageColor;
            case DamageTypeEnum.Dark:
                return DarkDamageColor;
            case DamageTypeEnum.Poison:
                return PoisonDamageColor;
            case DamageTypeEnum.Healing:
                return HealingColor;
            case DamageTypeEnum.Shield:
                return ShieldColor;
        }
        return PhysicalDamageColor;
    }
}
