using System.Collections.Generic;
using UnityEngine;

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

    public OriginTileEnum OriginTileKind = OriginTileEnum.None; 
    public OriginTargetEnum OriginTargetKind = OriginTargetEnum.None;
    public TargetTileEnum TargetTileKind = TargetTileEnum.MouseOverTile;

    [Space]
    public SO_SKillFX SKillFX;

    [HideInInspector] public List<BoardTile> OriginTiles;
    [HideInInspector] public List<Unit> OriginTargets;
    [HideInInspector] public BoardTile TargetTile;
    [HideInInspector] public SkillPartData PartData;
    [HideInInspector] public SkillPartGroupData MatchedSkillPartGroupData;
    [HideInInspector] public int FinalDirection;
	[HideInInspector] public bool MagicalDamage;
    [HideInInspector] public int SkillPartIndex = 0;

    [Space]
    public DamageTypeEnum DamageType;
    public int Power;
    public int Range;
    
    [Space]
    public List<SO_StatusEffect> StatusEfects;
    public List<DefaultStatusEffect> defaultStatusEffects;
    
    [Space]
    public TileColor tileColor;

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

        if (OriginTileKind == OriginTileEnum.Caster)
        {
            SkillData.Caster = UnitData.CurrentActiveUnit;
            tiles.Add(SkillData.Caster.currentTile);
        }

        if (OriginTileKind == OriginTileEnum.LastTargetTile)
        {
            if (previousTargetsHit.Count == 0)
                return new List<BoardTile>();

            previousTargetsHit.ForEach(x => tiles.Add(x.currentTile));
        }

        if (OriginTileKind == OriginTileEnum.LastTile)
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

        if (OriginTargetKind == OriginTargetEnum.Caster)
		{
            units.Add(SkillData.Caster);
		}

        if (OriginTargetKind == OriginTargetEnum.LastTarget)
		{
            units.AddRange(previousTargetsHit);
		}

        if (OriginTargetKind == OriginTargetEnum.TargetOnLastTiles)
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
        if (TargetTileKind== TargetTileEnum.MouseOverTile)
            return mouseOverTile;

        if (TargetTileKind == TargetTileEnum.CasterTile)
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
