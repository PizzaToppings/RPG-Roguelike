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
    public float MoveSpeedLeft;
    public int Initiative;

    public currentUnitAction CurrentUnitAction;

    
    public bool IsTargeted;

    public int PhysicalPower;
    public int MagicalPower;
	public int PhysicalDefense;
	public int MagicalDefense;

	public List<DamageTypeEnum> Resistances;
	public List<DamageTypeEnum> Vulnerabilities;

    public List<DefaultStatusEffect> statusEffects = new List<DefaultStatusEffect>();

    [HideInInspector] public CombatManager combatManager;
    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public UnitManager unitManager;
    [HideInInspector] public SkillsManager skillsManager;
    [HideInInspector] public StatusEffectManager statusEffectManager;
    [HideInInspector] public DamageManager damageManager;
    public BoardTile currentTile = null;

}
