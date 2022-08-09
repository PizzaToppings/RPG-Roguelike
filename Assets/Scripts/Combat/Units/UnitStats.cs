using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public string UnitName;

    public bool Friendly;
    public int MaxHitpoints;
    public int Hitpoints;
    public int ShieldPoints;

    public int MoveSpeed;
    public int MoveSpeedLeft;
    public int Initiative;

    public int MaxSkillShotAmount = 4;
    public List<bool> SkillshotsEquipped;
    public List<SO_MainSkillshot> skillshots = new List<SO_MainSkillshot>();
    public bool IsTargeted;

    public int PhysicalPower;
    public int MagicalPower;
    // public int PhysicalDefense;
    // public int MagicalDefense;

    public List<DamageTypeEnum> Resistances;

    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    [HideInInspector] public CombatManager combatManager;
    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public UnitManager unitManager;
    [HideInInspector] public StatusEffectManager statusEffectManager;
    public BoardTile currentTile = null;

}
