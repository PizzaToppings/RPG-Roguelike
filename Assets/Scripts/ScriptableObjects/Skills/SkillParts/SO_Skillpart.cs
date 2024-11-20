using System.Collections.Generic;
using UnityEngine;

public enum OriginTileEnum {Caster, LastTarget, LastTile, MouseOverTarget};
public enum TargetTileEnum {MouseOverTile, CasterTile, Caster, PreviousDirection, MouseOverTarget};

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

    //[HideInInspector] public Unit Caster;
    [HideInInspector] public List<BoardTile> OriginTiles;
    [HideInInspector] public BoardTile TargetTile;
    [HideInInspector] public int FinalDirection;
    [HideInInspector] public int skillPartIndex;
	[HideInInspector] public bool MagicalDamage;

	[Space]
    public DamageTypeEnum DamageType;
    public int Damage;
    public int Range;
    
    [Space]
    public List<SO_StatusEffect> StatusEfects;
    public List<DefaultStatusEffect> defaultStatusEffects;
    
    [Space]
    public TileColor tileColor;
    
    // debuffs

    public virtual SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillParts)
    {
        skillPartsList = skillParts;

        SkillData.SkillPartDatas[SkillData.SkillPartIndex].TilesHit = new List<BoardTile>();
        SkillData.SkillPartDatas[SkillData.SkillPartIndex].TargetsHit = new List<Unit>();
        OriginTiles = GetOriginTiles();
        TargetTile = GetTargetTile(mouseOverTile);
        return this;
    }

    List<BoardTile> GetOriginTiles()
    {
        var tiles = new List<BoardTile>();
        SO_Skillpart previousSkillPart = GetPreviousSkillPart();
        var previousTargetsHit = new List<Unit>();
        var previousTilesHit = new List<BoardTile>();

        if (SkillData.SkillPartIndex > 0)
		{
            previousTargetsHit = SkillData.SkillPartDatas[SkillData.SkillPartIndex - 1].TargetsHit;
            previousTilesHit = SkillData.SkillPartDatas[SkillData.SkillPartIndex - 1].TilesHit;
		}

        if (OriginTileKind == OriginTileEnum.Caster)
        {
            SkillData.Caster = UnitData.CurrentActiveUnit;
            tiles.Add(SkillData.Caster.currentTile);
        }

        if (OriginTileKind == OriginTileEnum.LastTarget)
        {
            if (previousTargetsHit.Count == 0)
                return new List<BoardTile>();

            previousTargetsHit.ForEach(x => tiles.Add(x.currentTile));
        }

        if (OriginTileKind == OriginTileEnum.LastTile)
        {
            tiles.AddRange(previousTilesHit);
        }

        if (OriginTileKind == OriginTileEnum.MouseOverTarget)
        {
            if (previousTargetsHit.Count == 0)
                return new List<BoardTile>();

            var target = previousTargetsHit.Find(x => x.IsTargeted);
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

        if (TargetTileKind == TargetTileEnum.CasterTile)
            return SkillData.Caster.currentTile;

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

        Debug.Log("casting...");
        
        foreach (var tile in SkillData.CurrentTilesHit)
		{
            Debug.Log(tile.Coordinates);
		}

        foreach (var target in SkillData.CurrentTargetsHit)
        {
            var data = damageManager.DealDamage(this, target);
            damageManager.TakeDamage(data);
        }
    }
}
