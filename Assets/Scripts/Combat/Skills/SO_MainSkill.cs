using System.Collections.Generic;
using UnityEngine;

public class SO_MainSkill : ScriptableObject
{
    public Sprite Image;
    public Sprite Image_Inactive;

    [Space]
    public List<ClassEnum> Classes;

    [Space]
    [TextArea(15,20)]    
    public string Description;

    [Space]
    public List<SkillPartGroup> SkillPartGroups = new List<SkillPartGroup>(1);

    [Space]
    public int EnergyCost;
    public int DafaultCharges = 1;
    [HideInInspector] public int Charges = 1;

    [Space]
    public TargetKindEnum TargetKind;
    public CursorType Cursor;

    public TileColor castLockColor;


    public virtual void Preview(BoardTile mouseOverTile, BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit target = null)
    {
        var SkillPartGroupIndex = SkillData.SkillPartGroupIndex;

        for (int i = 0; i < SkillPartGroups[SkillPartGroupIndex].skillParts.Count; i++)
		{
			var skillPart = SkillPartGroups[SkillPartGroupIndex].skillParts;
			skillPart[i].Preview(mouseOverTile, skillPart, overwriteOriginTile, overwriteTargetTile, target);
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

                foreach (var unit in spd.TargetsHit)
				{
                    unit.currentTile.SetColor(castLockColor);
				}
			}
        }
    }

    public float GetAttackRange()
    {
        var totalRange = 0f;

        foreach (var spg in SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
			{
                if (sp.IncludeInAutoMove)
                    totalRange += sp.MaxRange;
            }
        }

        return totalRange;
    }

    public void SetTargetAndTile(Unit target, BoardTile tile)
	{
        foreach (var spg in SkillPartGroups)
        {
            foreach (var sp in spg.skillParts)
            {
                sp.SetTargetAndTile(target, tile);
            }
        }
    }

    public void Init()
    {
        Charges = DafaultCharges;
        SkillData.SkillPartGroupIndex = 0;
    }

    public virtual void Reset()
	{
        SkillData.SkillPartGroupIndex = 0;
    }
}
