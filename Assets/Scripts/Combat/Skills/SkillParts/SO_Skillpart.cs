using System.Linq;
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

    [Header(" - Targeting")]
    public OriginTileEnum OriginTileKind = OriginTileEnum.None;
    public List<SO_Skillpart> OriginTileSkillParts;

    public OriginTargetEnum OriginTargetKind = OriginTargetEnum.None;
    public List<SO_Skillpart> OriginTargetSkillParts;

    public TargetTileEnum TargetTileKind = TargetTileEnum.None;
    public List<SO_Skillpart> TargetTileSkillParts;

    public OriginTileEnum DirectionAnchor = OriginTileEnum.None;
    public SO_Skillpart DirectionAnchorSkillPart;


    [Header(" - Visuals")]
    public TileColor tileColor;
    public bool AddProjectileLine;
    public float ProjectileLineOffset = 1;

    [Space]
    [Header(" - Damage and Range")]
    public DamageTypeEnum DamageType;
    public int Power;
    public float MinRange;
    public float MaxRange;
    [Space]
    public bool IncludeInAutoMove;
    public bool AffectedByOpenTiles;
    public bool AffectedByBlockedTiles;

    [Space]
    [Header(" - Prevent Duplicates")]
    public List<SO_Skillpart> PreventDuplicateTiles;
    public List<SO_Skillpart> PreventDuplicateTargets;

    [Space]
    [Header(" - Visuals")]
    public SO_SKillVFX[] SkillVFX;

    [Space]
    [Header(" - StatusEffects")]
    public List<SO_StatusEffect> StatusEfects;
    public List<DefaultStatusEffect> defaultStatusEffects;
    public SO_DisplacementEffect displacementEffect;
    
    List<SO_Skillpart> skillPartsList;

    public virtual SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillParts,
        BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit target = null)
    {
        skillPartsList = skillParts;

        SetInitData(mouseOverTile);

        PartData.CanCast = true;

        return this;
    }

    public void SetInitData(BoardTile mouseOverTile)
	{
        PartData.TilesHit = new List<BoardTile>();
        PartData.TargetsHit = new List<Unit>();
        OriginTiles = GetOriginTiles();
        OriginTargets = GetOriginTargets();
        TargetTile = GetTargetTile(mouseOverTile);
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

        switch (OriginTileKind)
		{
            case OriginTileEnum.Caster:
                SkillData.Caster = UnitData.ActiveUnit;
                tiles.Add(SkillData.Caster.Tile);
                break;

            case OriginTileEnum.LastTargetTile:
                if (previousTargetsHit.Count == 0)
                    return new List<BoardTile>();

                previousTargetsHit.ForEach(x => tiles.Add(x.Tile));
                break;

            case OriginTileEnum.LastTile:
                tiles.AddRange(previousTilesHit);
                break;

            case OriginTileEnum.GetFromSkillPart:
                var tileList = OriginTileSkillParts.SelectMany(x => x.PartData.TilesHit).ToList();
                tiles.AddRange(tileList);
                break;
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
        if (TargetTileKind == TargetTileEnum.MouseOverTile)
            return mouseOverTile;

        if (TargetTileKind == TargetTileEnum.CasterTile)
            return SkillData.Caster.Tile;

        return null;
    }

    public SO_Skillpart GetPreviousSkillPart() // TODO move to skillmanager?
    {
        var thisIndex = skillPartsList.FindIndex(x => x == this);
        if (thisIndex != 0)
            return skillPartsList[thisIndex - 1];

        return null;
    }

    public virtual void ShowProjectileLine(Vector3 casterPosition, Vector3 targetPosition)
	{
        if (MaxRange > 1.5f) // so more than melee
        {
            var skillVFXManager = SkillVFXManager.Instance;
            skillVFXManager.PreviewProjectileLine(casterPosition, targetPosition, ProjectileLineOffset);
        }
    }

    public virtual void SetTargetAndTile(Unit target, BoardTile tile)
	{
	}

    public virtual bool NoTargetsInRange()
	{
        return false;
	}

    public bool HasDuplicateTarget(Unit target)
	{
        return PreventDuplicateTargets.Any(skillPart => skillPart.PartData.TargetsHit.Contains(target));
    }

    public bool HasDuplicateTile(BoardTile tile)
    {
        return PreventDuplicateTiles.Any(skillPart => skillPart.PartData.TilesHit.Contains(tile));
    }
}
