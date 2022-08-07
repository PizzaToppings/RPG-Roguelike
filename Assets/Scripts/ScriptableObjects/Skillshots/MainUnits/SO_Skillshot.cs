using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Skillshot : ScriptableObject
{
    public enum OriginTileEnum {Caster, LastTarget, LastTile, MouseOverTarget, custom};

    List<SO_Skillshot> skillshotList;

    [HideInInspector] public Unit Caster;
    public OriginTileEnum OriginTileKind = OriginTileEnum.Caster;
    [HideInInspector] public List<BoardTile> OriginTiles;
    public int Damage;
    public int Range;

    public Color tileColor;

    [HideInInspector] public List<Unit> TargetsHit;
    [HideInInspector] public List<BoardTile> TilesHit;

    // public DamageType DamageType;
    // debuffs

    public virtual SO_Skillshot Preview(BoardTile mouseOverTile, List<SO_Skillshot> skillshots)
    {
        skillshotList = skillshots;
        TargetsHit = new List<Unit>();
        TilesHit = new List<BoardTile>();
        OriginTiles = GetOriginalTiles();
        return this;
    }

    List<BoardTile> GetOriginalTiles()
    {
        var tiles = new List<BoardTile>();
        var thisIndex = skillshotList.FindIndex(x => x == this);
        SO_Skillshot previousSkillshot = null;
        if (thisIndex != 0)
            previousSkillshot = skillshotList[thisIndex - 1];

        if (OriginTileKind == SO_Skillshot.OriginTileEnum.Caster)
            tiles.Add(Caster.currentTile);

        if (OriginTileKind == SO_Skillshot.OriginTileEnum.LastTarget)
        {
            if (previousSkillshot?.TargetsHit?.Count == 0)
                return new List<BoardTile>();
            
            previousSkillshot.TargetsHit.ForEach(x => tiles.Add(x.currentTile));
        }

        if (OriginTileKind == SO_Skillshot.OriginTileEnum.LastTile)
        {
            tiles.AddRange(previousSkillshot.TilesHit);
        }

        if (OriginTileKind == SO_Skillshot.OriginTileEnum.MouseOverTarget)
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
}
