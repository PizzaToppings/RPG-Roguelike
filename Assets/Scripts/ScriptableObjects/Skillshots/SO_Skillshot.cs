using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Skillshot", menuName = "ScriptableObjects/Skillshot", order = 1)]
public class SO_Skillshot : ScriptableObject
{
    public enum OriginTileEnum {Caster, LastTarget, Click};

    List<SO_Skillshot> skillshotList;

    [HideInInspector] public Unit Caster;
    public OriginTileEnum OriginTileKind;
    [HideInInspector] public List<BoardTile> OriginTiles;
    public int Damage;
    public int Range;

    public Color tileColor;

    public List<Unit> TargetsHit;

    // public DamageType DamageType;
    // debuffs

    public virtual SO_Skillshot Preview(BoardTile mouseOverTile, List<SO_Skillshot> skillshots)
    {
        skillshotList = skillshots;
        TargetsHit = new List<Unit>();
        OriginTiles = GetOriginalTiles();
        return this;
    }

    List<BoardTile> GetOriginalTiles()
    {
        var tiles = new List<BoardTile>();

        if (OriginTileKind == SO_Skillshot.OriginTileEnum.Caster)
            tiles.Add(Caster.currentTile);

        if (OriginTileKind == SO_Skillshot.OriginTileEnum.LastTarget)
        {
            var thisIndex = skillshotList.FindIndex(x => x == this);
            var previousSkillshot = skillshotList[thisIndex - 1];

            if (previousSkillshot?.TargetsHit?.Count == 0)
                return new List<BoardTile>();
            
            previousSkillshot.TargetsHit.ForEach(x => tiles.Add(x.currentTile));
        }

        return tiles;
    }
}
