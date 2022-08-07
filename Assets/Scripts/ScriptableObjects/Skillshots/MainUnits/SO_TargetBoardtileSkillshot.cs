using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TargetTileSkillshot", menuName = "ScriptableObjects/SkillshotParts/TargetTileSkillshot")]
public class SO_TargetBoardtileSkillshot : SO_Skillshot
{
    public override SO_Skillshot Preview(BoardTile mouseOverTile, List<SO_Skillshot> skillshots) 
    {
        base.Preview(mouseOverTile, skillshots);
        SkillShotManager skillShotManager = SkillShotManager.skillShotManager;
        skillShotManager.GetAOE(this);

        if (TilesHit.Contains(mouseOverTile))
        {
            TilesHit.Clear();
            TilesHit.Add(mouseOverTile);

            return this;
        }
        TilesHit.Clear();

        return this;
    }
}
