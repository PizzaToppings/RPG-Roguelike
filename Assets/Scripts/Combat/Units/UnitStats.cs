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

    public bool IsTargeted;

    public int PhysicalPower;
    public int MagicalPower;
	public int PhysicalDefense;
	public int MagicalDefense;

	public List<DamageTypeEnum> Resistances;
	public List<DamageTypeEnum> Vulnerabilities;

    public List<DefaultStatusEffect> statusEffects = new List<DefaultStatusEffect>();

    public BoardTile Tile = null;

}
