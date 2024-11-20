using System.Collections.Generic;
using UnityEngine;

public class SO_MainSkill : ScriptableObject
{
    public Sprite Image;

    public TileColor castLockColor;

    [TextArea(15,20)]    
    public string Description;

    public enum TargetKindEnum {Enemies, Allies, All};

    public bool MagicalDamage; //change to enum?

    public List<SkillPartGroup> SkillPartGroups;
    int SkillPartGroupNumber => SkillPartGroups.Count; 

    public TargetKindEnum TargetKind;

    private void Awake()
    {
        Reset();
        BoardManager boardManager = BoardManager.Instance;
        UnitManager unitManager = UnitManager.Instance;
    }

    public virtual void Preview(BoardTile mouseOverTile)
    {
        var skillPartIndex = SkillData.SkillPartIndex;
        Debug.Log(skillPartIndex);

        for (int i = 0; i < SkillPartGroups[skillPartIndex].skillParts.Count; i++)
		{
			var skillPart = SkillPartGroups[skillPartIndex].skillParts;
			skillPart[i].skillPartIndex = i;
			skillPart[i].Preview(mouseOverTile, skillPart);
		}

        if (skillPartIndex == 0)
            return;

        for (int i = 0; i < skillPartIndex; i++)
		{
			var spd = SkillData.SkillPartDatas[i];

			foreach (var tile in spd.TilesHit)
			{
                tile.SetColor(castLockColor);
			}
        }
    }

    public virtual void Reset()
	{
        SkillData.SkillPartIndex = 0;
    }

    public virtual void Cast()
    {
        if (SkillData.SkillPartIndex < SkillData.SkillPartDatas.Count - 1)
		{
            SkillData.SkillPartIndex++;
			return;
        }

        foreach (var spg in SkillPartGroups)
		{
            foreach (var sp in spg.skillParts)
			{
                sp.Cast();
			}
		}

        Character character = SkillData.Caster as Character;
        character.StopCasting();
    }
}
