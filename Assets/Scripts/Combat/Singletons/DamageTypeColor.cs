using System.Collections;
using System.Collections.Generic;
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

    public Color GetDamageTypeColor(DamageTypeEnum damageType)
    {
        if (damageType == DamageTypeEnum.Arcane)
            return ArcaneDamageColor;

        if (damageType == DamageTypeEnum.Fire)
            return FireDamageColor;
        
        if (damageType == DamageTypeEnum.Water)
            return WaterDamageColor;
        
        if (damageType == DamageTypeEnum.Earth)
            return EarthDamageColor;
        
        if (damageType == DamageTypeEnum.Ice)
            return IceDamageColor;
        
        if (damageType == DamageTypeEnum.Electric)
            return ElectricDamageColor;
        
        if (damageType == DamageTypeEnum.Holy)
            return HolyDamageColor;
        
        if (damageType == DamageTypeEnum.Dark)
            return DarkDamageColor;
        
        if (damageType == DamageTypeEnum.Poison)
            return PoisonDamageColor;

        if (damageType == DamageTypeEnum.Healing)
            return HealingColor;

        return PhysicalDamageColor;
    }
}
