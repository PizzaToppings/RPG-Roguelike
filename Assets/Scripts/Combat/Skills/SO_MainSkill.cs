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

    public TargetKindEnum TargetKind;

    private void Awake()
    {
        Reset();
    }

    public virtual void Preview(BoardTile mouseOverTile)
    {
        var SkillPartGroupIndex = SkillData.SkillPartGroupIndex;

        for (int i = 0; i < SkillPartGroups[SkillPartGroupIndex].skillParts.Count; i++)
		{
			var skillPart = SkillPartGroups[SkillPartGroupIndex].skillParts;
			skillPart[i].Preview(mouseOverTile, skillPart);
		}

        if (SkillPartGroupIndex == 0)
            return;

        for (int i = 0; i < SkillPartGroupIndex; i++)
		{
			var spgd = SkillData.SkillPartGroupDatas[i];

            foreach (var spd in spgd.SkillPartDatas)
			{
			    foreach (var tile in spd.TilesHit)
			    {
                    tile.SetColor(castLockColor);
			    }
			}
        }
    }

    public virtual void Reset()
	{
        SkillData.SkillPartGroupIndex = 0;
    }
}
