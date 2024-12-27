using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetTileSkill", menuName = "ScriptableObjects/SkillParts/TargetTileSkill")]
public class SO_TargetBoardtileSkill : SO_Skillpart
{
    [Header(" - TargetTile Specific")]
    public TileColor SelectedTileColor;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots, 
        BoardTile overwriteOriginTile = null, BoardTile overwriteTargetTile = null, Unit target = null) 
    {
        TargetTileKind = TargetTileEnum.MouseOverTile;

        base.Preview(mouseOverTile, skillshots);

        if (overwriteOriginTile != null)
            OriginTiles[0] = overwriteOriginTile;

        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.GetAOE(this);

        if (PartData.TilesHit.Contains(mouseOverTile) && HasDuplicateTile(mouseOverTile) == false)
        {
            PartData.TilesHit.Clear();
            mouseOverTile.SetColor(SelectedTileColor);

            PartData.TilesHit.Add(mouseOverTile);

            if (AddProjectileLine)
                ShowProjectileLine(SkillData.Caster.position, PartData.TilesHit[0].position);

            return this;
        }

        PartData.TilesHit.Clear();
        PartData.CanCast = false;

        return this;
    }

	public override bool UnableToCast()
	{
        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.GetAOE(this);

        if (PartData.TilesHit.Count == 0)
            return true;

        return false;
    }
}
