using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SO_Skillshot : ScriptableObject
{
    public enum OriginTileEnum {Caster, LastTarget, Click, MouseOverTarget, custom};

    List<SO_Skillshot> skillshotList;

    [HideInInspector] public Unit Caster;
    public OriginTileEnum OriginTileKind = OriginTileEnum.Caster;
    [HideInInspector] public List<BoardTile> OriginTiles;
    public int Damage;
    public int Range;

    public Color tileColor;

    [HideInInspector] public List<Unit> TargetsHit;

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
        var thisIndex = skillshotList.FindIndex(x => x == this);

        if (OriginTileKind == SO_Skillshot.OriginTileEnum.Caster)
            tiles.Add(Caster.currentTile);

        if (OriginTileKind == SO_Skillshot.OriginTileEnum.LastTarget)
        {
            var previousSkillshot = skillshotList[thisIndex - 1];

            if (previousSkillshot?.TargetsHit?.Count == 0)
                return new List<BoardTile>();
            
            previousSkillshot.TargetsHit.ForEach(x => tiles.Add(x.currentTile));
        }

        if (OriginTileKind == SO_Skillshot.OriginTileEnum.MouseOverTarget)
        {
            var previousSkillshot = skillshotList[thisIndex - 1];

            if (previousSkillshot?.TargetsHit?.Count == 0)
                return new List<BoardTile>();

            Debug.Log("previousSkillshot" + previousSkillshot.name);

            var target = previousSkillshot.TargetsHit.Find(x => x.IsTargeted);
            if (target != null)
            {
                Debug.Log(this.name + ", target tile: " + target.currentTile.name);
                tiles.Add(target.currentTile);
            }
        }

        return tiles;
    }
}
