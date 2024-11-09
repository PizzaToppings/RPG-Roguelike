using System.Collections.Generic;
using UnityEngine;


public enum OriginTileEnum {Caster, LastTarget, LastTile, MouseOverTarget, Custom};
public enum TargetTileEnum {MouseOverTile, Caster, AwayFromCaster, LastTarget, LastTile, PreviousDirection, MouseOverTarget, Custom};

public class SO_Skillpart : ScriptableObject
{
    [System.Serializable]
    public class DefaultStatusEffect 
    {
        public StatusEfectEnum Type;
        public int Duration;

        public DefaultStatusEffect(StatusEfectEnum Type, int Duration)
        {
            this.Type = Type;
            this.Duration = Duration;
        }
    }

    List<SO_Skillpart> skillPartsList;

    public OriginTileEnum OriginTileKind = OriginTileEnum.Caster; 
    public TargetTileEnum TargetTileKind = TargetTileEnum.MouseOverTile;

    [HideInInspector] public Unit Caster;
    [HideInInspector] public List<BoardTile> OriginTiles;
    [HideInInspector] public BoardTile targetTile;
    [HideInInspector] public int FinalDirection;
    [HideInInspector] public int skillPartIndex;
    // [HideInInspector] public bool MagicalDamage;

    [Space]
    public DamageTypeEnum DamageType;
    public int Damage;
    public int Range;
    
    [Space]
    public List<SO_StatusEffect> StatusEfects;
    public List<DefaultStatusEffect> defaultStatusEffects;
    
    [Space]
    public TileColor tileColor;

    [HideInInspector] public List<Unit> TargetsHit;
    [HideInInspector] public List<BoardTile> TilesHit;

    // debuffs

    public virtual SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillParts)
    {
        skillPartsList = skillParts;
        TargetsHit = new List<Unit>();
        TilesHit = new List<BoardTile>();
        OriginTiles = GetOriginTiles();
        targetTile = GetTargetTile(mouseOverTile);
        return this;
    }

    List<BoardTile> GetOriginTiles()
    {
        var tiles = new List<BoardTile>();
        SO_Skillpart previousSkillPart = GetPreviousSkillPart();

        if (OriginTileKind == OriginTileEnum.Caster) // is actually caster?
        {
            Caster = UnitData.CurrentActiveUnit;
            tiles.Add(Caster.currentTile);
        }

        if (OriginTileKind == OriginTileEnum.LastTarget)
        {
            if (previousSkillPart?.TargetsHit?.Count == 0)
                return new List<BoardTile>();
            
            previousSkillPart.TargetsHit.ForEach(x => tiles.Add(x.currentTile));
        }

        if (OriginTileKind == OriginTileEnum.LastTile)
        {
            tiles.AddRange(previousSkillPart.TilesHit);
        }

        if (OriginTileKind == OriginTileEnum.MouseOverTarget)
        {
            if (previousSkillPart?.TargetsHit?.Count == 0)
                return new List<BoardTile>();

            var target = previousSkillPart.TargetsHit.Find(x => x.IsTargeted);
            if (target != null)
            {
                tiles.Add(target.currentTile);
            }
        }

        return tiles;
    }

    BoardTile GetTargetTile(BoardTile mouseOverTile)
    {
        SO_Skillpart previousSkillshot = GetPreviousSkillPart();

        if (TargetTileKind== TargetTileEnum.MouseOverTile)
            return mouseOverTile;

        if (TargetTileKind == TargetTileEnum.Caster)
            return Caster.currentTile;

        return null;
    }

    public SO_Skillpart GetPreviousSkillPart()
    {
        var tiles = new List<BoardTile>();
        var thisIndex = skillPartsList.FindIndex(x => x == this);
        if (thisIndex != 0)
            return skillPartsList[thisIndex - 1];

        return null;
    }

    public virtual void Cast()
    {
        var damageManager = DamageManager.Instance;

        foreach (var target in TargetsHit)
        {
            var data = damageManager.DealDamage(this, target);
            damageManager.TakeDamage(data);
        }
    }
}
