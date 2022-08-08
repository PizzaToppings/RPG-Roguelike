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

    public bool Blinded; 
    public bool Silenced; 
    public bool Charmed; 
    public bool Frightened; 
    public bool Incapacitated; 
    public bool Stunned; 
    public bool Poisoned; 
    public bool Rooted; 

    public bool Invisible;
    public int LifeSteal;
    public int Unstoppable;

    [HideInInspector] public CombatManager combatManager;
    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public UnitManager unitManager;
    public BoardTile currentTile = null;

}
