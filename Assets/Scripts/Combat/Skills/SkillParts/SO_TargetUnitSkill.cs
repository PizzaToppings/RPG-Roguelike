using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetUnitSkill", menuName = "ScriptableObjects/SkillParts/TargetUnitSkill")]
public class SO_TargetUnitSkill : SO_Skillpart
{
    [Header(" - TargetUnit Specific")]
    public TileColor SelectedTargetTileColor;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        base.OriginTileKind = OriginTileKind;
        TargetTileKind = TargetTileEnum.MouseOverTarget;

        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.GetAOE(this);

        var target = SkillData.GetCurrentTargetsHit(SkillPartIndex).Find(x => x.IsTargeted);
		if (target != null)
		{
            TargetUnit(target);
		}

		return this;
    }

    public void TargetUnit(Unit target)
	{
        target.currentTile.SetColor(SelectedTargetTileColor);

        SkillData.GetCurrentTargetsHit(SkillPartIndex).Clear();
        SkillData.GetCurrentTargetsHit(SkillPartIndex).Add(target);
    }

    public override void SetTargetAndTile(Unit target, BoardTile tile)
    {
        TargetUnit(target);
    }
}
