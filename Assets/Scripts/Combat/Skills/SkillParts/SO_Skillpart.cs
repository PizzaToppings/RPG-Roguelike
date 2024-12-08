using System.Collections.Generic;
using UnityEngine;

public class SO_Skillpart : ScriptableObject
{
    [HideInInspector] public List<BoardTile> OriginTiles;
    [HideInInspector] public List<Unit> OriginTargets;
    [HideInInspector] public BoardTile TargetTile;
    [HideInInspector] public SkillPartData PartData;
    [HideInInspector] public SkillPartGroupData MatchedSkillPartGroupData;
    [HideInInspector] public int FinalDirection;
    [HideInInspector] public bool MagicalDamage;
    [HideInInspector] public int SkillPartIndex = 0;

    [HideInInspector] public OriginTileEnum OriginTileMain = OriginTileEnum.None;
    [HideInInspector] public OriginTargetEnum OriginTargetMain = OriginTargetEnum.None;
    [HideInInspector] public TargetTileEnum TargetTileMain = TargetTileEnum.None;

    [Header(" - Visuals")]
    public SO_SKillFX[] SKillFX;
    public TileColor tileColor;

    [Space]
    [Header(" - Damage and Range")]
    public DamageTypeEnum DamageType;
    public int Power;
    public float Range;
    
    [Space]
    [Header(" - StatusEffects")]
    public List<SO_StatusEffect> StatusEfects;
    public List<DefaultStatusEffect> defaultStatusEffects;
    
    List<SO_Skillpart> skillPartsList;

    public virtual SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillParts)
    {
        skillPartsList = skillParts;

        PartData.TilesHit = new List<BoardTile>();
        PartData.TargetsHit = new List<Unit>();
        OriginTiles = GetOriginTiles();
        OriginTargets = GetOriginTargets();
        TargetTile = GetTargetTile(mouseOverTile);
        return this;
    }

    List<BoardTile> GetOriginTiles()
    {
        var tiles = new List<BoardTile>();
        var previousTargetsHit = new List<Unit>();
        var previousTilesHit = new List<BoardTile>();

        if (SkillPartIndex > 0)
		{
            previousTargetsHit = SkillData.GetPreviousTargetsHit(SkillPartIndex);
            previousTilesHit = SkillData.GetPreviousTilesHit(SkillPartIndex);
		}

        if (OriginTileMain == OriginTileEnum.Caster)
        {
            SkillData.Caster = UnitData.CurrentActiveUnit;
            tiles.Add(SkillData.Caster.currentTile);
        }

        if (OriginTileMain == OriginTileEnum.LastTargetTile)
        {
            if (previousTargetsHit.Count == 0)
                return new List<BoardTile>();

            previousTargetsHit.ForEach(x => tiles.Add(x.currentTile));
        }

        if (OriginTileMain == OriginTileEnum.LastTile)
        {
            tiles.AddRange(previousTilesHit);
        }

        return tiles;
    }

    List<Unit> GetOriginTargets()
	{
        var units = new List<Unit>();
        var previousTargetsHit = new List<Unit>();
        var previousTilesHit = new List<BoardTile>();

        if (SkillPartIndex > 0)
        {
            previousTargetsHit = SkillData.GetPreviousTargetsHit(SkillPartIndex);
            previousTilesHit = SkillData.GetPreviousTilesHit(SkillPartIndex);
        }

        if (OriginTargetMain == OriginTargetEnum.Caster)
		{
            units.Add(SkillData.Caster);
		}

        if (OriginTargetMain == OriginTargetEnum.LastTarget)
		{
            units.AddRange(previousTargetsHit);
		}

        if (OriginTargetMain == OriginTargetEnum.TargetOnLastTiles)
        {
            foreach (var tile in previousTilesHit)
			{
                if (tile.currentUnit != null)
				{
                    units.Add(tile.currentUnit);
				}
			}
        }

        return units;
    }

    BoardTile GetTargetTile(BoardTile mouseOverTile)
    {
        if (TargetTileMain == TargetTileEnum.MouseOverTile)
            return mouseOverTile;

        if (TargetTileMain == TargetTileEnum.CasterTile)
            return SkillData.Caster.currentTile;

        return null;
    }

    public SO_Skillpart GetPreviousSkillPart() // TODO move to skillmanager?
    {
        var thisIndex = skillPartsList.FindIndex(x => x == this);
        if (thisIndex != 0)
            return skillPartsList[thisIndex - 1];

        return null;
    }

    public virtual void SetTargetAndTile(Unit target, BoardTile tile)
	{
	}
}
