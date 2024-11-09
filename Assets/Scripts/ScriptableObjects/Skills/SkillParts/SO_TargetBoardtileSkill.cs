using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetTileSkillshot", menuName = "ScriptableObjects/SkillshotParts/TargetTileSkillshot")]
public class SO_TargetBoardtileSkill : SO_Skillpart
{
    public TileColor SelectedTileColor;

    public override SO_Skillpart Preview(BoardTile mouseOverTile, List<SO_Skillpart> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillsManager skillShotManager = SkillsManager.Instance;
        skillShotManager.GetAOE(this);

        if (TilesHit.Contains(mouseOverTile))
        {
            TilesHit.Clear();
            mouseOverTile.SetColor(SelectedTileColor);
            TilesHit.Add(mouseOverTile);

            return this;
        }
        TilesHit.Clear();

        return this;
    }
}
