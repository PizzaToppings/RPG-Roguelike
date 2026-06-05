using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    public string UnitName;

    public bool Friendly;
    public bool Summon;
    public int MaxHitpoints;
    public int Hitpoints;
    public int ShieldPoints;

    public int MoveSpeed;
    public float MoveSpeedLeft;
    public int Initiative;

    public bool IsTargeted;

    public int Power;
	public int Armor;
	public int Range;

	public CombatStyle CurrentCombatStyle = CombatStyle.None;
    // The combat style selected by using a skill this turn. Applied at end of turn.
    public CombatStyle PendingCombatStyle = CombatStyle.None;

    public List<StatusEffect> statusEffects = new List<StatusEffect>();

    public BoardTile Tile = null;
    
    // Targets hit by the unit's most recently cast skill. Populated at cast time and
    // consumed by systems such as CombatStyle effects at end of turn.
    public List<Unit> LastSkillTargets = new List<Unit>();

}
