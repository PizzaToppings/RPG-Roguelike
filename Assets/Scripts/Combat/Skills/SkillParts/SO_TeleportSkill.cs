using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeleportSkill", menuName = "ScriptableObjects/SkillParts/TeleportSkill")]
public class SO_TeleportSkill : SO_Skillpart
{
    [Header(" - Teleport Specific")]
    public TileColor SelectedTileColor;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots)
    {
        TargetTileKind = TargetTileEnum.MouseOverTile;

        base.Preview(mouseOverTile, skillshots);
        var target = OriginTargets[0];

        SkillsManager skillsManager = SkillsManager.Instance;
        skillsManager.GetAOE(this);

        if (PartData.TilesHit.Contains(mouseOverTile))
        {
            PartData.TilesHit.Clear();
            mouseOverTile.SetColor(SelectedTileColor);
            PartData.TilesHit.Add(mouseOverTile);

            return this;
        }
        PartData.TilesHit.Clear();

        return this;
    }
}
