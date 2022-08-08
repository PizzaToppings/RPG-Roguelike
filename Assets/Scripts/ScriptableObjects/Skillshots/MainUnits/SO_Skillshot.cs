using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum OriginTileEnum {Caster, LastTarget, LastTile, MouseOverTarget, Custom};
public enum TargetTileEnum {MouseOverTile, Caster, AwayFromCaster, LastTarget, LastTile, PreviousDirection, MouseOverTarget, Custom};

public class SO_Skillshot : ScriptableObject
{
    List<SO_Skillshot> skillshotList;

    public OriginTileEnum OriginTileKind = OriginTileEnum.Caster; 
    public TargetTileEnum TargetTileKind = TargetTileEnum.MouseOverTile;

    [HideInInspector] public Unit Caster;
    [HideInInspector] public List<BoardTile> OriginTiles;
    [HideInInspector] public BoardTile targetTile;
    [HideInInspector] public int FinalDirection;

    [Space]
    public DamageTypeEnum DamageType;
    public bool MagicalDamage;
    public int Damage;
    public int Range;

    public Color tileColor;

    [HideInInspector] public List<Unit> TargetsHit;
    [HideInInspector] public List<BoardTile> TilesHit;

    // debuffs

    public virtual SO_Skillshot Preview(BoardTile mouseOverTile, List<SO_Skillshot> skillshots)
    {
        skillshotList = skillshots;
        TargetsHit = new List<Unit>();
        TilesHit = new List<BoardTile>();
        OriginTiles = GetOriginalTiles();
        targetTile = GetTargetTile(mouseOverTile);
        return this;
    }

    List<BoardTile> GetOriginalTiles()
    {
        var tiles = new List<BoardTile>();
        SO_Skillshot previousSkillshot = GetPreviousSkillshot();

        if (OriginTileKind == OriginTileEnum.Caster)
            tiles.Add(Caster.currentTile);

        if (OriginTileKind == OriginTileEnum.LastTarget)
        {
            if (previousSkillshot?.TargetsHit?.Count == 0)
                return new List<BoardTile>();
            
            previousSkillshot.TargetsHit.ForEach(x => tiles.Add(x.currentTile));
        }

        if (OriginTileKind == OriginTileEnum.LastTile)
        {
            tiles.AddRange(previousSkillshot.TilesHit);
        }

        if (OriginTileKind == OriginTileEnum.MouseOverTarget)
        {
            if (previousSkillshot?.TargetsHit?.Count == 0)
                return new List<BoardTile>();

            var target = previousSkillshot.TargetsHit.Find(x => x.IsTargeted);
            if (target != null)
            {
                tiles.Add(target.currentTile);
            }
        }

        return tiles;
    }

    BoardTile GetTargetTile(BoardTile mouseOverTile)
    {
        SO_Skillshot previousSkillshot = GetPreviousSkillshot();

        if (TargetTileKind== TargetTileEnum.MouseOverTile)
            return mouseOverTile;

        if (TargetTileKind == TargetTileEnum.Caster)
            return Caster.currentTile;

        return null;
    }

    public SO_Skillshot GetPreviousSkillshot()
    {
        var tiles = new List<BoardTile>();
        var thisIndex = skillshotList.FindIndex(x => x == this);
        if (thisIndex != 0)
            return skillshotList[thisIndex - 1];

        return null;
    }

    public virtual void Cast()
    {
        foreach (var target in TargetsHit)
        {
            var data = Caster.DealDamage(this, target);
            target.TakeDamage(data);
        }
    }
}
