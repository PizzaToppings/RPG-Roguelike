using System.Collections.Generic;
using UnityEngine;

public class Skill
{
    public SO_MainSkill mainSkillSO;

    [Space]
    public List<SkillPartGroup> SkillPartGroups = new List<SkillPartGroup>(1);

    [Space]
    public int EnergyCost = 10;
    public int DefaultCharges = 1;
    [HideInInspector] public int Charges = 1;

    [HideInInspector] public Unit Caster;

    public void Init(SO_MainSkill skillSO)
    {
        mainSkillSO = skillSO;
        EnergyCost = skillSO.EnergyCost;
        DefaultCharges = skillSO.DefaultCharges;
        SkillPartGroups = skillSO.SkillPartGroups;
        Charges = DefaultCharges;
    }

    public void Preview(BoardTile mouseOverTile, Unit caster, BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit target = null)
    {
        var SkillPartGroupIndex = SkillData.SkillPartGroupIndex;
        Caster = caster;

        for (int i = 0; i < SkillPartGroups[SkillPartGroupIndex].skillParts.Count; i++)
        {
            var skillPartList = SkillPartGroups[SkillPartGroupIndex].skillParts;
            skillPartList[i].Preview(mouseOverTile, skillPartList, caster, overwriteOriginTile, overwriteTargetTile, target);
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
                    tile.SetColor(mainSkillSO.castLockColor);
                }

                foreach (var unit in spd.TargetsHit)
                {
                    unit.Tile.SetColor(mainSkillSO.castLockColor);
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
        Charges = DefaultCharges;
        SkillData.SkillPartGroupIndex = 0;
    }

    public virtual void Reset()
    {
        SkillData.SkillPartGroupIndex = 0;
    }
}
